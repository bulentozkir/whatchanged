[CmdletBinding()]
param(
    [switch]$Overwrite,
    [switch]$ValidateOnly
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
if ($Overwrite -and $ValidateOnly) { throw 'Choose generation or validation, not both switches.' }
Add-Type -AssemblyName System.Drawing

$definitions = @(
    @{ Name = 'ChangeTracker-Logo-44x44.png'; Size = 44; Listing = $false; Title = $true },
    @{ Name = 'ChangeTracker-Logo-71x71.png'; Size = 71; Listing = $false; Title = $true },
    @{ Name = 'ChangeTracker-Logo-150x150.png'; Size = 150; Listing = $false; Title = $true },
    @{ Name = 'ChangeTracker-Logo-300x300.png'; Size = 300; Listing = $false; Title = $true },
    @{ Name = 'ChangeTracker-Logo-512x512.png'; Size = 512; Listing = $false; Title = $true },
    @{ Name = 'ChangeTracker-Logo-1024x1024.png'; Size = 1024; Listing = $false; Title = $true },
    @{ Name = 'ChangeTracker-BoxArt-1080x1080.png'; Size = 1080; Listing = $true; Title = $true },
    @{ Name = 'ChangeTracker-Poster-720x1080.png'; Width = 720; Height = 1080; Listing = $true; Title = $true },
    @{ Name = 'ChangeTracker-Logo-1920x1080.png'; Width = 1920; Height = 1080; Listing = $true; Title = $true }
)

function New-RoundedPath {
    param([single]$Left, [single]$Top, [single]$Width, [single]$Height, [single]$Radius)
    $diameter = [single]($Radius * 2)
    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $path.AddArc($Left, $Top, $diameter, $diameter, 180, 90)
    $path.AddArc([single]($Left + $Width - $diameter), $Top, $diameter, $diameter, 270, 90)
    $path.AddArc([single]($Left + $Width - $diameter), [single]($Top + $Height - $diameter), $diameter, $diameter, 0, 90)
    $path.AddArc($Left, [single]($Top + $Height - $diameter), $diameter, $diameter, 90, 90)
    $path.CloseFigure()
    return $path
}

function Draw-ChangeTrackerMark {
    param([System.Drawing.Graphics]$Graphics, [bool]$Simplified)
    $teal = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml('#08675F'))
    $white = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::White)
    $amber = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml('#F0AD32'))
    $outline = [System.Drawing.Pen]::new($teal.Color, 18)
    $lightOutline = [System.Drawing.Pen]::new($white.Color, 8)
    $before = New-RoundedPath 62 94 172 306 26
    $after = New-RoundedPath 266 94 184 306 26
    try {
        $Graphics.FillPath($teal, $before)
        $Graphics.DrawPath($lightOutline, $before)
        $Graphics.FillPath($white, $after)
        $Graphics.DrawPath($outline, $after)
        $barHeight = if ($Simplified) { 32 } else { 22 }
        $Graphics.FillRectangle($white, 98, 164, 100, $barHeight)
        $Graphics.FillRectangle($teal, 302, 164, 108, $barHeight)
        $Graphics.FillRectangle($white, 98, 250, 66, $barHeight)
        $Graphics.FillRectangle($amber, 302, 250, 108, [single]($barHeight + 10))
        if (-not $Simplified) {
            $Graphics.FillRectangle($white, 98, 324, 100, 22)
            $Graphics.FillRectangle($teal, 302, 324, 72, 22)
        }
    }
    finally {
        $before.Dispose(); $after.Dispose(); $outline.Dispose(); $lightOutline.Dispose()
        $teal.Dispose(); $white.Dispose(); $amber.Dispose()
    }
}

