# ChangeTracker

**See what changed on your PC. Understand what matters.**

ChangeTracker is a native Windows utility for saving a local configuration baseline, comparing later checks, and explaining which changes deserve a closer look.

**One app, two modes:** choose **Simple** or **Advanced** with radio controls. Simple shows readable change summaries and text reports. Advanced adds inline category/change-kind/source-time details and JSON/CSV export. Both modes expose the same underlying evidence, source controls, saved history, and date comparisons. Switching never rescans, changes consent, grants administrator access, or costs extra.

## Project Status

**Working development build 0.1.0.** The WPF app, 11 scoped manual collectors, SQLite history, review-first comparisons, checkpoint management, expected-change annotations, and sanitized text/JSON/CSV reporting are implemented. This is an incremental development release, not the finished 20-category product or a Store-certified release.

**Unreleased source changes:** current-user/machine-wide selection, optional one-check administrator access, accessible radio controls and change summaries, date/snapshot comparison selection, formatted offline help, and a reorganized **Settings** page (language, light/dark theme, font, button text color, optional snapshot scheduling and retention, optional sign-in startup and tray residency) are implemented in source. The existing installers under `releases/0.1.0/` predate these changes and have not been replaced. See [CHANGELOG.md](CHANGELOG.md).

The full roadmap remains in [productspec.md](productspec.md). The manifest now uses the owner-supplied Partner Center identity below. Signing, Windows 10/ARM64 qualification, and Store certification/publication remain outstanding. No application is installed or certificate trusted by the build scripts.

## Store Identity

| Field | Partner Center value |
| --- | --- |
| Package identity name | `BulentOzkir.ChangeTracker` |
| Package identity publisher | `CN=06D08AF4-6BB1-40DF-9B96-5DF27BEE0635` |
| Publisher display name | Bulent Ozkir |
| Package family name (PFN) | `BulentOzkir.ChangeTracker_ghsxnkq5jxyxm` |
| Package SID | `S-1-15-2-1448008496-617310015-3993436849-2557138480-1316930502-1461559310-1083836187` |
| Store ID | `9PGK3NF5MK42` |

