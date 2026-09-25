# Changelog

## Unreleased

- No changes recorded.

## 1.0.0 - 2026-09-23

- Improve accessibility across Review, Snapshots, Sources, Settings, Help and Report: selected-page list navigation, readable item/group names, source status help, a visible checkpoint label, themed list selections, wrapping control text and 44-DIP minimum interaction heights. Modal scope/details now disable all background sidebar actions and manage focus explicitly.
- Add a saved 100/125/150/200% text-size setting, adaptive paired columns, scrollable large-text layouts, shared Help/Report fonts, F6 region navigation and Ctrl+1 through Ctrl+4 page shortcuts. Document keyboard and screen-reader behavior in offline help; full manual assistive-technology qualification remains outstanding.
- Enable all supported checks by default, including network configuration and persisted PATH. Preserve saved scoped/legacy opt-outs, keep unsupported sources disabled, and test that explicit captures receive the complete default set without implicit capture or elevation.
- Show snapshot-history storage in the left sidebar on every page, with localized size units, accessible hover guidance for frequency/retention, and refresh after history changes. Count database/WAL/SHM files across all scopes and explain reusable database space instead of promising immediate shrinkage after deletion.
- Start normal launches maximized, preserve a manually restored window size through tray restore, and keep sign-in launches hidden. Close, Alt+F4, and Minimize always hide the main window; only the tray Exit command performs an app-requested shutdown and cancels active work.
- Make Simple changes readable with larger selectable Before/After field values, repeated labels, field-specific screen-reader names, structured details, and keyboard focus return. Add retained-day filters and separate exact capture-time lists in Advanced without changing saved selections, capture scope, or baselines.
- Replace the undersized full-wordmark Windows icon with a tightly framed before/after mark at nine native sizes from 16 to 256 pixels. Use it for the executable, window/taskbar, and tray; preserve the nine wordmarked PNG exports in `logos/` unchanged. Generate the ICO in build intermediates with PowerShell 7.
- Default snapshot frequency to every 4 hours (240 minutes) and retention to 30 days when no valid preference is saved. Preserve existing valid selections, including Off and Forever, keep all baselines and named checkpoints protected, and add the four-hour option and updated guidance in all 20 languages.
- Synchronize all 20 embedded help guides with the current Settings, exact retained timestamps, appearance choices, automatic capture, retention, startup/tray behavior, and accessibility. Verify embedded source fidelity and the searchable Settings topic in Release tests; native-speaker review remains outstanding.
- Add independent, persisted app-text, label, button-background, and button-text swatch choices with light/dark shades, high-contrast overrides, translated labels in all 20 languages, and contrast regression tests for every supported combination.
- Apply theme/font resources explicitly to windows and theme the dropdowns; keep the Settings color controls within the visible viewport and verify the live Settings/report/tray workflow.
- Replace calendar entry with exact retained-snapshot timestamp selectors showing local date, milliseconds, and UTC offset across all stored scopes. Preserve the selected reference during automatic refresh.
- Run retention independently of automatic capture, protect all baselines and named checkpoints, support cancellation of scheduled captures, reject mismatched capture scope/access, and fix startup/tray preference restoration and failed-registration rollback.
- Replace the button-shaped mode selector with recognizable Simple/Advanced radio controls and clarify their actual differences. Date comparisons remain available in both modes.
- Add before/after local-date filters, exact saved-snapshot selectors, and a Today target for fresh checks. Preserve the chosen reference and saved baseline; saved-to-saved comparisons never capture. Reject missing, reversed, identical, and incompatible selections.
- Increase text/control contrast and readable text sizes, add explicit change-kind/value summaries, fix numeric-only group headers and their accessible names, strengthen focus outlines, and honor live high-contrast changes.
- Add the complete embedded Helpme.md offline guide, Help me/F1 entry points, topic search, formatted native documents, and adjustable text with compact table layouts.
- Ask for current-user or machine-wide scope before the first check. Scope and Simple/Advanced preferences never imply elevation or start collection.
- Keep normal checks unelevated in both scopes. A separate machine-wide administrator check requires explicit confirmation and Windows UAC authorization for that check only.
- Separate registry and network collectors by scope; machine-wide means shared system configuration, not all private user profiles.
- Keep the main window unelevated and use a bounded, read-only helper with local peer-verified IPC, fixed collector requests, and no privileged database/output paths.
- Persist scope/access in snapshots and reports, maintain separate baselines, preserve old snapshots as legacy mixed scope, and mark history version 2 so old builds cannot misinterpret it.
- Declare optional MSIX `allowElevation`; Store capability approval and real UAC/cross-account/installed-package validation remain outstanding. Automated tests do not trigger elevation.
- Make **Current user** and **Machine-wide** independent checkboxes instead of an either/or choice; both start selected for a new profile, and at least one is required. Combined checks read each scope separately, merge them into one snapshot, and keep their own baseline; only the machine portion can request administrator access.
- Add an in-app **Language** selector with 20 bundled offline languages (English, Mandarin Chinese, Hindi, Spanish, Standard Arabic, French, Bengali, Portuguese, Indonesian, Urdu, Russian, German, Japanese, Nigerian Pidgin, Egyptian Arabic, Marathi, Vietnamese, Telugu, Hausa, Turkish). Switching language updates the interface, open Help window, and text reports immediately; raw names, paths, identifiers, and JSON/CSV field names are never translated. Arabic, Egyptian Arabic, and Urdu render right-to-left. These are preview translations pending native-speaker review.
- Translate the offline Help guide into all 20 languages (`help/{code}.md`, embedded alongside the English `Helpme.md`), each covering the same 18 topics including Settings, language selection, independent scopes, and full Advanced evidence.
- Expand Advanced change details to show every captured field (not just a 3-field summary), full observation metadata (snapshot IDs, scope/access, exact UTC times, versions, counts), and hidden-value change detection without exposing fingerprints or keys.
- Fix a privacy defect where each change card's accessibility name silently fell back to the raw internal object text (including the protected comparison fingerprint) because the review/routine/other lists had no explicit accessible name for their item containers. Each card now exposes a safe, sanitized accessible name instead.
- Verify (with an automated end-to-end test) that choosing a language immediately relocalizes the interface, persists across a full app restart, and never captures data, changes scope, or moves the baseline/reference by itself.
- Reorganize **Settings** into grouped sections (Language, Appearance, Automatic checks, Startup and tray, Local history) and move the **Language** selector there from the sidebar; behavior of every relocated control is unchanged.
- Add a **Light/Dark** appearance theme, a font-family choice, and a button text color choice under Settings > Appearance. Selections apply immediately and persist; Windows high-contrast mode always overrides a custom button color.
- Add configurable **snapshot frequency** (Off, 15 minutes to weekly) that captures a standard-access snapshot for the current scope while the app is running, and **data retention** (30/90/180/365 days, or forever) that deletes only unlabeled, non-baseline snapshots older than the cutoff. Defaults are now every 4 hours and 30 days, without overriding saved choices. Neither collects with administrator access, runs while the app is closed, or overrides an explicit checkpoint/baseline.
- Add an optional, off-by-default **Start ChangeTracker when I sign in** preference that registers only this app's own per-user, unelevated `Run` entry (never a service, scheduled task, or another application's entry). Tray operation now always uses Open/Exit commands and ignores the obsolete optional-tray preference.
- These preferences implement part of `productspec.md`'s "M: Monitoring" controls in source. The requested scheduling and retention defaults do not establish compliance with that section's measurable resource budget and idle/active/OS/GPU profiling gate; that profiling has not been performed.
- Restrict the before/after comparison date pickers to calendar dates that still have a retained snapshot. A date without one is no longer selectable, replacing the previous trial-and-error "no snapshot for that date" result.
- Expose a heading level on every page title, section heading, and finding name so screen readers such as Narrator can jump directly between them instead of reading the window top to bottom; add automated regression coverage for the four custom button text colors against both theme surfaces (all combinations measure at least 4.5:1, most above 7:1).
- Flatten `logos/` to nine files (44, 71, 150, 300, 512, 1024, 1080x1080 box art, 720x1080 poster, 1920x1080 wide), removing the `store-listing/` subfolder and every Dark/2160/3840/1440 alternate. Every file now shows the ChangeTracker wordmark, including the six square icon sizes that previously carried the mark only; the 1920x1080 file is accordingly a general-purpose wide banner rather than a text-free Partner Center "Super Hero" upload. The MSIX manifest, packaging script, and the app's embedded tray-icon resource were repointed to the new filenames; no manifest-referenced size was dropped.
- Fix a keyboard-trap gap in the finding detail overlay: the rest of the window was only visually dimmed behind it, not disabled, so Tab could reach and activate hidden background controls. Opening it now also disables the background (matching the existing scope-selection dialog), moves focus into the overlay, closes on Escape, and cycles Tab within it.