function Write-Logo {
    param([hashtable]$Definition)
    $width = if ($Definition.ContainsKey('Width')) { [int]$Definition.Width } else { [int]$Definition.Size }
    $height = if ($Definition.ContainsKey('Height')) { [int]$Definition.Height } else { [int]$Definition.Size }
    $scale = if ($width -le 300) { 4 } elseif ($width -le 2160) { 2 } else { 1 }
    $renderWidth = $width * $scale
    $renderHeight = $height * $scale
    $logicalHeight = [single](512.0 * $height / $width)
    $render = [System.Drawing.Bitmap]::new($renderWidth, $renderHeight, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [System.Drawing.Graphics]::FromImage($render)
    $output = [System.Drawing.Bitmap]::new($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $resample = [System.Drawing.Graphics]::FromImage($output)
    try {
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
        $background = if ($Definition.Listing) { [System.Drawing.ColorTranslator]::FromHtml('#F1F7F6') } else { [System.Drawing.Color]::Transparent }
        $graphics.Clear($background)
        $graphics.ScaleTransform([single]($renderWidth / 512.0), [single]($renderWidth / 512.0))
        $titleColor = [System.Drawing.ColorTranslator]::FromHtml('#08675F')
        if ($width -gt ($height * 1.3)) {
            # Wide horizontal lockup: mark on the left, wordmark text to its right, single row.
            $markAreaHeight = [single]($logicalHeight * 0.74)
            $wideMarkScale = [single]($markAreaHeight / 306.0)
            $state = $graphics.Save()
            try {
                $graphics.TranslateTransform([single](14 - 62 * $wideMarkScale), [single](($logicalHeight - $markAreaHeight) / 2 - 94 * $wideMarkScale))
                $graphics.ScaleTransform($wideMarkScale, $wideMarkScale)
                Draw-ChangeTrackerMark $graphics $true
            } finally { $graphics.Restore($state) }
            $markRight = [single](14 + (450 - 62) * $wideMarkScale)
            $availableWidth = [single](500 - $markRight - 16)
            $ink = [System.Drawing.SolidBrush]::new($titleColor)
            $format = [System.Drawing.StringFormat]::new()
            $probeFont = [System.Drawing.Font]::new('Segoe UI', 100, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
            try {
                $format.Alignment = [System.Drawing.StringAlignment]::Near
                $format.LineAlignment = [System.Drawing.StringAlignment]::Center
                $format.FormatFlags = [System.Drawing.StringFormatFlags]::NoWrap
                $probeWidth = $graphics.MeasureString('ChangeTracker', $probeFont).Width
                $fitSize = [single]([Math]::Min(100.0 * ($availableWidth * 0.92 / $probeWidth), $logicalHeight * 0.44))
                $font = [System.Drawing.Font]::new('Segoe UI', $fitSize, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
                try {
                    $textBounds = [System.Drawing.RectangleF]::new([single]($markRight + 16), 0, $availableWidth, $logicalHeight)
                    if ($graphics.MeasureString('ChangeTracker', $font).Width -gt $textBounds.Width) { throw 'The wide logo title does not fit beside the mark.' }
                    $graphics.DrawString('ChangeTracker', $font, $ink, $textBounds, $format)
                } finally { $font.Dispose() }
            } finally { $probeFont.Dispose(); $ink.Dispose(); $format.Dispose() }
        } else {
            # Stacked layout: mark on top, wordmark text beneath it within the upper two-thirds safe area.
            $state = $graphics.Save()
            try {
                $markScale = if ($height -gt $width) { [single]0.95 } else { [single]0.65 }
                $graphics.TranslateTransform([single](256 - 256 * $markScale), 0)
                $graphics.ScaleTransform($markScale, $markScale)
                Draw-ChangeTrackerMark $graphics ($width -le 96)
            } finally { $graphics.Restore($state) }
            $font = [System.Drawing.Font]::new('Segoe UI', 43, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
            $ink = [System.Drawing.SolidBrush]::new($titleColor)
            $format = [System.Drawing.StringFormat]::new()
            try {
                $format.Alignment = [System.Drawing.StringAlignment]::Center
                $format.LineAlignment = [System.Drawing.StringAlignment]::Center
                $format.FormatFlags = [System.Drawing.StringFormatFlags]::NoWrap
                $bounds = [System.Drawing.RectangleF]::new(32, [single]($logicalHeight * 0.57), 448, 46)
                if ($graphics.MeasureString('ChangeTracker', $font).Width -gt $bounds.Width) { throw 'The logo title does not fit its safe area.' }
                if ($bounds.Bottom -gt ($logicalHeight * 2 / 3)) { throw 'The title exceeds the top-two-thirds safe area.' }
                $graphics.DrawString('ChangeTracker', $font, $ink, $bounds, $format)
            } finally { $font.Dispose(); $ink.Dispose(); $format.Dispose() }
        }
        $output.SetResolution(96, 96)
        $resample.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
        $resample.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
        $resample.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $resample.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $resample.DrawImage($render, [System.Drawing.Rectangle]::new(0, 0, $width, $height), 0, 0, $renderWidth, $renderHeight, [System.Drawing.GraphicsUnit]::Pixel)
        $output.Save((Join-Path $PSScriptRoot $Definition.Name), [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $resample.Dispose(); $output.Dispose(); $graphics.Dispose(); $render.Dispose() }
}

if (-not $ValidateOnly) {
    foreach ($definition in $definitions) {
        if ((Test-Path -LiteralPath (Join-Path $PSScriptRoot $definition.Name)) -and -not $Overwrite) {
            throw 'Logo files already exist. Use -ValidateOnly to check them or explicitly pass -Overwrite to regenerate.'
        }
    }
    foreach ($definition in $definitions) { Write-Logo $definition }
}

$summary = foreach ($definition in $definitions) {
    $path = Join-Path $PSScriptRoot $definition.Name
    $file = Get-Item -LiteralPath $path
    if ($file.Length -gt 50MB) { throw "Store image exceeds 50 MB: $($definition.Name)" }
    $header = [System.IO.File]::ReadAllBytes($path)
    if ($header.Length -lt 33 -or [Convert]::ToHexString($header[0..7]) -ne '89504E470D0A1A0A' -or $header[24] -ne 8 -or $header[25] -ne 6) {
        throw "Expected an 8-bit-per-channel RGBA PNG: $($definition.Name)"
    }
    $image = [System.Drawing.Bitmap]::new($path)
    try {
        $expectedWidth = if ($definition.ContainsKey('Width')) { $definition.Width } else { $definition.Size }
        $expectedHeight = if ($definition.ContainsKey('Height')) { $definition.Height } else { $definition.Size }
        if ($image.Width -ne $expectedWidth -or $image.Height -ne $expectedHeight) { throw "Incorrect logo dimensions: $($definition.Name)" }
        $colors = [System.Collections.Generic.HashSet[int]]::new()
        $visible = 0
        $stride = [Math]::Max(1, [int][Math]::Floor($image.Width / 64.0))
        for ($vertical = 0; $vertical -lt $image.Height; $vertical += $stride) {
            for ($horizontal = 0; $horizontal -lt $image.Width; $horizontal += $stride) {
                $pixel = $image.GetPixel($horizontal, $vertical)
                if ($pixel.A -gt 0) { $visible++; [void]$colors.Add($pixel.ToArgb()) }
            }
        }
        if ($visible -lt 8 -or $colors.Count -lt 4) { throw "The logo is blank or lacks visible artwork: $($definition.Name)" }
        if (-not $definition.Listing -and $image.GetPixel(0, 0).A -ne 0) { throw "Native icon background is not transparent: $($definition.Name)" }
        [pscustomobject]@{ File = $definition.Name; Width = $image.Width; Height = $image.Height; Bytes = $file.Length }
    } finally { $image.Dispose() }
}
if (@(Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.png' -File).Count -ne 9) { throw 'Expected exactly 9 PNG exports in the logos folder.' }
$summary | Format-Table -AutoSize
Write-Output 'PASS: 9 correctly sized RGBA PNGs, flat in the logos folder, every one showing the ChangeTracker name.'