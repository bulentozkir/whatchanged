# ChangeTracker Help

This na offline guide for development preview. App dey observe settings; e no repair Windows, change monitored settings or promise say PC safe.

## Language

Choose **Settings > Language**. App go remember am and change interface, open help, dates and text report without restart or new capture. Twenty languages dey inside; no internet needed. Arabic, Egyptian Arabic and Urdu content dey read right-to-left, menu still dey left.

App names, names you type, paths, IDs and original values no dey translate. JSON/CSV keep stable English fields. Windows/UAC use Windows language. Native speakers still need review translations before release.

## Settings

Open Settings for menu. Preferences dey save for the history folder wey you dey use and return when you reopen app.

- Appearance get Light/Dark theme, font and separate colours for app text, labels, button background and button text. Named colour samples get Default, Navy, Forest green, Maroon and Purple. Default return that part to theme colour. Windows high contrast get priority; main action buttons keep text wey contrast well. If you never save any choice, Dark na the default. Buttons get clear order: the main check na teal, delete actions na red, and all other commands na neutral with colour-coded icon (like Report, Change wetin to check and Help me). Dropdowns, checkboxes and switches use accent colour for the arrow, tick and focus border. Settings sections show for columns, so the page fit one screen most times without scrolling.
- Automatic snapshots default na every 4 hours; saved choices, including Off, remain. Choose every 15 minutes, 1 hour, 4 hours, 6 hours, day or week, or Off for manual checks only. Checks work only while app dey open, including for tray, with normal access and cancellation. After scope confirmation, first or overdue check fit run for next minute check; later checks follow the interval. E no ask admin, wake sleeping PC or replay all missed intervals.
- Retention default na 30 days; saved choices, including Forever, remain. Choose 30, 90, 180 or 365 days, or Forever. Cleanup removes only old snapshots wey no get name and no be reference. E run when e first due, then daily while app dey open and after successful automatic checks, even if automatic snapshots dey Off. Named checkpoints and all scope/access references dey protected.
- Start when you sign in dey optional and Off by default. E include sign-in after restart, no collection before sign-in. Only this app own per-user startup entry go change; no service or boot task, no other app or policy change. If registration fail, old choice remain.
- Minimize or Close always hide window for tray while checks continue. Open, double-click the icon, or launch app again to restore am. Tray Exit cancel active work and quit. Normal launch open maximized; tray restore keep the last visible window state. Optional sign-in startup start hidden, including after Windows restart; e no run before sign-in or as service.

Simple show changed public fields with Before/After labels and bigger read-only values wey you fit select. Closing details return keyboard focus to the original button if e still dey available. Advanced separate retained dates from exact capture times, with checkpoint and scope/access for their own lines. Changing mode keep the selected comparison pair.

New profile select both scopes; saved choices remain. You fit query every retained scope from the same history, but comparison endpoints must match scope and access. No setting grant administrator rights. Resource profiling and installed-package lifecycle checks still pending.

## Start here

1. Open app normal, no be as administrator.
2. Current user and Whole computer both dey tick for new profile. Keep one or both, no leave both empty, then confirm.
3. Check Sources. All supported checks, including Network and PATH, dey on by default. Saved choices remain; you fit turn sources off. Selection no start capture.
4. Choose Today and Check now.

First useful observation become reference for that scope and access. Na current inventory, no be past events. New profile use 4-hour interval, only after scope confirmation and while app dey run. Choose Off for manual checks only.

## Simple and Advanced

Simple show summary and text reports. Advanced add every captured field, complete before/after values, observation metadata and JSON/CSV. Both get dates, saved history and source controls.

Changing mode no collect, raise permission, move reference or charge extra. Planned price na US$0.99 once for both modes. Preview no get purchase flow.

## Independent scopes

Current user get your app registrations, Run/RunOnce, defaults, sound, proxy and PATH. Computer get shared registrations, services, tasks, updates, drivers, firewall, DNS/DHCP and machine PATH. App no load other people private profiles.

If both dey selected, app read each part separately with normal access under your own account and save one combined snapshot. No scope get administrator rights. App respect old saved choices and sources per scope.

## Dates and observations

Selectors show only snapshots wey still dey saved, with local date, time including milliseconds, UTC offset, checkpoint and scope/access. You no fit type any date you like; deleted snapshots leave the list. Second endpoint fit be saved snapshot or fresh Today check. Reference button select normal reference; e no replace am. Only current state clear earlier selection.

