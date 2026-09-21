[CmdletBinding()]
param(
    [switch]$CreateRelease,
    [switch]$SkipMsiValidation
)

$ErrorActionPreference = 'Stop'
if ($SkipMsiValidation -and -not $CreateRelease) { throw '-SkipMsiValidation requires -CreateRelease.' }
$root = Split-Path -Parent $PSScriptRoot
$artifacts = Join-Path $root 'artifacts'
$logoDirectory = Join-Path $root 'logos'
$stage = Join-Path $artifacts ('msix-stage-' + [guid]::NewGuid().ToString('N'))
$manifestPath = Join-Path $PSScriptRoot 'AppxManifest.xml'
[xml]$manifest = Get-Content -LiteralPath $manifestPath -Raw
$namespaces = [System.Xml.XmlNamespaceManager]::new($manifest.NameTable)
$namespaces.AddNamespace('app', 'http://schemas.microsoft.com/appx/manifest/foundation/windows10')
$identity = $manifest.SelectSingleNode('/app:Package/app:Identity', $namespaces)
$publisherDisplayName = $manifest.SelectSingleNode('/app:Package/app:Properties/app:PublisherDisplayName', $namespaces)
if ($null -eq $identity -or $null -eq $publisherDisplayName -or
    $identity.GetAttribute('Name') -cne 'BulentOzkir.ChangeTracker' -or
    $identity.GetAttribute('Publisher') -cne 'CN=06D08AF4-6BB1-40DF-9B96-5DF27BEE0635' -or
    $publisherDisplayName.InnerText -cne 'Bulent Ozkir') {
    throw 'Package identity does not match the supplied Partner Center values. Check packaging/AppxManifest.xml.'
}

$packageVersion = [version]$identity.GetAttribute('Version')
$releaseVersion = if ($packageVersion.Revision -eq 0) { $packageVersion.ToString(3) } else { $packageVersion.ToString(4) }
$packageBaseName = "ChangeTracker-$releaseVersion-x64"
$packagePath = Join-Path $artifacts ($packageBaseName + '.msix')
$releaseDirectory = Join-Path $root "releases\$releaseVersion"
$releaseNotes = Join-Path $root 'CHANGELOG.md'
if ($CreateRelease) {
    if (Test-Path -LiteralPath $releaseDirectory) { throw "Release output already exists and will not be overwritten: $releaseDirectory" }
    if ($packageVersion.Revision -ne 0) { throw 'MSI releases require a three-part product version; the MSIX revision must be zero.' }
    if (-not (Test-Path -LiteralPath $releaseNotes -PathType Leaf)) { throw 'CHANGELOG.md is required for a release.' }
    $heading = '(?m)^## ' + [regex]::Escape($releaseVersion) + ' - '
    if (-not [regex]::IsMatch((Get-Content -LiteralPath $releaseNotes -Raw), $heading)) { throw 'CHANGELOG.md does not contain notes for this package version.' }
}

& (Join-Path $logoDirectory 'Generate-Logos.ps1') -ValidateOnly | Out-Null

dotnet restore (Join-Path $PSScriptRoot 'BuildTools.csproj') --configfile (Join-Path $root 'NuGet.config') --verbosity minimal
if ($LASTEXITCODE -ne 0) { throw 'Approved SDK build-tools restore failed.' }

$sdk = dotnet msbuild (Join-Path $PSScriptRoot 'BuildTools.csproj') -getProperty:PkgMicrosoft_Windows_SDK_BuildTools
if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $sdk)) { throw 'SDK package path could not be resolved.' }
$makeAppx = Get-ChildItem -LiteralPath (Join-Path $sdk 'bin') -Filter makeappx.exe -Recurse |
    Where-Object FullName -Match '\\x64\\' | Select-Object -First 1 -ExpandProperty FullName
if (-not $makeAppx) { throw 'MakeAppx is not present in the approved SDK package.' }

dotnet publish (Join-Path $root 'src\PCChangeTracker.App\PCChangeTracker.App.csproj') --configuration Release --runtime win-x64 --self-contained true --output $stage -p:PublishTrimmed=false -nodeReuse:false
if ($LASTEXITCODE -ne 0) { throw 'Application publish failed.' }

if ($CreateRelease) {
    $binaryVersion = [version][System.Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $stage 'PCChangeTracker.exe')).FileVersion
    if ($binaryVersion -ne $packageVersion) { throw 'The published executable version does not match the MSIX manifest version.' }
}

