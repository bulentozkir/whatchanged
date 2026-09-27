# ChangeTracker Help

An offline guide to the current development build. ChangeTracker saves read-only observations of selected Windows configuration and compares them. It does not repair Windows, remove software, or decide whether a computer is safe.

## Language

Choose **Language** in **Settings**. The app remembers your choice and updates the interface, open help, text reports, dates, and explanatory labels without a restart or a new capture. The twenty languages are English, Mandarin Chinese (Simplified), Hindi, Spanish, Standard Arabic, French, Bengali, Portuguese (Brazilian formatting), Indonesian, Urdu, Russian, German, Japanese, Nigerian Pidgin, Egyptian Arabic, Marathi, Vietnamese, Telugu, Hausa, and Turkish. The selection uses total-speaker rankings rather than country population; rankings and regional classifications vary.

Arabic, Egyptian Arabic, and Urdu use right-to-left content. The selector stays on the left. All translations and guides are bundled offline. Application names, user-written checkpoint names, paths, identifiers, and original collected values are not rewritten. JSON/CSV keep stable English schema fields and original evidence. Windows UAC and operating-system dialogs follow Windows' own language. Translations need native-speaker review before production release.

## Settings

Open **Settings** in the navigation menu. Preferences are saved for the current history directory and restored when you reopen the app. Sections appear as cards in up to three columns, read top to bottom and then across, so the page usually fits one screen without scrolling. Scope and Comparison controls are hidden on Settings; they remain on the other pages.

- **Language** changes the interface and offline help immediately.
- **Appearance** offers Dark/Light (Dark for new profiles; a saved choice is kept), a font family, **Text size** (100%, 125%, 150%, or 200%), and independent app-text, label, button-background, and button-text colors. Text size applies immediately to the main pages, controls, Help, and Report; the choice is saved. Named swatches offer Default, Navy, Forest green, Maroon, and Purple. Default restores that role's theme color. With Default button colors, buttons follow a clear hierarchy: the main check is blue, replacing the baseline is amber, deletions are red, and every other command is a neutral button with a color-coded icon (blue for reports and viewing, violet for scope and navigation, gold for Help). The default palette is designed for low vision and color-vision deficiency: interactive elements are blue (the hue best preserved by common forms of color blindness), field labels have their own navy (Light) or light-blue (Dark) color so they are never mistaken for values, and every status also has a word and a symbol. A custom button background or text color applies to every button instead. Shades adapt to Dark/Light; Windows high contrast overrides them. Primary actions retain contrasting text.
- **Snapshot frequency** defaults to every 4 hours when no valid preference is saved. Existing choices, including Off, are preserved. Choose Off, every 15 minutes, hour, 4 hours, 6 hours, day, or week. Checks run only while the app is open, including in the tray, use standard access, and can be canceled. After scope confirmation, a first or overdue check can run at the next minute check; subsequent checks follow the chosen interval. The app does not wake a sleeping PC or replay missed intervals.
- **Data retention** defaults to 30 days when no valid preference is saved. Existing choices, including Forever, are preserved. Choose 30, 90, 180, or 365 days, or Forever. Cleanup deletes only older unnamed, non-baseline snapshots, runs when first due and then daily while open, even when automatic snapshots are Off, and after successful automatic checks. Named checkpoints and every scope/access baseline are protected. Deleted timestamps leave the selectors.
- **Start ChangeTracker when I sign in** is an optional switch, off by default. It includes sign-in after a reboot, not collection before sign-in. Only this app's per-user startup entry is changed; no service or boot task is installed, and no other app or policy is changed. A failed registration restores the previous choice.
- **Tray operation** is always enabled. Minimize, Close, and Alt+F4 hide the main window while checks continue. **Open ChangeTracker** or double-clicking its tray icon restores it as you left it; starting ChangeTracker again (for example from the Start menu) opens it maximized. Use the tray's **Exit** command to cancel active work and quit. Turning off sign-in startup does not change this behavior.

Both **Current user** and **Machine-wide** start selected for new profiles; saved scope choices are preserved. All retained scopes remain browsable from the same history, but comparison endpoints must have matching scope and access. No preference grants administrator access. Resource profiling and installed-package lifecycle qualification remain outstanding.

## Getting Started