Day without snapshot no get history to reconstruct. App no quietly choose nearest date. Observations must be different, follow time order, no overlap, and get same scope/access.

## Compare saved snapshots

Pick earlier date and snapshot, then Saved snapshot and later date/snapshot. Compare no run collector, ask UAC or save new observation. Reference no change. Different times for same day fit compare.

## Compare with today

Choose earlier snapshot and Today. Check now make fresh observation and compare exactly wetin you choose, no hidden switch to another reference. Panel go fold after success; you fit open am again.

Snapshot wey old version save with administrator access no fit be reference for new check, because checks always use normal access. Choose normal-access snapshot or Only current state. Two saved snapshots still fit compare with each other. Cancel stop capture and keep previous history. Close allow collection continue for tray; tray Exit cancel am and quit app.

## Administrator access

ChangeTracker no ever ask for administrator access. Every check, whether by hand or automatic, dey run with normal Windows permissions for both scopes, so Windows no show UAC prompt for check. No administrator mode, no elevated helper, no background service.

Plenty whole-computer settings dey readable with normal access. If source get wetin normal permissions no fit read, app report that source as incomplete (e dey coverage details and never suggest removal) instead of elevation. Starting ChangeTracker with “Run as administrator” no dey supported: app go show message and close; open am normally.

Snapshots wey old version save with administrator access stay for history: you fit view them, compare them with each other, and include them for reports, but dem no fit be earlier snapshot for new check; choose normal-access snapshot or “Only current state”. MSI install needs administrator approval from Windows (installation only, no be checks); Microsoft Store package installs without am. Never share administrator password.

## Understand full details

Added, Removed and Modified describe two observations, no be who change am, exact time or cause. Important/Review na priority, no be malware verdict. Routine/expected and unknown-impact changes get separate groups. Heading count follow filter; View all show beyond first three.

Advanced show long values, unchanged context, added/removed fields, record identity and source. Metadata get snapshot IDs, scope/access, exact UTC snapshot/read times, status, versions and counts. Empty value different from absent. Commands wey app never save no fit come back; keys and fingerprints remain hidden. IDs fit identify your device: review before copying.

Expected mark na only this occurrence and you fit undo. Open settings only open approved Windows tool; e no repair anything.

## Coverage and uncertainty

Success mean complete for implemented subset, no be all Windows. Partial mean missing entry or limit; Failed mean no useful reading; Disabled/outside scope mean no reading. Incomplete data no create guessed removals.

Current complete reading fit still no compare if old reading incomplete or format/key change. One incomplete part make combined category partial. Coverage details keep reference and unchanged areas. No difference no mean guaranteed safety or cause.

## Reference and history

Snapshots page let you view, name checkpoint with 1–120 characters, delete or replace reference after confirmation. Choose another reference before deleting current one. User, computer, both, access levels and old mixed records get separate references.

No fixed checkpoint cap. Optional retention clean old snapshots wey no get name; named checkpoints and all references dey protected. All-useless capture no save. Clear history remove snapshots/marks after confirmation but keep preferences/key and leave exports/Windows alone. No be forensic erasure.

## Sources and limits

| Source | Limit |
| --- | --- |
| Apps and startup | Uninstall registration and Run/RunOnce only; no Store/portable apps or Startup folder. Registration no prove execution. |
| Services and tasks | Accessible settings; no execution, saved commands/XML or continuous polling. |
| Updates and drivers | Successful local history up to 5,000 events, more become Partial; WMI metadata. No install, firmware probe or rollback. |
| Defaults and sound | Supported associations and default devices; no recording or changes. |
| Protection | Firewall profiles, no antivirus assessment. |
| Network and PATH | On by default: proxy or DNS/DHCP and saved PATH; no packets, passwords, probes or other variables. |

All supported sources dey on by default when valid saved choice no dey. Source wey you switch off go remain off after update; change am for Sources. Switching am on no start capture or raise permissions. Changes need usable observations for both ends; old snapshots no get new data added back to them.

Each source get 25-second limit. Screen inventory show up to 1,000 records per source but all captured records remain saved. Read-only still fit write app history and exports wey you ask for, no monitored Windows settings.

## Reports

Report use displayed result, no be date selection wey you never run. Preview/copy/save text for both modes; Advanced add JSON/CSV. Text follows language, structured schema stays fixed. No automatic sending.