Copy-Item -LiteralPath $manifestPath -Destination (Join-Path $stage 'AppxManifest.xml')
$assets = Join-Path $stage 'Assets'
[void][System.IO.Directory]::CreateDirectory($assets)
foreach ($logoName in @('ChangeTracker-Logo-44x44.png', 'ChangeTracker-Logo-71x71.png', 'ChangeTracker-Logo-150x150.png')) {
    Copy-Item -LiteralPath (Join-Path $logoDirectory $logoName) -Destination (Join-Path $assets $logoName)
}

$packagingLog = Join-Path $artifacts 'makeappx.log'
& $makeAppx pack /d $stage /p $packagePath /o *> $packagingLog
if ($LASTEXITCODE -ne 0) {
    Get-Content -LiteralPath $packagingLog -Tail 30 | Write-Output
    throw 'MSIX validation or packaging failed.'
}
Write-Output "Unsigned package with Partner Center identity: $packagePath"
Write-Output 'Store ID: 9PGK3NF5MK42. No certificate was created or installed, and no Store submission was performed.'

if ($CreateRelease) {
    $releaseStage = Join-Path $artifacts ('release-stage-' + [guid]::NewGuid().ToString('N'))
    [void][System.IO.Directory]::CreateDirectory($releaseStage)
    $bundleInput = Join-Path $artifacts ('bundle-input-' + [guid]::NewGuid().ToString('N'))
    [void][System.IO.Directory]::CreateDirectory($bundleInput)
    Copy-Item -LiteralPath $packagePath -Destination (Join-Path $bundleInput ($packageBaseName + '.msix'))
    $bundlePath = Join-Path $releaseStage ($packageBaseName + '.msixbundle')
    $bundleLog = Join-Path $artifacts 'makeappx-bundle.log'
    & $makeAppx bundle /d $bundleInput /p $bundlePath /bv $packageVersion.ToString(4) /o *> $bundleLog
    if ($LASTEXITCODE -ne 0) {
        Get-Content -LiteralPath $bundleLog -Tail 30 | Write-Output
        throw 'MSIX bundle validation or packaging failed.'
    }

    $msiOutput = Join-Path $artifacts ('msi-output-' + [guid]::NewGuid().ToString('N'))
    $msiProperties = @()
    if ($SkipMsiValidation) {
        $msiProperties += '-p:SuppressValidation=true'
        Write-Warning 'MSI ICE validation is explicitly skipped. Installer qualification remains required on a machine whose policy permits validation.'
    }
    dotnet build (Join-Path $PSScriptRoot 'Msi\ChangeTracker.wixproj') --configuration Release "-p:PublishDirectory=$stage" "-p:OutputPath=$msiOutput" "-p:Version=$releaseVersion" "-p:RestoreConfigFile=$(Join-Path $root 'NuGet.config')" @msiProperties -nodeReuse:false --verbosity minimal
    if ($LASTEXITCODE -ne 0) { throw 'MSI build failed.' }
    $msiPath = Join-Path $msiOutput ($packageBaseName + '.msi')
    if (-not (Test-Path -LiteralPath $msiPath -PathType Leaf)) { throw 'MSI build did not produce the expected installer.' }
    Copy-Item -LiteralPath $msiPath -Destination (Join-Path $releaseStage ($packageBaseName + '.msi'))
    Copy-Item -LiteralPath $releaseNotes -Destination (Join-Path $releaseStage 'RELEASE_NOTES.md')
    $checksumLines = @(Get-ChildItem -LiteralPath $releaseStage -File | Sort-Object Name | ForEach-Object {
        $hash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        '{0}  {1}' -f $hash, $_.Name
    })
    [System.IO.File]::WriteAllLines((Join-Path $releaseStage 'SHA256SUMS.txt'), $checksumLines, [System.Text.UTF8Encoding]::new($false))
    [void][System.IO.Directory]::CreateDirectory((Split-Path -Parent $releaseDirectory))
    [System.IO.Directory]::Move($releaseStage, $releaseDirectory)
    Write-Output "Local preview release: $releaseDirectory"
    Write-Output 'Includes unsigned x64 MSIX bundle and MSI, release notes, and SHA-256 checksums. No portable ZIP, installation, signing, tag, upload, or publication was performed.'
}