1. Open ChangeTracker normally, without **Run as administrator**. It starts maximized, including when you start it again while it runs in the tray. You can restore and resize it; **Open ChangeTracker** in the tray keeps the last visible state. Sign-in startup stays hidden in the tray.
2. Review the **Current user** and **Machine-wide** checkboxes, both selected for a new profile. Keep either or both, never neither, then confirm. Your previous choice is remembered. Selecting scopes does not start collection or request administrator access.
3. Open **Sources** and review the selected sources. All supported checks, including Network and PATH, start enabled when no valid choice is saved. Saved choices, including disabled checks, are preserved. Turn off any sources you do not want; unsupported sources stay disabled for the selected scope.
4. In **Comparison**, leave **Today (new check)** selected. If there is no earlier snapshot, the next check saves current state only.
5. Select **Check now** at the right end of the command bar. The app collects the selected sources, saves the observation locally, and reports incomplete coverage where necessary.
6. After a later configuration change, choose your earlier snapshot and check again, or compare two snapshots already saved.

Your first usable snapshot becomes the baseline for that scope and access level. A first snapshot is inventory, not evidence of earlier changes. Subsequent captures do not automatically move the baseline.

Normal capture happens when you select **Check now**, or when an enabled automatic interval is due while the app is running. Both use standard access; ChangeTracker never asks for administrator access. A new profile starts with a four-hour schedule, but no capture occurs before scope confirmation. Choose Off in Settings for manual checks only. Closing hides the window; use tray Exit to stop the app. Changes that happen and disappear between checks can be missed.

## Simple And Advanced

These are **presentation modes**, not separate editions, permissions, or levels of scan depth. Choose either radio option whenever you need it. The app remembers the mode.

| Area | Simple | Advanced |
| --- | --- | --- |
| Findings | Change name, Added/Removed/Modified, priority, explanation, and labeled Before/After rows for changed public fields in larger, selectable text. | All captured fields side by side, including unchanged context, added/removed fields, identifiers, and long values without the summary limit. |
| Before / after | Structured fields with repeated Before/After labels, the observation interval, reason, and uncertainty. | Full fields plus snapshot IDs, scope/access, exact UTC snapshot/source times, collector status/version, schema version, counts, and hidden-value change status. |
| Date comparisons | Two saved snapshots or a saved snapshot versus today, selected by their exact timestamps. | Separate retained-date filters and capture-time lists, with checkpoint and scope/access on separate lines. |
| Sources, scope, and history | Available. | Available; no extra collection is enabled by switching modes. |
| Reports | Preview, copy, and save sanitized text. | Text plus JSON and CSV export. |
| Administrator access | Never requested. | Never requested. Advanced is not administrator mode. |

Switching modes does not run a check, create or delete a snapshot, reset a comparison, move a baseline, enable a source, or grant elevation. Both modes are included in the planned one-time **$0.99** price. This development build has no purchase flow.

## Choosing Scope

**Current user** observes selected configuration associated with the Windows account running the app. **Machine-wide** observes shared system configuration. They are independent checkboxes, both enabled for a fresh profile. Machine-wide does not mean loading all users' private profiles or reading the administrator account's personal settings.

| Source | Current user | Machine-wide |
| --- | --- | --- |
| Installed apps | This user's desktop uninstall registrations. | Machine desktop uninstall registrations in 32-bit and 64-bit registry views. |
| Startup entries | This user's Run and RunOnce entries. | Machine Run and RunOnce entries. |
| Default apps | This user's effective supported file/link handlers. | Not collected. |
| Audio defaults | This user's playback, recording, and communications defaults. | Not collected. |
| Network | This user's proxy settings. | Adapter DNS and DHCP configuration. |
| PATH | Persisted user PATH. | Persisted machine PATH. |
| Services, tasks, updates, drivers, firewall | Not collected in this scope. | Selected readable system sources. |

Use **Change scope**, below the page navigation in the sidebar, to select another scope. An unavailable source is labeled outside scope and cannot be enabled there. Source selections are remembered separately for each scope. Changing scope never starts a capture or an elevation prompt.

With both scopes selected, user and machine observations are read separately and combined into one snapshot. Both use standard access under your own account. If either portion is incomplete, the combined category is incomplete and cannot infer removals. Combined snapshots have their own baselines and cannot be compared to old single-scope snapshots.

History stays local to the initiating user even for machine-wide checks. The app does not provide shared multi-user history or logged-out monitoring.

## Dates And Snapshots