Report omit keys, fingerprints, launch values, checkpoint names and audio IDs. Profile paths and common secrets dey redacted; CSV formulas dey neutralised. Identifying names fit remain. No PDF/HTML, import or encrypted bundles yet. Exported files remain after clearing history.

## Privacy and storage

Normal location na `%LOCALAPPDATA%\PCChangeTracker`; Settings show real location. SQLite itself no encrypted. Key uses current-user DPAPI; copying am to another account no guarantee decryption. Back up important data safely before new builds.

### Manage disk space

1. Check Snapshot storage above Help for the left sidebar. E dey every page and e count all scopes for the current history.
2. Put mouse on the size to see help. Tab fit focus the label too; screen reader fit read the name and help text.
3. Change automatic-check frequency for Settings. Longer interval create fewer future snapshots. Off stop automatic capture, but e no delete history or stop retention cleanup.
4. Shorter retention remove eligible old snapshots for the next due cleanup, no be immediately when you select am. Cleanup run when due, then daily while app dey run and after successful automatic captures. All baselines and named checkpoints stay protected, so retention no be hard disk-space limit.

Size na total of `history.db`, `history.db-wal` and `history.db-shm` when dem dey. E include preferences, database overhead and reusable space, no be only snapshot data or Windows block-rounded Size on disk. Exports, app installation and key file no join. B, KiB, MiB, GiB and TiB use multiples of 1,024 with your language number format.

Size update when history refresh after capture, delete or cleanup; e no monitor disk all the time. Size no dey available no mean zero. Deleting fit leave reusable space without shrinking files; empty history still get overhead. App no compact database automatically. No delete database or temporary files while app dey run.

History format 2 keep old records as mixed scope and block old readers. Only normal-access UI save history. MSIX data lifecycle still need separate testing.

## Accessibility

For Settings > Appearance, Text size get 100%, 125%, 150% and 200%. E save the choice and enlarge pages, controls, Help and Report. Paired fields stack when space small; pages, sidebar and dialogs fit scroll. Font choice apply to Help document too; Help own document zoom still reach 160% of that base size.

Sidebar navigation announce the selected page and use arrow keys. Ctrl+1 open changes, Ctrl+2 snapshots, Ctrl+3 sources and Ctrl+4 settings. F6 and Shift+F6 move between navigation, command bar and page heading; Tab continue inside the page. For Help, F6 move between search, topics and document, while Ctrl+F return to search.

Scope picker focus the first checkbox and return to Change scope after confirmation. Modal panels disable the whole sidebar and page shortcuts; Tab stay inside. Escape close details and return focus to the original action if e still available. Help start for search; Report start for read-only preview. Snapshot rows, topics, groups and fields get readable names; values identify field and Before/After side, sources identify status and scope. Main controls get at least 44 device-independent units of interaction height.

This no be universal accessibility certification. Manual screen-reader, contrast-theme, Windows-scale and disabled-user evaluation still needed. Real keyboard tests need unlocked session wey nobody dey interrupt.

Use Tab/Shift+Tab, arrows and Space. Scopes use independent checkboxes; modes use radio. Change type/priority get text, no be only colour. Visible focus and Windows high-contrast colours dey supported.

F1 open Help, Ctrl+F search, Escape close. Help zoom reach 160%; narrow tables become labelled entries. Full screen-reader and native-language review still pending.

Headings get levels for screen-reader navigation. Opening details move focus inside; Tab stay inside the panel and Escape close am. Settings get font and theme-aware colours; Windows high contrast get priority.

## Troubleshooting

Empty date: choose another observation. Comparison refused: check order, scope/access. Partial source no mean removal. Old report: run new selection first. Database no open: check space, rights and version before deleting anything.

Some whole-computer settings need administrator rights; ChangeTracker report them as incomplete instead of asking for elevation, and other sources still compare. For support share reviewed report and app/Windows versions, no password, raw database or comparison key.

## Release status

Preview get 11 limited categories, manual or optional scheduled checks and retention rules. No continuous event monitor, notifications or complete timeline. Collection uses resources; no promise of zero CPU.

Local release get x64 and ARM64 MSI installers plus x64 MSIX bundle, all unsigned. MSI installation needs administrator approval, but installed app always runs with normal permissions. Signing, Store certification, Windows 10/ARM64 qualification, and install/upgrade/uninstall qualification still pending. Source change no automatically rebuild old releases. Logos no replace real screenshots or certification.