These changes are in source only. Existing 0.1.0 installers and their checksums have not been replaced. No Windows service is added; the optional scheduler and sign-in entry only ever run this same unelevated app, never a separate process. No signing/publication is performed, and the fixed $0.99 price is unchanged.

## 0.1.0 - 2026-09-18 (Preview)

First local preview of **ChangeTracker** by **Bulent Ozkir**. This is an incremental development release, not the completed product roadmap or a Store-certified release.

### Included

- Native Windows interface with Simple and Advanced modes in one app.
- Manual read-only checks for 11 scoped sources: registered desktop apps, Run/RunOnce startup entries, services, scheduled tasks, Windows Update history, driver metadata, default apps, audio defaults, firewall profiles, optional network configuration, and optional PATH.
- Local SQLite snapshots, a stable baseline, named checkpoints, and before/after comparisons.
- Review-first findings, routine/unassessed groups, visible coverage gaps, and reversible occurrence-specific expected marks.
- Sanitized text reports; Advanced also offers JSON and CSV with report preview and safe save behavior.
- Isolated collector workers with per-source timeout and cancellation. No monitored Windows settings are changed.
- Partner Center identity `BulentOzkir.ChangeTracker`, publisher **Bulent Ozkir**, Store ID `9PGK3NF5MK42`.