The **Comparison** section is available in both modes before running a check. Its header always states both ends of the current selection, for example **Comparison: Baseline · 9/17/2026 12:00 PM → Today (new check)**, so you can see what will be compared while it is collapsed. It collapses after a successful check or comparison and when you open another page, to make more room for results; expand it whenever you want to change the selection.

- **Earlier snapshot** selects an exact retained observation. In the open list, each choice shows its local time including milliseconds and UTC offset on the first line and its checkpoint and scope/access on the second; Simple also shows the date. The closed selector shows the date, time, and checkpoint on one line, with scope/access beneath it. Advanced adds a **Before date** filter containing only retained days, newest first, and limits the time list to that day. Dates and times cannot be typed or invented.
- **Baseline** selects the saved standard-access baseline for the currently selected scope. It does not replace that baseline. It sits beside the scope/access line under the selector.
- **Current state only**, the clear-reference button next to **Baseline**, removes the earlier selection and targets a fresh check today without a comparison.
- **Today (new check)** means a new observation captured when you press **Check now**, not a previously saved snapshot from earlier today.
- **Saved snapshot** shows the **Later snapshot** selector for an exact retained observation. Advanced also shows the **After date** filter. The main action becomes **Compare snapshots**, which reads saved history without collecting current configuration.

Displayed dates and times are local. Each snapshot also records its observation timestamps; a capture is not an instantaneous whole-PC transaction. Choose two different, chronological, non-overlapping snapshots with matching scope and access.

A date with no snapshot means **no observation is available** and is not offered. Deleted or expired snapshots disappear from both selectors. History from all collection scopes stays visible regardless of the current capture scope, but a comparison requires compatible scope and access at both endpoints.

## Compare Two Saved Snapshots

1. Expand **Comparison** if necessary.
2. In Advanced, choose **Before date**, then select the exact **Earlier snapshot** time. In Simple, select its full timestamp directly.
3. Choose **Saved snapshot** on the After side.
4. In Advanced, choose **After date**, then select the exact **Later snapshot** time. In Simple, select its full timestamp directly.
5. Select **Compare snapshots**.

You can compare snapshots from the same day by choosing different capture times. Comparisons do not modify the saved baseline or original observations. The review and report show the selected interval.

If the snapshots are the same, reversed, overlapping, or have different scopes/access levels, the app asks you to correct the selection. An administrator snapshot cannot be treated as equivalent to a standard-access snapshot merely because both read some of the same sources.

Saved comparisons do not need an administrator prompt, including comparisons of previously saved administrator observations. They do not run a collector.

## Compare A Snapshot With Today

1. Choose the before date and exact earlier snapshot.
2. Select **Today (new check)**.
3. Ensure the current collection scope matches the earlier snapshot. Review **Sources** as needed.
4. Select **Check now** for a standard-access observation.

The fresh snapshot is saved and compared against the selected earlier snapshot, even when that snapshot is not the baseline. The selected reference is not replaced by a baseline behind the scenes.

If the earlier snapshot used administrator access (saved by an earlier version), it cannot be the reference for a new check, because checks always use standard access. Choose a standard-access snapshot, or use **Current state only** to make a new observation without a compatible earlier reference. Two saved snapshots can still be compared.

**Cancel** stops an in-progress manual or automatic check and leaves previously saved history and baselines unchanged. Closing during collection hides the window and lets collection continue in the tray. The tray's **Exit** command cancels active collection and exits after the worker stops. Changing selectors does not run a check.

## Administrator Access

ChangeTracker never asks for administrator access. Every check, manual or automatic, runs with your standard Windows permissions in both scopes, so Windows never shows a UAC prompt for a check. There is no administrator mode, elevated helper, or background service.

Many machine-wide settings are readable with standard permissions. When a source contains something those permissions cannot read, the check reports that source as incomplete instead of elevating. Incomplete sources are listed in the coverage details and never suggest removals.

Starting ChangeTracker with **Run as administrator** is not supported: the app shows a message and closes. Open it normally from the Start menu.

Snapshots saved with administrator access by an earlier version stay in history. You can view them, compare two of them, and include them in reports, but they cannot be the earlier snapshot for a new check, because new checks always use standard access. Choose a standard-access snapshot, or select **Current state only**.

Installing the MSI requires administrator approval from Windows; that approval is part of installation, not of ChangeTracker's checks. The Microsoft Store package installs without it. Never share an administrator password through an app report, help search, chat, or log.

## Reading Changes

