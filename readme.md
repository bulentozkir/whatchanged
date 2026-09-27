# ChangeTracker

**See what changed on your PC. Understand what matters.**

ChangeTracker is a native Windows utility for saving a local configuration baseline, comparing later checks, and explaining which changes deserve a closer look.

**One app, two modes:** choose **Simple** or **Advanced** with radio controls. Simple shows readable change summaries and text reports. Advanced adds inline category/change-kind/source-time details and JSON/CSV export. Both modes expose the same underlying evidence, source controls, saved history, and date comparisons. Switching never rescans, changes consent, grants administrator access, or costs extra.

## Project Status

**Working development build 1.0.2.** The WPF app, 11 scoped manual collectors, SQLite history, review-first comparisons, checkpoint management, expected-change annotations, and sanitized text/JSON/CSV reporting are implemented. This is an incremental development release, not the finished 20-category product or a Store-certified release.

**Release 1.0.2** redesigns the interface for accessibility and density: a color-vision-deficiency-safe palette with 7:1 text everywhere (blue interaction, distinct navy/light-blue field labels, bluish-green success, amber caution for replacing a baseline, red danger), a sticky command bar that stacks rather than squeezes, scope in the sidebar, a comparison header that states both ends while collapsed, a Snapshots list that fills the window beneath its commands, compact source cards in columns, 36-DIP controls, named column headers, themed disabled lists, dashed disabled buttons, visible high-contrast selection bars, and a sidebar version label. It also localizes the startup messages and fixes Ctrl+F in Help. See [CHANGELOG.md](CHANGELOG.md).

**Release 1.0.1** removed the optional administrator check so the app runs only with the signed-in user's standard permissions (Microsoft Store denied the `allowElevation` capability), and added the first design system (semantic command colors with icons, modern selectors, icon navigation, one-screen Settings), the Dark default theme, and maximized relaunch from the tray, on top of 1.0.0's current-user/machine-wide selection, retained date/time comparison selection, formatted offline help, scheduling, retention, and optional sign-in startup. Closing or minimizing always hides to the tray; tray Exit terminates the app. Local preview installers are built under `releases/1.0.2/`.

The full roadmap remains in [productspec.md](productspec.md). The manifest now uses the owner-supplied Partner Center identity below and no longer declares `allowElevation`. Signing, Windows 10/ARM64 qualification, and Store certification/publication remain outstanding. No application is installed or certificate trusted by the build scripts.

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

The first launch asks you to confirm **Current user**, **Machine-wide**, or both, and does not collect anything before confirmation. **Check now** uses standard access in either scope. Review the **Sources** page first; all supported checks, including network and PATH, default to enabled. Existing saved choices are preserved, and any source can be disabled. Unsupported sources remain off for that scope. A second compatible check compares against the saved baseline without moving it automatically.

## Offline Help

Select **Help me** or press **F1** for the local guide, also available before choosing scope. [Helpme.md](Helpme.md) is the English source; 19 translated guides in [help/](help/) are embedded alongside it at build time. All 20 contain 18 topics, including Settings, exact snapshot timestamps, appearance, automatic checks, retention, and startup/tray behavior. Markdig parses the guides into native WPF headings, lists, tables, and emphasized text; there is no browser, WebView, downloaded content, script execution, or online dependency at runtime. Translations still require native-speaker review before production release.

The help window offers topic/body search, **Ctrl+F** to focus search, **Escape** to close, and adjustable document text size. Tables become labeled vertical entries in narrow views or at larger zoom. It covers every current screen, modes, dates, baselines, scope, one-check consent, coverage, reports, privacy, keyboard use, troubleshooting, and preview limitations.

## Choose A Comparison

The **Comparison** picker is available in both modes. **Before date** selects an exact retained snapshot from a non-editable list. Choices show the local date, time including milliseconds and UTC offset, checkpoint, and scope/access; they do not accept arbitrary dates or times.

- **Today (new check):** **Check now** captures a fresh observation and compares it with the selected earlier snapshot, not necessarily the baseline.
- **Saved snapshot:** choose the retained timestamp under **After date**, then **Compare snapshots**. No collector runs and no snapshot is created.
- **Baseline:** select the saved standard-access baseline without replacing it.
- **Current state only:** clear the earlier reference to capture inventory without a comparison.