### Release Files

- `ChangeTracker-0.1.0-x64.msixbundle`: unsigned bundle containing the x64 application package with the supplied Partner Center identity. It is not a trusted sideload package and has not been submitted or certified.
- `ChangeTracker-0.1.0-x64.msi`: unsigned, self-contained x64 Windows Installer package. Installs to `%ProgramFiles%\ChangeTracker` for all users and adds a Start menu shortcut. Installation requires administrator approval; the application runs without elevation. The .NET runtime is included, and the internal executable name remains `PCChangeTracker.exe`.
- `SHA256SUMS.txt`: integrity checksums for both installers and the external release notes. Checksums are not a digital signature or publisher-trust guarantee.

Use one installation format, not both on the same computer. No portable ZIP is included. MSI uninstall removes the installed application, not per-user snapshot history. Close ChangeTracker before installing or upgrading.

The product targets Windows 10 22H2 and Windows 11 x64. Current runtime/UI verification was performed on Windows 11 x64; Windows 10, ARM64, and installed MSI/MSIX lifecycle qualification remain outstanding. WiX ICE validation was blocked by this build machine's policy and must be completed on a suitable validation machine. This build is not Authenticode-signed and may be blocked by device policy. Do not disable SmartScreen or organizational protections to run it; use the approved signing/deployment process for managed devices.

### First Use And Privacy

Review **Sources**, then select **Check now** to create a baseline. No capture starts automatically on launch. A later check reveals differences; the first snapshot cannot reconstruct earlier configuration. Network and PATH collection are optional.

History remains under `%LOCALAPPDATA%\PCChangeTracker`. The public rename does not move that directory. Launch values and hidden proxy URL fields are compared using a DPAPI-protected key rather than stored verbatim. The database itself is not encrypted. Reports redact profile paths and selected sensitive fields, but may still contain identifying names; review them before sharing.

### Known Limits

- Manual collection only: no monitoring service, tray residency, startup task, schedule, or notifications. Background performance requirements remain unresolved.
- This preview implements scoped subsets of 11 sources, not all 20 planned collector categories. Protected or incomplete sources are reported as unavailable for comparison.
- No full event timeline, configurable retention, ignore-rule engine, report import, HTML/PDF export, encrypted support bundles, or completed localization.
- No automatic repair, malware verdict, causal guarantee, account requirement, ads, or telemetry.
- Microsoft Store signing/submission, installed-package testing, full accessibility qualification, and the remaining roadmap are not complete.

The planned Store price is fixed at **$0.99 one-time**, including both modes and all features. This preview has no purchase flow. No GitHub release, Git tag, certificate installation, or public upload is performed by the local release build.