**Added**, **Removed**, and **Modified** describe differences between the selected observations, not a precise timestamp or a confirmed cause.

Each finding is a compact card: the change kind in an outlined tag and the category on the first line, the priority as a word with its own symbol at the end of that line (a warning triangle for Important, a flag for Review, an information mark for Info, and a check mark for Expected by you), then the item name, the reason, the values, and the actions.

| Finding | Meaning |
| --- | --- |
| Review first | A rule found a change worth checking, such as a launch registration or a protection setting. It is not a malware verdict. |
| Important | A higher-impact rule matched, such as a firewall setting changing from on to off. Intent and cause remain unknown. |
| Review | A change deserves attention, without a claim that it is harmful. |
| Info / Routine | Lower-priority or version-only activity where the required comparison evidence was available. This is not a safety guarantee. |
| Expected by you | You marked this particular observed occurrence expected. The underlying evidence remains. |
| Other changes | A change was observed but its impact was not assessed by a supported rule. |

The heading counts review-priority findings in the current filter, not every difference. Routine/expected and Other groups have separate labeled counts. Initially, up to three review findings are shown; **View all review findings** exposes the remainder. The **Category** and **Search changes** filters sit on one line above the findings with their labels beside them; they narrow the visible list, and **Clear filters** restores it.

In Simple, changed public fields appear directly in each finding with **Before** and **After** labels beside larger, selectable values. Missing values say **Not present** and empty values are labeled separately. **Before / after** opens structured details, interval, reason, and uncertainty; its header and actions stay in place while the details scroll. Hidden launch values, protected fingerprints, and some identifiers are not displayed or exported. A hidden-value change can be detected without exposing that value.

In **Advanced**, **All captured fields** is a Field / Before / After table that includes source/record/endpoint identifiers where captured. **Observation metadata (UTC)** uses the same table for exact recorded times and versions. Missing and empty values are distinct; unchanged context remains visible. Long values are selectable and not truncated to the summary limit. Copy details uses this fuller view in Advanced. Device identifiers can identify you; review before sharing. No view can restore sensitive contents that were never stored, and fingerprints/keys stay hidden. Reports retain their stricter privacy projection.

**Open settings** opens an allowlisted Windows settings or management destination. It does not repair the change. Any changes you then make in Windows are your separate actions.

**Mark expected** annotates one comparison occurrence; **Undo expected** reverses it. It does not approve a publisher forever, disable collection, hide coverage failures, or change Windows configuration.

## Coverage And Uncertainty

Read the coverage summary before interpreting the findings.

**Coverage details** contains the saved baseline label and unchanged-area list. The current coverage count and any comparison-gap heading remain visible when those details are collapsed.

| State | Meaning |
| --- | --- |
| Success | The collector completed its implemented, selected scope. It does not mean every possible setting in that category was inspected. |
| Partial | Some data was read, but some entries were unavailable or a safety limit was reached. The category is not treated as a complete comparison. |
| Failed | The source did not produce a usable reading, or timed out. |
| Disabled / outside scope | The source was not selected or does not belong to this collection scope. |
| Comparison coverage gap | The two endpoints cannot be reliably compared for that source, even if the current read succeeded. |

**All selected sources checked completely** describes the latest capture. It can coexist with **Comparison coverage gaps** when the earlier snapshot lacks a source, used another collector format, or has a different comparison key. Expand the gap list for the reason.

Incomplete, missing, disabled, duplicate-identity, incompatible, or cross-scope readings must not produce inferred mass removals or an all-clear. A new source needs compatible observations at both endpoints. Changing source selections can therefore introduce comparison gaps.

The app observes that a value differs between two readings. It generally cannot establish the exact change time, who made it, whether it caused a problem, or whether a registration actually ran. A program update and a service change close together may be related, but timing alone does not prove causation.

**Current inventory** is the current observation, not a historical event timeline. The UI limits inventory display to 1,000 records per source; complete captured records remain in the snapshot and eligible exports.

## Baselines And Saved History

Open **Snapshots** to see capture times, scope/access, checkpoints, record counts, and complete-source counts. The commands sit above the list, and the list fills the rest of the window and scrolls on its own, so the commands stay in view however long the history grows.