Store link: [ChangeTracker](https://apps.microsoft.com/detail/9PGK3NF5MK42). Store app deep link: `ms-windows-store://pdp/?productid=9PGK3NF5MK42`. Visibility depends on publication status; these links do not establish that the app is live.

The manifest contains Name, Publisher, and PublisherDisplayName. PFN and Package SID are derived Windows identities, not additional manifest attributes; Store ID belongs to the listing. Internal project/assembly names, executable name, and `%LOCALAPPDATA%\PCChangeTracker` remain stable so a branding-only change does not reset local history. Repository/remote names are unchanged. Installed-package data behavior must still be verified separately.

## Run

Prerequisites: Windows x64, .NET 10 SDK, and access to the approved NuGet feed. Run from the repository root:

```powershell
dotnet restore PCChangeTracker.slnx --configfile NuGet.config
dotnet build PCChangeTracker.slnx --configuration Release -nodeReuse:false
dotnet run --project src/PCChangeTracker.App --configuration Release --no-build
```

VS Code: press F5 using the configuration in [.vscode/launch.json](.vscode/launch.json), with the C# debugger installed. The build and non-UI test tasks are in [.vscode/tasks.json](.vscode/tasks.json).

To isolate development data:

```powershell
dotnet run --project src/PCChangeTracker.App --configuration Release --no-build -- --data-dir ./local-data/manual-test
```

The first launch asks you to choose **Current user** or **Machine-wide** and does not collect anything. **Check now** uses standard access in either scope. Review the **Sources** page first; network and PATH are opt-in. A second compatible check compares against the saved baseline without moving it automatically.

## Offline Help

Select **Help me** or press **F1** for the local guide, also available before choosing scope. [Helpme.md](Helpme.md) is the single source, embedded into the application at build time. Markdig parses it into native WPF headings, lists, tables, and emphasized text; there is no browser, WebView, downloaded content, script execution, or online dependency at runtime.

The help window offers topic/body search, **Ctrl+F** to focus search, **Escape** to close, and adjustable document text size. Tables become labeled vertical entries in narrow views or at larger zoom. It covers every current screen, modes, dates, baselines, scope, one-check consent, coverage, reports, privacy, keyboard use, troubleshooting, and preview limitations.

## Choose A Comparison

The **Comparison** picker is available in both modes. **Before date** selects an exact retained snapshot from a non-editable list. Choices show the local date, time including milliseconds and UTC offset, checkpoint, and scope/access; they do not accept arbitrary dates or times.

- **Today (new check):** **Check now** captures a fresh observation and compares it with the selected earlier snapshot, not necessarily the baseline.
- **Saved snapshot:** choose the retained timestamp under **After date**, then **Compare snapshots**. No collector runs and no snapshot is created.
- **Baseline:** select the saved standard-access baseline without replacing it.
- **Current state only:** clear the earlier reference to capture inventory without a comparison.

Selections do not run a check or move the baseline. Deleted or expired snapshots disappear from the choices. Missing, identical/reversed/overlapping, and incompatible scope/access selections are rejected. Both selectors and the history list include every retained scope, regardless of the current capture scope; saved administrator observations can be compared without elevation. An administrator reference never causes automatic elevation for a new capture. Reports describe the displayed result, not pending selections.

## Scope And Administrator Access

- **Current user:** that user's desktop app registrations, Run/RunOnce entries, effective default apps, audio defaults, proxy configuration, and PATH. System services, scheduled tasks, driver/update inventory, firewall profiles, and adapter settings are outside this scope.
- **Machine-wide:** shared system configuration, not other users' private profiles. Machine app/startup registrations, services, accessible tasks, updates, drivers, firewall profiles, adapter DNS/DHCP, and machine PATH are available. It does not load other users' registry hives or collect the administrator account's personal settings.
- **Check now:** always standard access. Scope selection, app restart, source selection, and Simple/Advanced mode changes never request elevation. Inaccessible sources remain explicit coverage gaps.
- **Check with administrator access:** available only in machine-wide scope. Requires an explicit click, an app confirmation defaulting to No, and Windows UAC authorization. Permission applies to that check only; it is never saved as a preference. Declining either prompt leaves the baseline and history unchanged.

The UI must run unelevated; an explicitly elevated main-window launch is rejected with a request to open normally. MSI installation is a separate machine-wide operation that requires administrator approval. No service, scheduled task, startup registration, or background monitor is installed by a scope change.

The elevated helper has a fixed, read-only collector allowlist. A local named pipe restricts access to the initiating user and administrators, rejects network access, and verifies both peer process IDs; the helper also checks the parent executable path and disables pipe impersonation. Its request contains only category IDs and a transient comparison key, never an output path, command, user profile, or database location. Only the unelevated UI saves history. Each collector has a 25-second timeout; the helper has a five-minute bound and cancels workers on parent exit or pipe disconnect. Canceling in the app cannot dismiss the Windows secure-desktop UAC prompt; dismiss that prompt in Windows if it is still open.

Snapshots and reports identify scope and access. User, standard machine, administrator machine, and old mixed-scope snapshots have separate baselines. They are not compared across these boundaries. Legacy snapshots retain their original meaning and are not relabeled as user-only. The history metadata version advances to 2 without rewriting snapshot payloads; older builds refuse to open this history. Keep a backup before using an unreleased build with important existing history.

For MSIX, the source manifest declares the restricted `allowElevation` capability alongside `runFullTrust`. This does not elevate startup or replace UAC. Microsoft Store approval for `allowElevation` is a separate, strict requirement and has not been obtained; see [Microsoft's capability reference](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/app-capability-declarations#restricted-capability-list). Installed-MSIX and cross-account elevation still require manual qualification.

## Implemented Collection

| Source | Current scope | Important limits |
| --- | --- | --- |
| Installed apps | Desktop uninstall registrations and versions from the selected user or machine hive, never both in one check. | Store packages and portable apps are not enumerated. Registration identity changes can appear as removal/addition. |
| Startup entries | Run and RunOnce values from the selected user or machine hive, with private launch-value comparison. | No Startup folder or packaged-startup inventory; registration is not proof of execution. |
| Services | Machine scope: registered Win32 services and startup/delayed-start configuration; launch-value fingerprints. | No runtime polling, driver-service inventory, or full dependency/account analysis. |
| Scheduled tasks | Machine scope: accessible task names, enabled state, trigger types, and protected fingerprints of actions/triggers/settings/principals. | No raw task XML or commands saved; inaccessible entries cause partial coverage. |
| Windows updates | Machine scope: successful locally exposed install/uninstall history with event deduplication. | At most 5,000 history entries per check; a larger history is explicitly partial and not compared. Missing old events are not uninstalls. |
| Drivers | Machine scope: WMI signed-device driver identities, versions, providers, and INF names. | No firmware probing, driver rollback, or install-time inference. |
| Default apps | Current-user scope: effective HTTP/HTTPS, PDF, JPG, PNG, MP3, MP4, ZIP, and CSV associations. | No protected association writes. |
| Audio defaults | Current-user scope: playback/recording and multimedia/communications endpoint defaults. | No recording, sound playback, or application-specific override detection. |
| Protection | Machine scope: Windows Firewall profile enabled state. | Not a Defender/antivirus status collector or security verdict. |
| Network (opt-in) | Current-user scope: proxy configuration with hidden URL fingerprint comparison. Machine scope: adapter DNS/DHCP only. | No packet capture, Wi-Fi passwords, network probes, full VPN profiles, or complete network inventory. |
| PATH (opt-in) | Persisted PATH for the selected user or machine scope, including order and duplicate entries. | No other environment variables, executable discovery, or directory scanning. |

Collectors execute sequentially in short-lived child processes of the same application with a 25-second per-source limit. Cancellation terminates the worker and preserves prior history. Partial/failed/disabled sources never generate inferred removals or a blanket all-clear. Standard checks never retry automatically with administrator privileges.

## Review And Share

- **Review first:** up to three explained findings initially, with the full count and View all. Routine/expected activity and unassessed changes are grouped separately.
- **Before / after:** inspect observed values, the detection interval, why the change is shown, and what is not known.
- **Visible changes:** each card shows Added/Removed/Modified plus a readable changed-value summary. Priority is named in text, not conveyed by color alone; group headings include their meaning as well as their count.
- **Expected:** annotate only the selected comparison occurrence; undo is available. No future publisher-wide suppression.
- **Snapshots:** retain manual checks, name checkpoints, explicitly replace the baseline, and delete non-baseline snapshots. There is no fixed checkpoint cap or automatic pruning.
- **Advanced:** show additional inline technical observation details and expose JSON/CSV export. Date comparisons and full before/after inspection are available in Simple too.
- **Reports:** preview sanitized text before sharing; save text in either mode, plus JSON and CSV in Advanced. Nothing is sent automatically.

The inventory panel shows up to 1,000 records per source to keep the UI responsive; complete captured records remain in local snapshots and current-state exports. Current implementation compares snapshot endpoints; the persistent per-event timeline, cross-gap recovery, richer filters, tags/pins/notes, ignore rules, and guided investigations remain roadmap work.

## Local Data And Privacy

Default storage is `%LOCALAPPDATA%\PCChangeTracker`: the SQLite history and a current-user DPAPI-protected fingerprint key. Launch values and hidden proxy URL components are compared using HMAC, not stored in plaintext. The database itself is not encrypted. PATH, application names, source locations, and endpoint identities are local configuration metadata and can be sensitive.

Reports omit hidden fingerprints, keys, notes, and audio endpoint identifiers and redact profile paths and common credential patterns. Redaction is not a guarantee of anonymity; inspect the preview for organization names or other identifying metadata. Clear history deletes app-owned snapshots and expected marks; it retains source/mode preferences and the comparison key, and never deletes exported files. It is not forensic erasure. The optional data-retention preference deletes only unlabeled, non-baseline snapshots past the chosen age; named checkpoints and every scope/access baseline are never pruned automatically.

The app does not record microphone audio, execute collected commands, modify Windows settings, install updates, perform network scans, run cleanup, or upload diagnostics. Inspection buttons open allowlisted Windows settings/management tools for user-directed investigation. A change is not evidence of malware or proof of cause.

## Settings Appearance

Open **Settings > Appearance** for the Light/Dark theme, font family, and separate **App text color**, **Label color**, **Button background color**, and **Button text color** choices. Named swatches offer Default, Navy, Forest green, Maroon, and Purple. Selections apply immediately across the app and are saved independently. Default restores the theme's color for that role; shades adapt when the theme changes. Primary buttons retain automatically contrasting text, and Windows high contrast takes precedence over customization.

## Background Behavior

**Settings** offers optional **snapshot frequency** (15 minutes, hourly, every six hours, daily, or weekly), **data retention** (30/90/180/365 days or forever), sign-in startup, and tray residency. Automatic capture, startup, and tray residency default to Off; retention defaults to Forever. Automatic checks use the chosen scope with standard access and never request elevation. They are cancellable and do not replace the selected comparison reference or baseline.

Retention cleanup runs when first due and then daily while the app is open, including when automatic capture is Off; it also runs after automatic captures. It removes only unnamed, non-baseline snapshots past the selected age across all scopes. Named checkpoints and all scope/access baselines are protected. The app checks due work once a minute and does not wake a sleeping computer.

**Start ChangeTracker when I sign in** registers only the app's per-user, unelevated `Run` entry with the same history directory. It includes sign-in after a reboot, not collection before anyone signs in. No service or boot-time scheduled task is installed. A failed registration leaves the previous choice intact and reports an error. With tray residency enabled, minimizing or closing hides the window and keeps checks running; Open or launching another copy restores it. Explicit tray Exit cancels active work and terminates the app.

These controls remain unqualified against `productspec.md`'s owner-approved resource budget and idle/active/OS/GPU profiling gate. Functional tests are not resource profiling. Closing without tray residency cancels any active check and exits when the worker stops; workers otherwise exist only during a requested or scheduled check.

Literal zero CPU during collection is impossible. Neither broad scope nor this manual build relaxes the resource condition. Visible WPF UI may use GPU resources for rendering.

## Tests

```powershell
# Unit, SQLite, privacy, consent, IPC, and read-only collector boundary tests; no UAC.
dotnet test tests/PCChangeTracker.Tests --configuration Release --filter 'Category!=Desktop' -nodeReuse:false

# Native UI automation: opens real windows and performs read-only live checks.
dotnet test tests/PCChangeTracker.Tests --configuration Release --filter 'Category=Desktop' -nodeReuse:false
```

Desktop tests require an interactive unlocked Windows desktop. They use separate temporary databases, synthetic history for screenshots, and a real first/second capture test. They do not modify monitored settings. Test-created temporary history is deleted. Synthetic UI screenshots are written under `artifacts/screenshots/` and ignored by Git.

The automated suite covers incomplete-source suppression, stable identity, PATH ordering, protected-key mismatch, classification/coverage safeguards, scoped baseline persistence and migration, SQLite rollback, occurrence-specific expectations, redaction, CSV formula safety, scope/mode switching, same-day snapshot selection, snapshot-versus-today comparisons, empty-date validation, offline-help resource/search/formatting, report preview, and high-DPI resizing. Consent and denial use a fake capture service; IPC tests run unelevated. No automated test approves UAC or launches an elevated process.

Accessibility checks assert named radio controls, meaningful group headings, labeled date/snapshot fields, keyboard focus styling, and desktop/smaller-window layouts. Default body/secondary/priority text colors meet a 7:1 contrast target, primary-button text meets 4.5:1, and the four optional custom button text colors meet at least 4.5:1 against both theme surfaces; control outlines meet 3:1 against their intended backgrounds. Page titles, section headings, and finding names expose a heading level for screen-reader navigation. The finding detail overlay moves focus into itself on open, closes on Escape, cycles Tab within itself, and disables the rest of the window so keyboard focus cannot land on a hidden background control while it is showing. System high-contrast colors can be applied/restored without changing Windows settings. These checks are not full Narrator or universal accessibility certification.

Manual qualification on an approved test machine remains required for real UAC approval, denial, cancellation during and after the prompt, a standard user supplying a different administrator account, parent exit during an elevated check, and installed-MSIX behavior. Confirm no administrator-profile data is collected, the main window remains unelevated, helpers exit, and subsequent normal checks retain standard access. Windows 10/ARM64, full Narrator/high-contrast qualification, MSI/MSIX installation/update/uninstall, and long-term resource profiling are also not yet verified.

## MSIX Build

```powershell
./packaging/Build-Package.ps1
```

This first checks the manifest against the supplied Partner Center identity and validates the PNG assets documented in [logos/README.md](logos/README.md), restores Microsoft Windows SDK BuildTools from the approved NuGet feed, publishes self-contained `win-x64`, copies the three manifest-named logos, and runs MakeAppx validation. Output: `artifacts/ChangeTracker-0.1.0-x64.msix`. Older packages are not updated by this build. The [logos/](logos/) folder holds nine flat, named files at fixed sizes (44 through 1920x1080), each showing the ChangeTracker name; these do not replace genuine screenshots or certification.

The package is **unsigned and uses the supplied Partner Center identity** from [packaging/AppxManifest.xml](packaging/AppxManifest.xml). A local build does not establish Store acceptance or make it a trusted sideload package. Complete the appropriate signing and certification workflow separately; a sideload signing certificate must match the manifest Publisher, not merely its display name. The script does not generate a certificate, alter trust stores, sideload, or submit the app.

## Local Preview Release

After running the tests, create a versioned local release bundle:

```powershell
./packaging/Build-Package.ps1 -CreateRelease
```

For version 0.1.0, output is `releases/0.1.0/` in the repository root and includes:

- `ChangeTracker-0.1.0-x64.msixbundle`: an unsigned bundle containing the x64 package with the supplied Store identity.
- `ChangeTracker-0.1.0-x64.msi`: an unsigned, self-contained x64 installer for all users, with a Start menu shortcut. Installation requires administrator approval; the app itself does not. No separate .NET installation is required.
- `RELEASE_NOTES.md`: copied from [CHANGELOG.md](CHANGELOG.md).
- `SHA256SUMS.txt`: SHA-256 integrity hashes of both installers and external release notes, not a publisher signature.

No portable ZIP is generated. The MSI installs under `%ProgramFiles%\ChangeTracker` and leaves per-user history intact on uninstall. Use one installation format, not both; close the app before installing or upgrading. The MSI definition is [packaging/Msi/Package.wxs](packaging/Msi/Package.wxs), built with pinned WiX SDK 5.0.2 from the approved NuGet feed.

MSI ICE validation is enabled by default. If a build machine's policy prevents it, `-SkipMsiValidation` explicitly skips that build-time check and emits a warning; this is not installer qualification or a change to device security policy. Complete ICE and install/upgrade/uninstall testing on a suitable validation machine before distribution. This preview has not been installed or certified.

The script derives the output version from the package manifest, checks it against the published executable and changelog, and refuses to overwrite an existing versioned release folder. It does not bump versions, create Git tags/GitHub releases, sign, upload, or publish anything. These are unsigned preview artifacts, not an approved Store release; managed devices may block execution, and security policies must not be bypassed.

## Price And Remaining Roadmap

Fixed price: **$0.99 one-time for the entire app**, all features and both modes included. No mode fee, subscription, in-app purchase, or professional sign-in. This development build has no purchase flow. Store acquisition/licensing still needs production integration.

Still planned: remaining collector breadth (Store packages, Startup folders, display/device/printer configuration, extensions, selected policies/registry/files, certificates, listeners, expanded protection and environment data); a persistent event timeline; retention controls; rich notes/tags/rules; HTML/PDF and safe imported/encrypted reports; resource-approved monitoring; localization and full release qualification. None are represented as implemented by the UI. The longer roadmap and fixed price remain unchanged.

## Solution And Dependencies

| Project | Responsibility |
| --- | --- |
| PCChangeTracker.Core | Snapshot contracts, deterministic comparison, SQLite history, sanitized reports. |
| PCChangeTracker.Windows | Scoped read-only Windows collectors and protected comparison keys. |
| PCChangeTracker.App | WPF/MVVM UI, isolated collector workers, source preferences, single-instance activation, report dialogs. |
| PCChangeTracker.Tests | Unit/integration tests and native UI automation. |

Only approved dependency feeds may be used. [NuGet.config](NuGet.config) clears inherited sources and maps all packages to the approved feed:

- NuGet: `https://packagefeedproxy.microsoft.io/nuget/v3/index.json`
- PyPI, if future tooling needs it: `https://packagefeedproxy.microsoft.io/pypi/simple`

There is no public-feed fallback. Python is not part of the app runtime. SQLitePCLRaw is explicitly pinned to avoid the vulnerable native dependency pulled by older default bundles; dependency audits must continue before release.

The repository's [.gitignore](.gitignore) excludes common .NET/Visual Studio outputs, generated Store packages, local databases, snapshot/export directories, and signing secrets. Shared VS Code configuration, package manifests, and synthetic text-based test fixtures remain trackable.

Keep development captures and private reports in the ignored `local-data/`, `snapshots/`, or `exports/` directories, or outside the repository. Never commit real PC inventories, credentials, or private signing keys. Ignore rules do not remove already tracked files or replace a review of staged changes.

Validate with fixtures and isolated adapters before live read-only checks. Any live system mutation test belongs in a disposable or explicitly approved environment, not a developer's real PC. Do not confuse successful tests on one PC with universal source coverage or Store readiness.