Selections do not run a check or move the baseline. Deleted or expired snapshots disappear from the choices. Missing, identical/reversed/overlapping, and incompatible scope/access selections are rejected. Both selectors and the history list include every retained scope, regardless of the current capture scope; saved administrator observations can be compared without elevation. An administrator reference never causes automatic elevation for a new capture. Reports describe the displayed result, not pending selections.

## Scope And Permissions

- **Current user:** that user's desktop app registrations, Run/RunOnce entries, effective default apps, audio defaults, proxy configuration, and PATH. System services, scheduled tasks, driver/update inventory, firewall profiles, and adapter settings are outside this scope.
- **Machine-wide:** shared system configuration, not other users' private profiles. Machine app/startup registrations, services, accessible tasks, updates, drivers, firewall profiles, adapter DNS/DHCP, and machine PATH are available. It does not load other users' registry hives or collect the administrator account's personal settings.
- **Every check uses standard access.** ChangeTracker runs only in the signed-in user's default, unelevated security context and never requests administrator access: there is no administrator mode, UAC prompt, elevated helper, or service. Machine-wide sources read what standard permissions allow; anything they cannot read is reported as an explicit coverage gap rather than retried with elevation. Scope selection, app restart, source selection, and Simple/Advanced mode changes never change permissions.

The executable manifest requests `asInvoker`, and collector workers are started directly (not through the shell) so they inherit the app's unelevated token. A launch with **Run as administrator** is rejected with a request to open normally, and workers refuse to run elevated. MSI installation is a separate machine-wide operation that requires administrator approval; the Store (MSIX) package does not. No service, scheduled task, startup registration, or background monitor is installed by a scope change.

Snapshots and reports identify scope and access. User, machine, and old mixed-scope snapshots have separate baselines and are not compared across these boundaries. Snapshots saved with administrator access by version 1.0.0 remain readable: they can be viewed, compared with each other, and reported, but never serve as the reference for a new check. Legacy snapshots retain their original meaning and are not relabeled as user-only. The history metadata version is 2; older builds refuse to open this history. Keep a backup before using an unreleased build with important existing history.