- **View selected** displays a saved observation and its compatible baseline comparison when available.
- **Name checkpoint** assigns a local label of 1 to 120 characters, typed in the labeled **Checkpoint name** field beside it.
- **Use as baseline** replaces that scope/access baseline only after confirmation. The original capture time is retained. It is amber because it replaces a saved reference.
- **Delete selected** deletes a saved snapshot after confirmation. It is red and placed last, apart from the other commands. Choose another baseline before deleting a snapshot that is currently a baseline.

There are separate baselines for user-only, machine-only, both scopes, their applicable access levels, and legacy mixed-scope observations. A switch between these contexts cannot be interpreted as added or removed settings.

Ordinary captures and custom comparisons do not move a baseline. There is no fixed checkpoint limit. **Settings > Data retention** defaults to 30 days; saved preferences, including Forever, are preserved. Time windows remove only unnamed, non-baseline snapshots older than the selected age. Cleanup is checked once a minute, runs when first due and then daily while the app is open, and also runs after successful automatic captures. It works even with automatic snapshots Off. Named checkpoints and every scope/access baseline are protected. A capture with no usable sources is not saved as a baseline.

**Clear local history**, under Settings, requires confirmation and deletes app-owned snapshots and expected marks. It retains source/mode preferences and the protected comparison key. It does not delete exported reports or modify Windows settings. This is not forensic erasure.

## Collection Sources And Limits

The app currently implements scoped subsets of 11 categories. It is not the complete 20-category roadmap.

All supported sources start enabled when no valid preference is saved. This includes Network and PATH in both user and machine scopes. An existing disabled choice remains disabled after an update; open **Sources** to change it. Changing a source does not run a check or elevate permissions. A newly enabled source needs usable observations at both endpoints before it can show changes; older snapshots are not backfilled.

On **Sources**, each source is a compact card in a grid of up to three columns: its checkbox and name, its status as a word with a symbol (a check mark for Success, a warning triangle for Partial, an error mark for Failed, and an empty circle when it has not been checked, is disabled, or is outside scope), what it reads in the selected scope, its record count, and any diagnostic.

| Source | What is read | Important limits |
| --- | --- | --- |
| Installed apps | Selected-hive desktop uninstall names, versions, and publishers. | No Store-package or portable-app inventory; a changed registration identity may appear as removal/addition. |
| Startup entries | Selected-hive Run and RunOnce registrations. | No Startup folder inventory; registered does not mean enabled or executed. Launch contents are privately compared. |
| Services | Registered Win32 service startup configuration and protected launch fingerprints. | No runtime polling, driver-service inventory, or full account/dependency analysis. |
| Scheduled tasks | Accessible task names, enabled state, trigger summaries, and protected configuration fingerprints. | No task execution or stored raw XML/commands; inaccessible entries cause partial coverage. |
| Windows updates | Successful local Windows Update install/uninstall history. | No online scan or update installation. A 5,000-event safety cap produces Partial when exceeded; lost history entries are not inferred uninstalls. |
| Drivers | Locally exposed signed-device driver identifiers, versions, providers, and INF names. | No firmware probing, driver rollback, or inferred installation dates. |
| Default apps | Effective HTTP/HTTPS, PDF, JPG, PNG, MP3, MP4, ZIP, and CSV handlers. | No protected association writes. |
| Audio defaults | Default playback, recording, and communications endpoints. | No sound recording/playback or per-app override detection. |
| Protection | Windows Firewall profile enabled state. | Not an antivirus assessment or security verdict. |
| Network | User proxy or shared adapter DNS/DHCP, depending on scope. | Enabled by default; no traffic capture, Wi-Fi passwords, network probing, or full VPN inventory. |
| PATH | Persisted user or machine PATH, including order, duplicates, and unexpanded references. | Enabled by default; no arbitrary environment variables, executable discovery, or directory scanning. |

Collectors run sequentially in short-lived processes with a 25-second per-source bound. A slow or inaccessible source is shown as incomplete instead of blocking indefinitely. Read-only means monitored Windows configuration is not changed; the app still writes its own history, preferences, and explicitly requested exports.

## Reports And Sharing

Select **Report** after displaying the observation or comparison you want to share. The report uses that displayed result, not an unexecuted date selection.

1. Review the sanitized preview.
2. Use **Copy text** or **Save text** in either mode.
3. In Advanced, **Save JSON** and **Save CSV** provide the invariant structured report. The interface and text preview follow your language; JSON/CSV retain stable English field names and original collected values.
4. Choose the destination yourself. Nothing is uploaded automatically.

