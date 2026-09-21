# ChangeTracker Help

This na offline guide for development preview. App dey observe settings; e no repair Windows, change monitored settings or promise say PC safe.

## Language

Choose Language for left menu. App go remember am and change interface, open help, dates and text report without restart or new capture. Twenty languages dey inside; no internet needed. Arabic, Egyptian Arabic and Urdu content dey read right-to-left, menu still dey left.

App names, names you type, paths, IDs and original values no dey translate. JSON/CSV keep stable English fields. Windows/UAC use Windows language. Native speakers still need review translations before release.

## Start here

1. Open app normal, no be as administrator.
2. Current user and Whole computer both dey tick for new profile. Keep one or both, no leave both empty, then confirm.
3. Check Sources. Network and PATH na optional. Selection no start capture.
4. Choose Today and Check now.

First useful observation become reference for that scope and access. Na current inventory, no be past events. App no collect on launch or monitor continuously.

## Simple and Advanced

Simple show summary and text reports. Advanced add every captured field, complete before/after values, observation metadata and JSON/CSV. Both get dates, saved history and source controls.

Changing mode no collect, raise permission, move reference or charge extra. Planned price na US$0.99 once for both modes. Preview no get purchase flow.

## Independent scopes

Current user get your app registrations, Run/RunOnce, defaults, sound, proxy and PATH. Computer get shared registrations, services, tasks, updates, drivers, firewall, DNS/DHCP and machine PATH. App no load other people private profiles.

If both dey selected, app read each part separately and save one combined snapshot. Only computer part fit use admin after explicit request. User part stay with original normal user. App respect old saved choices and sources per scope.

## Dates and observations

Dates filter local calendar day. Pick exact time/checkpoint too because many observations fit dey same day. Reference button select normal reference; e no replace am. Only current state clear earlier selection.

Day without snapshot no get history to reconstruct. App no quietly choose nearest date. Observations must be different, follow time order, no overlap, and get same scope/access.

## Compare saved snapshots

Pick earlier date and snapshot, then Saved snapshot and later date/snapshot. Compare no run collector, ask UAC or save new observation. Reference no change. Different times for same day fit compare.

## Compare with today

Choose earlier snapshot and Today. Check now make fresh observation and compare exactly wetin you choose, no hidden switch to another reference. Panel go fold after success; you fit open am again.

Admin reference no automatically raise permissions. Use separate admin action or only current state. Cancel or close during capture ask workers to stop and keep previous history.

## Administrator access

Many machine settings dey readable with normal access. Protected sources remain visible gaps. Admin action need your click, app confirmation wey default to No, and Windows UAC permission for one check.

Main window stay normal. Temporary read-only helper checks machine part, no permanent service or permission. If you refuse, history/reference no change. No share passwords or disable organisation policy. If UAC don show, answer am for Windows; app Cancel no fit dismiss secure desktop.

## Understand full details

Added, Removed and Modified describe two observations, no be who change am, exact time or cause. Important/Review na priority, no be malware verdict. Routine/expected and unknown-impact changes get separate groups. Heading count follow filter; View all show beyond first three.

Advanced show long values, unchanged context, added/removed fields, record identity and source. Metadata get snapshot IDs, scope/access, exact UTC snapshot/read times, status, versions and counts. Empty value different from absent. Commands wey app never save no fit come back; keys and fingerprints remain hidden. IDs fit identify your device: review before copying.

Expected mark na only this occurrence and you fit undo. Open settings only open approved Windows tool; e no repair anything.

## Coverage and uncertainty

Success mean complete for implemented subset, no be all Windows. Partial mean missing entry or limit; Failed mean no useful reading; Disabled/outside scope mean no reading. Incomplete data no create guessed removals.

Current complete reading fit still no compare if old reading incomplete or format/key change. One incomplete part make combined category partial. Coverage details keep reference and unchanged areas. No difference no mean guaranteed safety or cause.

## Reference and history

Snapshots page let you view, name checkpoint with 1–120 characters, delete or replace reference after confirmation. Choose another reference before deleting current one. User, computer, both, access levels and old mixed records get separate references.

No automatic pruning or fixed checkpoint cap. All-useless capture no save. Clear history remove snapshots/marks after confirmation but keep preferences/key and leave exports/Windows alone. No be forensic erasure.

## Sources and limits

| Source | Limit |
| --- | --- |
| Apps and startup | Uninstall registration and Run/RunOnce only; no Store/portable apps or Startup folder. Registration no prove execution. |
| Services and tasks | Accessible settings; no execution, saved commands/XML or continuous polling. |
| Updates and drivers | Successful local history up to 5,000 events, more become Partial; WMI metadata. No install, firmware probe or rollback. |
| Defaults and sound | Supported associations and default devices; no recording or changes. |
| Protection | Firewall profiles, no antivirus assessment. |
| Network and PATH | Optional proxy or DNS/DHCP and saved PATH; no packets, passwords, probes or other variables. |

Each source get 25-second limit. Screen inventory show up to 1,000 records per source but all captured records remain saved. Read-only still fit write app history and exports wey you ask for, no monitored Windows settings.

## Reports

Report use displayed result, no be date selection wey you never run. Preview/copy/save text for both modes; Advanced add JSON/CSV. Text follows language, structured schema stays fixed. No automatic sending.

Report omit keys, fingerprints, launch values, checkpoint names and audio IDs. Profile paths and common secrets dey redacted; CSV formulas dey neutralised. Identifying names fit remain. No PDF/HTML, import or encrypted bundles yet. Exported files remain after clearing history.

## Privacy and storage

Normal location na `%LOCALAPPDATA%\PCChangeTracker`; Settings show real location. SQLite itself no encrypted. Key uses current-user DPAPI; copying am to another account no guarantee decryption. Back up important data safely before new builds.

History format 2 keep old records as mixed scope and block old readers. Helper get fixed categories and temporary key only, no history path or arbitrary command. Only normal UI save data. MSIX data lifecycle still need separate testing.

## Accessibility

Use Tab/Shift+Tab, arrows and Space. Scopes use independent checkboxes; modes use radio. Change type/priority get text, no be only colour. Visible focus and Windows high-contrast colours dey supported.

F1 open Help, Ctrl+F search, Escape close. Help zoom reach 160%; narrow tables become labelled entries. Full screen-reader and native-language review still pending.

## Troubleshooting

Empty date: choose another observation. Comparison refused: check order, scope/access. Partial source no mean removal. Old report: run new selection first. Database no open: check space, rights and version before deleting anything.

For support share reviewed report and app/Windows versions, no password, raw database or comparison key. If you deny admin, normal check still works.

## Release status

Manual preview get 11 limited categories, no continuous monitor, notifications, retention rules or complete timeline. Collection uses resources; no promise of zero CPU.

Local x64 MSI/MSIX no signed. MSI install needs separate approval; no install both formats together. Real UAC, different admin account, Windows 10/ARM64, install/update/remove and Store `allowElevation` approval still pending. Source change no automatically rebuild old releases. Logos no replace real screenshots or certification.