For MSIX, the manifest declares only the `runFullTrust` capability that desktop (Win32) apps require. Version 1.0.1 removed the restricted `allowElevation` capability after Microsoft Store certification denied it (policy 10.6.3); see [Microsoft's capability reference](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/app-capability-declarations#restricted-capability-list).

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
| Network | Current-user scope: proxy configuration with hidden URL fingerprint comparison. Machine scope: adapter DNS/DHCP only. | No packet capture, Wi-Fi passwords, network probes, full VPN profiles, or complete network inventory. |
| PATH | Persisted PATH for the selected user or machine scope, including order and duplicate entries. | No other environment variables, executable discovery, or directory scanning. |

Collectors execute sequentially in short-lived child processes of the same application with a 25-second per-source limit. Cancellation terminates the worker and preserves prior history. Partial/failed/disabled sources never generate inferred removals or a blanket all-clear. Standard checks never retry automatically with administrator privileges.

## Review And Share

- **Review first:** up to three explained findings initially, with the full count and View all. Routine/expected activity and unassessed changes are grouped separately.
- **Before / after:** inspect observed values, the detection interval, why the change is shown, and what is not known.
- **Visible changes:** each card shows Added/Removed/Modified plus a readable changed-value summary. Priority is named in text, not conveyed by color alone; group headings include their meaning as well as their count.
- **Expected:** annotate only the selected comparison occurrence; undo is available. No future publisher-wide suppression.
- **Snapshots:** retain manual and automatic checks, name checkpoints, explicitly replace the baseline, and delete non-baseline snapshots. There is no fixed checkpoint cap. Age-based retention prunes only unnamed, non-baseline snapshots; the default is 30 days, with saved choices (including Forever) preserved.
- **Advanced:** show additional inline technical observation details and expose JSON/CSV export. Date comparisons and full before/after inspection are available in Simple too.
- **Reports:** preview sanitized text before sharing; save text in either mode, plus JSON and CSV in Advanced. Nothing is sent automatically.

The inventory panel shows up to 1,000 records per source to keep the UI responsive; complete captured records remain in local snapshots and current-state exports. Current implementation compares snapshot endpoints; the persistent per-event timeline, cross-gap recovery, richer filters, tags/pins/notes, ignore rules, and guided investigations remain roadmap work.

## Local Data And Privacy

Default storage is `%LOCALAPPDATA%\PCChangeTracker`: the SQLite history and a current-user DPAPI-protected fingerprint key. Launch values and hidden proxy URL components are compared using HMAC, not stored in plaintext. The database itself is not encrypted. PATH, application names, source locations, and endpoint identities are local configuration metadata and can be sensitive.

The left sidebar shows **Snapshot storage** on every page. Its hover tooltip points to frequency and retention in Settings. The size includes all scopes in the current history database, database overhead/reusable space, and SQLite WAL/SHM files; it refreshes after history changes. Deletion does not necessarily shrink the files immediately. Named checkpoints and baselines stay protected.

Reports omit hidden fingerprints, keys, notes, and audio endpoint identifiers and redact profile paths and common credential patterns. Redaction is not a guarantee of anonymity; inspect the preview for organization names or other identifying metadata. Clear history deletes app-owned snapshots and expected marks; it retains source/mode preferences and the comparison key, and never deletes exported files. It is not forensic erasure. The optional data-retention preference deletes only unlabeled, non-baseline snapshots past the chosen age; named checkpoints and every scope/access baseline are never pruned automatically.

The app does not record microphone audio, execute collected commands, modify Windows settings, install updates, perform network scans, run cleanup, or upload diagnostics. Inspection buttons open allowlisted Windows settings/management tools for user-directed investigation. A change is not evidence of malware or proof of cause.

## Settings Appearance

Settings groups Language, Appearance, Automatic checks, Startup and tray, Local history, and privacy notes into cards that flow into up to three balanced columns, so the whole page fits one screen on typical displays (for example a maximized 1366×768 window at 100% text). Cards read top to bottom, then across, in the same order as Tab and screen readers. Review-only scope and comparison controls are hidden on Settings and return on the other pages. Narrow windows and larger text reduce the columns and scroll instead of hiding settings.

Open **Settings > Appearance** for the Dark/Light theme (**Dark** for new profiles; a saved choice is kept), font family, and separate **App text color**, **Label color**, **Button background color**, and **Button text color** choices. Named swatches offer Default, Navy, Forest green, Maroon, and Purple. Selections apply immediately across the app and are saved independently. Default restores the theme's color for that role; shades adapt when the theme changes. The default palette is designed for low vision and color-vision deficiency: every text role, including secondary text and field labels, reaches 7:1 on every surface; interactive color is blue; field labels have their own navy (Light) or light-blue (Dark) color; success is a bluish green kept distinct from red under protanopia and deuteranopia; and every status also has a word and a distinct glyph. With Default button colors, commands follow a semantic hierarchy instead of decoration: blue primary checks, an amber caution button for replacing a baseline, red danger buttons for deletion, and neutral buttons with color-coded icons (blue reports/viewing, violet scope/navigation, gold Help), all at 7:1 text and 3:1 outline contrast. Disabled buttons drop their fill for a dashed outline. Choosing a custom button background or text color applies it to all buttons with a visible outline. Dropdowns and fields use a 3:1 outline with an accent arrow and focus outline; check boxes, the sign-in switch, and radio buttons fill with the accent when on. Navigation and selected rows show a tint, an accent bar, and bold text; scrollbars and tooltips follow the theme; and the title bar goes dark in the Dark theme where Windows allows it. Primary buttons retain automatically contrasting text, and Windows high contrast takes precedence over customization.

The layout is compact: controls are 36 DIPs tall (above the WCAG 2.2 AA 24-pixel target minimum), the command bar (Simple/Advanced, Report, Check now) stays at the top on one row and moves the commands to a second row rather than squeezing them, scope and **Change scope** sit in the sidebar under the page navigation, the collapsed **Comparison** header states both ends of the selection, **Snapshots** keeps its commands above a list that fills the window, **Sources** shows compact cards in up to three columns, and the Report preview fills its window instead of scrolling the whole dialog.

**Text size** offers 100%, 125%, 150%, and 200%, saved per history directory and applied to all pages, controls, Help, and Report. Large text reflows paired controls and scrolls rather than hiding commands. Native selected-page navigation supports arrow keys; F6/Shift+F6 move between navigation, command bar, and page, and Ctrl+1 through Ctrl+4 open those pages directly. Help also supports F6 between search/topics/document and its separate document zoom. The offline guide describes modal focus behavior and screen-reader labels.

## Background Behavior

**Settings** offers **snapshot frequency** (Off, 15 minutes, hourly, every four hours, every six hours, daily, or weekly), **data retention** (30/90/180/365 days or forever), and optional sign-in startup. With no valid saved preference, snapshots default to **every 4 hours** and retention to **30 days**. Existing valid choices, including Off and Forever, are preserved. Sign-in startup remains off by default; tray residency is always enabled. Automatic checks begin only after scope confirmation, use standard access, never request elevation, and can be canceled. The first or an overdue check can run at the next minute check; subsequent checks follow the selected interval. They do not replace the selected comparison reference or baseline.

Retention cleanup runs when first due and then daily while the app is open, including when automatic capture is Off; it also runs after automatic captures. It removes only unnamed, non-baseline snapshots past the selected age across all scopes. Named checkpoints and all scope/access baselines are protected. The app checks due work once a minute and does not wake a sleeping computer.

**Start ChangeTracker when I sign in** registers only the app's per-user, unelevated `Run` entry with the same history directory and launches hidden in the tray. It includes sign-in after a reboot, not collection before anyone signs in. No service or boot-time scheduled task is installed. A failed registration leaves the previous choice intact and reports an error. Normal launches open maximized, including starting the app again while it runs in the tray. Minimizing or closing always hides the window and keeps checks running; tray Open or double-click restores its last visible state. Explicit tray Exit cancels active work and terminates the app.

These controls remain unqualified against `productspec.md`'s owner-approved resource budget and idle/active/OS/GPU profiling gate. Functional tests are not resource profiling. Tray Exit cancels any active check and exits when the worker stops; workers otherwise exist only during a requested or scheduled check.

Literal zero CPU during collection is impossible. Neither broad scope nor this manual build relaxes the resource condition. Visible WPF UI may use GPU resources for rendering.

## Tests

```powershell
# Unit, SQLite, privacy, standard-permission, and read-only collector boundary tests; no UAC.
dotnet test tests/PCChangeTracker.Tests --configuration Release --filter 'Category!=Desktop' -nodeReuse:false

# Native UI automation: opens real windows and performs read-only live checks.
dotnet test tests/PCChangeTracker.Tests --configuration Release --filter 'Category=Desktop' -nodeReuse:false
```

Desktop tests require an interactive unlocked Windows desktop. They use separate temporary databases, synthetic history for screenshots, and a real first/second capture test. They do not modify monitored settings. Test-created temporary history is deleted. Synthetic UI screenshots are written under `artifacts/screenshots/` and ignored by Git.

The automated suite covers incomplete-source suppression, stable identity, PATH ordering, protected-key mismatch, classification/coverage safeguards, scoped baseline persistence and migration, SQLite rollback, occurrence-specific expectations, redaction, CSV formula safety, scope/mode switching, same-day snapshot selection, snapshot-versus-today comparisons, empty-date validation, offline-help resource/search/formatting, report preview, and high-DPI resizing. A source scan and manifest checks guard against reintroducing `allowElevation`, UAC launch verbs, or elevating manifests. No test launches an elevated process.

Accessibility checks cover named controls and lists on every main page, Help and Report; native selected-page semantics; readable snapshot/source/topic names; text-size persistence; palette contrast in both themes; and sampled 100–200%/RTL layouts. Default text, labels, selected-row text, and primary-button text target 7:1, status glyphs 4.5:1, and control boundaries 3:1. Design-system tests also require a Dark value for every color token, keep the Default swatches equal to their tokens, fail on any brush a view references that is not a declared token, and forbid hard-coded colors outside `Themes/Colors.xaml`. Modal panels disable sidebar actions, set initial focus and restore it on dismissal; Help and Report have explicit starting focus. A guarded keyboard test exercises F6, page shortcuts and Escape only while the test process owns foreground input. If the desktop is locked, it cannot qualify that workflow. Manual Narrator, Windows contrast-theme/DPI matrices and user accessibility evaluation remain required; automated checks are not universal certification.

Manual qualification on an approved test machine remains required for installed-MSIX behavior, including machine-wide checks by a standard (non-administrator) user and the coverage gaps they report. Windows 10/ARM64, full Narrator/high-contrast qualification, MSI/MSIX installation/update/uninstall, and long-term resource profiling are also not yet verified.

## MSIX Build

```powershell
./packaging/Build-Package.ps1
```

This first checks the manifest against the supplied Partner Center identity and validates the PNG assets documented in [logos/README.md](logos/README.md), restores Microsoft Windows SDK BuildTools from the approved NuGet feed, publishes self-contained `win-x64`, copies the three manifest-named logos, and runs MakeAppx validation. Output: `artifacts/ChangeTracker-<version>-x64.msix` (for example `ChangeTracker-1.0.2-x64.msix`). Older packages are not updated by this build. The [logos/](logos/) folder holds nine flat, named files at fixed sizes (44 through 1920x1080), each showing the ChangeTracker name; these do not replace genuine screenshots or certification.

The package is **unsigned and uses the supplied Partner Center identity** from [packaging/AppxManifest.xml](packaging/AppxManifest.xml). A local build does not establish Store acceptance or make it a trusted sideload package. Complete the appropriate signing and certification workflow separately; a sideload signing certificate must match the manifest Publisher, not merely its display name. The script does not generate a certificate, alter trust stores, sideload, or submit the app.

## Local Preview Release

After running the tests, create a versioned local release bundle:

```powershell
./packaging/Build-Package.ps1 -CreateRelease
```

For version 1.0.2, output is `releases/1.0.2/` in the repository root and includes:

- `ChangeTracker-1.0.2-x64.msixbundle`: an unsigned bundle containing the x64 package with the supplied Store identity.
- `ChangeTracker-1.0.2-x64.msi` and `ChangeTracker-1.0.2-arm64.msi`: unsigned, self-contained installers for all users, with a Start menu shortcut, each built from its own `win-x64`/`win-arm64` publish. Installation requires administrator approval; the app itself does not. No separate .NET installation is required.
- `RELEASE_NOTES.md`: copied from [CHANGELOG.md](CHANGELOG.md).
- `SHA256SUMS.txt`: SHA-256 integrity hashes of the installers and external release notes, not a publisher signature.

No portable ZIP is generated. The MSI installs under `%ProgramFiles%\ChangeTracker` and leaves per-user history intact on uninstall. Use one installation format, not both; close the app before installing or upgrading. The MSI definition is [packaging/Msi/Package.wxs](packaging/Msi/Package.wxs), built with pinned WiX SDK 5.0.2 from the approved NuGet feed.

MSI ICE validation is enabled by default. If a build machine's policy prevents it, `-SkipMsiValidation` explicitly skips that build-time check and emits a warning; this is not installer qualification or a change to device security policy. Complete ICE and install/upgrade/uninstall testing on a suitable validation machine before distribution. This preview has not been installed or certified.

The script derives the output version from the package manifest, checks it against the published executable and changelog, and refuses to overwrite an existing versioned release folder. It does not bump versions, create Git tags/GitHub releases, sign, upload, or publish anything. These are unsigned preview artifacts, not an approved Store release; managed devices may block execution, and security policies must not be bypassed.

## Price And Remaining Roadmap

Fixed price: **$0.99 one-time for the entire app**, all features and both modes included. No mode fee, subscription, in-app purchase, or professional sign-in. This development build has no purchase flow. Store acquisition/licensing still needs production integration.

Still planned: remaining collector breadth (Store packages, Startup folders, display/device/printer configuration, extensions, selected policies/registry/files, certificates, listeners, expanded protection and environment data); a persistent event timeline; storage-budget retention and archive controls; rich notes/tags/rules; HTML/PDF and safe imported/encrypted reports; resource-approved monitoring; native-speaker localization review and full release qualification. These remaining capabilities are not represented as implemented by the UI. The longer roadmap and fixed price remain unchanged.

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