Reports include the selected observation interval, scope/access, findings, coverage limitations, and valid unchanged areas. A current-state report includes inventory; a comparison report describes differences between its endpoints. It is not a full persisted event timeline.

Profile paths and common credential patterns are redacted. Hidden launch values, fingerprints, comparison keys, checkpoint labels/notes, and audio endpoint IDs are omitted. CSV output quotes values and neutralizes formula-like cells. These protections do not guarantee anonymity: app names, folder names, network metadata, and organizational details may remain. Inspect before sharing.

HTML/PDF export, report import, and encrypted support bundles are not implemented. An export remains wherever you saved it even after local history is cleared.

## Privacy And Storage

The normal unpackaged/MSI app stores history under `%LOCALAPPDATA%\PCChangeTracker`. The Settings page shows the actual data directory. Development runs can use a separate `--data-dir` location. Public branding is ChangeTracker, but the internal data/executable name remains PCChangeTracker.

### Manage Disk Usage

1. Find **Snapshot storage** above **Help me** in the left sidebar. It stays visible on every page and includes retained history from all scopes, not just the current capture scope.
2. Hover over the size label for guidance. Keyboard users can focus the label with Tab; screen readers can read its name and help text.
3. Open **Settings > Automatic checks**. A less frequent capture interval creates fewer future snapshots; Off stops automatic captures, not retention cleanup. It does not delete existing history.
4. Choose a shorter **Data retention** period to remove eligible older observations at the next due cleanup. Cleanup runs when first due and then daily while the app runs, even with automatic captures Off, and after successful automatic captures. Changing the selector does not force immediate cleanup.

Baselines for every scope/access and named checkpoints are protected, so retention is not a hard disk-space limit. Saved choices are preserved; without a valid saved preference the defaults are every 4 hours and 30 days. You can still request manual checks.

### Understand The Size

The label counts `history.db`, `history.db-wal`, and `history.db-shm` when present in the current history directory. This is the combined file size, not just snapshot payloads or Windows' allocation-unit-rounded "Size on disk". It includes preferences, database overhead, and reusable free space. It does not include exported reports, the app installation, or the separate protected key file.

Units use powers of 1,024: B, KiB, MiB, GiB, or TiB; the number follows your selected language's formatting. The display refreshes with history after capture, deletion, clearing, or retention cleanup. Temporary SQLite files can appear and disappear, so it is not a continuously sampled disk monitor. **Size unavailable** means the files could not be measured, not that they use zero space.

Deleting snapshots may leave reusable space inside SQLite instead of shrinking the files immediately. Even an empty history has database overhead. The app does not automatically compact the database; do not delete `history.db` or its temporary files while the app is running to try to reclaim space.

| Local data | Protection and behavior |
| --- | --- |
| Snapshot history and preferences | Local SQLite storage. The database itself is not encrypted. |
| Comparison key | Protected with current-user Windows DPAPI. Sensitive launch/proxy values use keyed fingerprints rather than plaintext storage. |
| Exported reports | Saved only on request. Redacted, but potentially identifying; not encrypted by this app. |

Do not move only the database and assume hidden values will remain comparable under another account or a different key. A missing/incompatible key makes those comparisons unavailable. Back up app-owned history and key material securely, and retain the Windows user protection context; copying a DPAPI file does not make it decryptable elsewhere.

The scoped-history format uses metadata version 2. Older builds refuse that history rather than misinterpreting it. Legacy mixed-scope snapshots remain labeled as legacy. Back up important data before using an unreleased build. Installed-MSIX data redirection/reset/uninstall behavior still needs qualification; do not assume its lifecycle is identical to the MSI build.

The app has no telemetry, account requirement, invisible uploads, ads, automatic repairs, or executable-command replay. It does not read documents, browsing history, passwords, cookies, microphone content, or packet payloads as collection sources.

## Accessibility And Keyboard Use

### Text And Colors

Open **Settings > Appearance > Text size** to choose 100%, 125%, 150%, or 200%. This enlarges interface text, labels, buttons, details, Help, and Report without changing Windows settings. The font-family choice also applies to the Help document. The Help window's own Text size slider adds document-only zoom up to 160% of that base size.

Larger text makes settings columns, source cards, and before/after columns stack when needed, and the sidebar widens so navigation labels wrap between words. The command bar stays at the top of the window; when its commands do not fit beside the Simple/Advanced choice, they move to their own row instead of being squeezed or cut off. Each page opens on its title, followed by the comparison it uses and then its content. The page below it, the sidebar, dialogs, and long field values can scroll; use the scrollbars or keyboard to reach content below the visible area. The **Snapshots** list fills the window and scrolls on its own, and at very large text sizes the whole page scrolls instead. Selecting a page brings its heading into view. Collapsing Comparison provides more room for findings. Windows display scaling can enlarge the entire interface further.

Dark (the default) and Light themes, named color swatches, and Windows high-contrast overrides are supported. The default palette is built for low vision and color-vision deficiency: all text, including secondary text and field labels, targets at least 7:1 contrast on every surface; control boundaries, the selection bar, and focus accents at least 3:1; interactive color is blue, success uses a bluish green that stays distinct from red under red-green color blindness, and field labels use their own navy (Light) or light-blue (Dark) color. Available custom presets are contrast-tested. Buttons use semantic roles rather than decoration: the blue primary button and the amber caution and red danger buttons have at least 7:1 text and a 3:1 outline, and neutral buttons carry a color-coded icon with at least 4.5:1 contrast. Disabled buttons lose their fill and show a dashed outline, so the state is visible without relying on color. Dropdowns and text fields have a 3:1 outline, an accent arrow, and an accent outline while focused or open; check boxes, switches, and radio buttons fill with the accent color when on. Every keyboard focus shows a solid 2-pixel ring. Change kind, priority, and source status use words with distinct symbols, radio choices show selected circles, check boxes show check marks, switches move their knob, button labels stay visible, and the active page and selected rows show a tint, a selection bar, and bold text, so color is not the only indicator. The window title bar follows the Dark theme where Windows allows it. Buttons, fields, and list rows are at least 36 device-independent pixels tall, and check boxes, switches, and radio buttons at least 32, above the WCAG 2.2 AA minimum target size of 24 pixels; fonts can make them taller.

### Keyboard Navigation

| Key | Action |
| --- | --- |
| Tab / Shift+Tab | Move forward/backward through controls. Focused fields can scroll into view. |
| Arrow keys in Page navigation | Select Review changes, Snapshots, Sources, or Settings. |
| F6 / Shift+F6 | Move forward/backward between page navigation, the main command bar, and the current page heading. Tab continues into that page. |
| Ctrl+1 / Ctrl+2 / Ctrl+3 / Ctrl+4 | Open Review changes / Snapshots / Sources / Settings and focus the page heading. |
| Arrow keys / Space / Enter | Use native list, radio, checkbox, and button behavior. Alt+Down opens a focused selection list. |
| Escape in change details | Close the detail panel and return focus to the original action when it is still available. |
| F1 | Open offline Help. |
| Ctrl+F in Help | Focus topic search. |
| F6 / Shift+F6 in Help | Move between search, the topic list, and the document. |
| Escape in Help or Report | Close that window. |

Page shortcuts do not change pages behind an open scope chooser or change-detail panel. All sidebar actions are disabled behind those panels. The scope chooser focuses its first checkbox; confirmation returns focus to Change scope. Tab stays within the modal panel. Its own Help button remains available. Closing Help returns focus to the originating control when it is still available. Report initially focuses its read-only preview.

Closing the main window, including Alt+F4, hides it in the tray rather than quitting. Use the notification area's ChangeTracker icon and its **Exit** menu command to stop the app.

### Screen-Reader Information

Page titles, section headings, and finding names expose heading levels. Page navigation announces its selected item, and the command bar is announced as the **Main commands** toolbar. Snapshot rows announce their timestamp, checkpoint, and scope/access instead of internal object text, and the snapshot list's column headers have names. Source checkboxes have names, scope/coverage help, and status. Finding groups, captured fields, and Help topics have readable names. Each selectable before/after value identifies its field and side. The checkpoint-name input has a persistent visible label. Decorative icons are hidden from screen readers, so only the visible label beside each icon is announced.

The main status and comparison-selection messages, and the Help result count, expose polite live updates. The sidebar storage label exposes the same guidance as its mouse-hover tooltip. Icons used as commands have names; raw fingerprints and comparison keys remain hidden from both visible text and accessible item names.

These are implemented accessibility supports, not a certification. Automated checks cover names, selected states, palettes, modal boundaries, and sampled enlarged/RTL layouts; a complete keyboard-only run, manual Narrator/other assistive-technology evaluation, Windows contrast themes, DPI combinations, and testing with users with disabilities are still required before an accessibility-conformance claim. Automated desktop tests require an unlocked, undisturbed session.


## Troubleshooting

| Symptom | What to check |
| --- | --- |
| Nothing happens until scope is selected | Confirm at least one of Current user and Machine-wide. Both start selected. Launching alone does not capture data. |
| A date shows no snapshots | There was no saved observation that day. Choose another date; the app cannot reconstruct an earlier state. |
| Compare is rejected | Choose distinct, chronological snapshots with the same scope and access. Use separate times for two observations on one day. |
| Today cannot match an administrator snapshot | Snapshots saved with administrator access by an earlier version can only be compared with each other. Choose a standard-access reference or **Current state only**. |
| The current capture succeeded but coverage has gaps | Inspect the earlier snapshot's coverage, format, and key compatibility. Current success does not establish comparison coverage. |
| A source is outside scope | It belongs to the other scope. Changing scope is separate from granting administrator rights. |
| A machine-wide source is incomplete | Some settings need administrator rights. ChangeTracker reports them as incomplete instead of asking for elevation; other sources are still compared. |
| A source times out | It is recorded as incomplete. Try a later manual check, with a smaller selected source set if needed. |
| No baseline exists | Complete at least one usable check for that scope/access. All-failed captures are not saved. |
| Reports show older results after changing dates | Execute the comparison/check first. Reports describe the currently displayed observation, not pending selections. |
| Network or PATH is still unchecked after an update | Saved disabled choices are preserved. Enable the supported source in Sources; it needs usable observations at both ends of a comparison. |
| Storage size stays large after retention or deletion | Cleanup is scheduled, not immediate on selection. Named checkpoints and baselines are protected; SQLite may retain freed space for reuse. See Privacy And Storage. |
| Snapshot storage says Size unavailable | Check access to the history directory shown in Settings. No files or snapshots are deleted by the size display. |
| Closing the window does not quit | Close, Minimize, and Alt+F4 hide the main window. Use Exit in the ChangeTracker tray menu to stop active work and quit. |
| Text or controls are difficult to read | Use Settings > Appearance for text size, font, and colors, or a Windows contrast theme. F6 and Ctrl+1 through Ctrl+4 provide direct navigation without repeated tabbing. |
| Enlarged text puts controls below the viewport | Scroll the page or sidebar. Use F6 for the command bar/current page and Tab to continue; paired controls stack when space is limited. |
| The app says history cannot be opened | Close duplicate instances and check storage access, free space, and version compatibility. Existing data is not silently deleted. |
| A finding seems routine, such as a browser update | Inspect before/after values and coverage. Mark that occurrence expected only when appropriate; timing/publisher identity alone is not proof of safety. |
| JSON or CSV buttons are missing | Select Advanced. Text preview/copy/save remain available in Simple. |

When requesting support, share a previewed sanitized report and the app/Windows version. Do not share the raw database, comparison key, administrator credentials, or private configuration unless an appropriate secure process explicitly requires it.

## Preview And Installation Status

The planned product is one app at **$0.99 one-time**, including both modes and all features. This preview is incomplete and has no purchase flow.

The repository provides local x64 and ARM64 MSI installers and an x64 MSIX bundle, not a portable ZIP. The MSI is a conventional machine-wide installer under Program Files; installation requires administrator approval, but the installed app always runs with standard permissions. The MSIX bundle uses the supplied Partner Center identity. Use one installation format, not both on the same computer.

Current local release artifacts are unsigned previews. They are not a trusted sideload distribution or proof of Microsoft Store publication. Signing, Store certification, Windows 10/ARM64 qualification, and install/upgrade/uninstall qualification remain outstanding. MSI ICE validation was blocked by the build machine's policy. Do not disable SmartScreen or managed-device protection to run a preview.

Source changes marked **Unreleased** may be newer than existing installers. This guide is embedded when the application is built; it describes the source build that includes it. The version at the bottom of the sidebar identifies the build you are running. A release is not automatically rebuilt when source code or help changes.

Still planned: broader collectors, persistent event timeline, richer tags/rules/notes, storage-budget retention and archive controls, report import and encrypted bundles, HTML/PDF export, native-speaker localization qualification, and resource-approved background monitoring. Age-based retention is available in Settings. Collection consumes resources; no literal zero-CPU collection or universally harmless-change promise is made.