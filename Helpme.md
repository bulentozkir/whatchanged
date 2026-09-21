# ChangeTracker Help

An offline guide to the current development build. ChangeTracker saves read-only observations of selected Windows configuration and compares them. It does not repair Windows, remove software, or decide whether a computer is safe.

## Language

Choose **Language** in **Settings**. The app remembers your choice and updates the interface, open help, text reports, dates, and explanatory labels without a restart or a new capture. The twenty languages are English, Mandarin Chinese (Simplified), Hindi, Spanish, Standard Arabic, French, Bengali, Portuguese (Brazilian formatting), Indonesian, Urdu, Russian, German, Japanese, Nigerian Pidgin, Egyptian Arabic, Marathi, Vietnamese, Telugu, Hausa, and Turkish. The selection uses total-speaker rankings rather than country population; rankings and regional classifications vary.

Arabic, Egyptian Arabic, and Urdu use right-to-left content. The selector stays on the left. All translations and guides are bundled offline. Application names, user-written checkpoint names, paths, identifiers, and original collected values are not rewritten. JSON/CSV keep stable English schema fields and original evidence. Windows UAC and operating-system dialogs follow Windows' own language. Translations need native-speaker review before production release.

## Settings

Open **Settings** in the navigation menu. Preferences are saved for the current history directory and restored when you reopen the app.

- **Language** changes the interface and offline help immediately.
- **Appearance** offers Light/Dark, a font family, and independent app-text, label, button-background, and button-text colors. Named swatches offer Default, Navy, Forest green, Maroon, and Purple. Default restores that role's theme color. Shades adapt to Light/Dark; Windows high contrast overrides them. Primary actions retain contrasting text.
- **Snapshot frequency** is Off by default. Choose every 15 minutes, hour, 6 hours, day, or week. Checks run only while the app is open, including in the tray, use standard access, and can be canceled. The app checks due work once a minute and does not wake a sleeping PC. An overdue check may run after reopening; missed intervals are not replayed.
- **Data retention** keeps snapshots forever by default. Choose 30, 90, 180, or 365 days to delete older unnamed, non-baseline snapshots. Cleanup runs when first due and then daily while open, even when automatic snapshots are Off, and after successful automatic checks. Named checkpoints and every scope/access baseline are protected. Deleted timestamps leave the selectors.
- **Start ChangeTracker when I sign in** is optional and off by default. It includes sign-in after a reboot, not collection before sign-in. Only this app's per-user startup entry is changed; no service or boot task is installed, and no other app or policy is changed. A failed registration restores the previous choice.
- **Keep running in the tray when closed** is optional and off by default. Minimize or Close hides the window while checks continue. Open or launching another copy restores it. Tray **Exit** cancels active work and quits. Without this preference, Close cancels and exits.

Both **Current user** and **Machine-wide** start selected for new profiles; saved scope choices are preserved. All retained scopes remain browsable from the same history, but comparison endpoints must have matching scope and access. No preference grants administrator access. Resource profiling and installed-package lifecycle qualification remain outstanding.

## Getting Started

1. Open ChangeTracker normally, without **Run as administrator**.
2. Review the **Current user** and **Machine-wide** checkboxes, both selected for a new profile. Keep either or both, never neither, then confirm. Your previous choice is remembered. Selecting scopes does not start collection or request administrator access.
3. Open **Sources** and review the selected sources. Network and PATH are optional.
4. In **Comparison**, leave **Today (new check)** selected. If there is no earlier snapshot, the next check saves current state only.
5. Select **Check now**. The app collects the selected sources, saves the observation locally, and reports incomplete coverage where necessary.
6. After a later configuration change, choose your earlier snapshot and check again, or compare two snapshots already saved.

Your first usable snapshot becomes the baseline for that scope and access level. A first snapshot is inventory, not evidence of earlier changes. Subsequent captures do not automatically move the baseline.

Normal capture happens when you select **Check now**, or when an enabled automatic interval is due while the app is running. Both use standard access; administrator capture requires its separate explicit action. A new profile does not capture automatically. Closing exits unless tray operation is enabled. Changes that happen and disappear between checks can be missed.

## Simple And Advanced

These are **presentation modes**, not separate editions, permissions, or levels of scan depth. Choose either radio option whenever you need it. The app remembers the mode.

| Area | Simple | Advanced |
| --- | --- | --- |
| Findings | Change name, Added/Removed/Modified, priority, readable changed-value summary, and explanation. | All captured fields side by side, including unchanged context, added/removed fields, identifiers, and long values without the summary limit. |
| Before / after | Readable details for the selected finding. | Full fields plus snapshot IDs, scope/access, exact UTC snapshot/source times, collector status/version, schema version, counts, and hidden-value change status. |
| Date comparisons | Two saved snapshots or a saved snapshot versus today. | The same comparison choices. |
| Sources, scope, and history | Available. | Available; no extra collection is enabled by switching modes. |
| Reports | Preview, copy, and save sanitized text. | Text plus JSON and CSV export. |
| Administrator access | Only the separate explicit machine-wide administrator-check action. | Exactly the same rule. Advanced is not administrator mode. |

Switching modes does not run a check, create or delete a snapshot, reset a comparison, move a baseline, enable a source, or grant elevation. Both modes are included in the planned one-time **$0.99** price. This development build has no purchase flow.

## Choosing Scope

**Current user** observes selected configuration associated with the Windows account running the app. **Machine-wide** observes shared system configuration. They are independent checkboxes, both enabled for a fresh profile. Machine-wide does not mean loading all users' private profiles or reading the administrator account's personal settings.

| Source | Current user | Machine-wide |
| --- | --- | --- |
| Installed apps | This user's desktop uninstall registrations. | Machine desktop uninstall registrations in 32-bit and 64-bit registry views. |
| Startup entries | This user's Run and RunOnce entries. | Machine Run and RunOnce entries. |
| Default apps | This user's effective supported file/link handlers. | Not collected. |
| Audio defaults | This user's playback, recording, and communications defaults. | Not collected. |
| Network, optional | This user's proxy settings. | Adapter DNS and DHCP configuration. |
| PATH, optional | Persisted user PATH. | Persisted machine PATH. |
| Services, tasks, updates, drivers, firewall | Not collected in this scope. | Selected readable system sources. |

Use **Change scope** to select another scope. An unavailable source is labeled outside scope and cannot be enabled there. Source selections are remembered separately for each scope. Changing scope never starts a capture or an elevation prompt.

With both scopes selected, user and machine observations are read separately and combined into one snapshot. Both use standard access unless you explicitly request an administrator check. Only the machine portion can elevate; the user portion stays under the original unelevated account. If either portion is incomplete, the combined category is incomplete and cannot infer removals. Combined snapshots have their own baselines and cannot be compared to old single-scope snapshots.

History stays local to the initiating user even for machine-wide checks. The app does not provide shared multi-user history or logged-out monitoring.

## Dates And Snapshots

The **Comparison** section is available in both modes before running a check. It collapses after a successful check/comparison to make more room for results; expand it whenever you want to change the selection.

- **Before date** selects an exact retained snapshot. Each choice shows its local date, time including milliseconds and UTC offset, checkpoint, and scope/access. Dates and times cannot be typed or invented.
- **Baseline** selects the saved standard-access baseline for the currently selected scope. It does not replace that baseline.
- **Current state only**, the clear-reference button, removes the earlier selection and targets a fresh check today without a comparison.
- **Today (new check)** means a new observation captured when you press **Check now**, not a previously saved snapshot from earlier today.
- **Saved snapshot** shows the **After date** selector for an exact retained observation. The main action becomes **Compare snapshots**, which reads saved history without collecting current configuration.

Displayed dates and times are local. Each snapshot also records its observation timestamps; a capture is not an instantaneous whole-PC transaction. Choose two different, chronological, non-overlapping snapshots with matching scope and access.

A date with no snapshot means **no observation is available** and is not offered. Deleted or expired snapshots disappear from both selectors. History from all collection scopes stays visible regardless of the current capture scope, but a comparison requires compatible scope and access at both endpoints.

## Compare Two Saved Snapshots

1. Expand **Comparison** if necessary.
2. Select the desired saved timestamp under **Before date**.
3. Choose **Saved snapshot** on the After side.
4. Select the desired saved timestamp under **After date**.
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

If the earlier snapshot used administrator access, use the separate **Check with administrator access** action and grant the requested approval for this check. The app will not elevate automatically because a date or an administrator snapshot was selected. Use **Current state only** to make a new observation without a compatible earlier reference.

**Cancel** stops an in-progress manual or automatic check and leaves previously saved history and baselines unchanged. Closing during collection requests cancellation before exit unless tray operation is enabled, in which case collection continues in the tray. The tray's **Exit** command always cancels active collection and exits. Changing selectors does not run a check.

## Administrator Access

Many machine-wide settings are readable with ordinary user permissions. **Check now** therefore uses standard access in both scopes. Missing access is a coverage limitation, not a reason to silently elevate.

For protected machine sources:

1. Select machine-wide scope and the sources you want checked.
2. Use a matching earlier administrator snapshot, or select **Current state only** for the first administrator observation.
3. Select **Check with administrator access**.
4. Confirm the app's one-check request. The confirmation defaults to **No**.
5. Respond to Windows UAC yourself. Never share an administrator password through an app report, help search, chat, or log.

Approval is for one check only. The main window stays unelevated. A short-lived read-only helper performs the requested machine checks and returns observations to the initiating app. Only that app saves history. It is not a Windows service and does not create a permanent administrator grant.

Declining or canceling approval leaves history unchanged. A later normal check remains standard access. Device policy may prevent elevation even after you request it; do not disable organizational protections to make a check run.

Canceling in the app cannot dismiss a secure-desktop UAC prompt that Windows is already showing. Dismiss that prompt in Windows. The helper is bounded and cancels on parent exit or loss of its local connection.

Actual UAC approval/denial, alternate administrator credentials, cancellation, and installed-MSIX behavior still require manual qualification. The MSIX source manifest declares restricted `allowElevation`; Microsoft Store approval for that capability has not been obtained. Compilation is not proof of Store approval or successful installed-package elevation.

## Reading Changes

**Added**, **Removed**, and **Modified** describe differences between the selected observations, not a precise timestamp or a confirmed cause.

| Finding | Meaning |
| --- | --- |
| Review first | A rule found a change worth checking, such as a launch registration or a protection setting. It is not a malware verdict. |
| Important | A higher-impact rule matched, such as a firewall setting changing from on to off. Intent and cause remain unknown. |
| Review | A change deserves attention, without a claim that it is harmful. |
| Info / Routine | Lower-priority or version-only activity where the required comparison evidence was available. This is not a safety guarantee. |
| Expected by you | You marked this particular observed occurrence expected. The underlying evidence remains. |
| Other changes | A change was observed but its impact was not assessed by a supported rule. |

The heading counts review-priority findings in the current filter, not every difference. Routine/expected and Other groups have separate labeled counts. Initially, up to three review findings are shown; **View all review findings** exposes the remainder. Category and search filters can narrow the visible list; **Clear filters** restores it.

Each card includes a readable change summary. **Before / after** opens the full available details, interval, reason, and uncertainty. Hidden launch values, protected fingerprints, and some identifiers are not displayed or exported. A hidden-value change can be detected without exposing that value.

In **Advanced**, **All captured fields** includes source/record/endpoint identifiers where captured. **Observation metadata (UTC)** shows exact recorded times and versions. Missing and empty values are distinct; unchanged context remains visible. Long values are selectable and not truncated to the summary limit. Copy details uses this fuller view in Advanced. Device identifiers can identify you; review before sharing. No view can restore sensitive contents that were never stored, and fingerprints/keys stay hidden. Reports retain their stricter privacy projection.

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

Open **Snapshots** to see capture times, checkpoints, scope/access, record counts, and complete-source counts.

- **View selected** displays a saved observation and its compatible baseline comparison when available.
- **Name checkpoint** assigns a local label of 1 to 120 characters.
- **Use as baseline** replaces that scope/access baseline only after confirmation. The original capture time is retained.
- **Delete selected** deletes a saved snapshot after confirmation. Choose another baseline before deleting a snapshot that is currently a baseline.

There are separate baselines for user-only, machine-only, both scopes, their applicable access levels, and legacy mixed-scope observations. A switch between these contexts cannot be interpreted as added or removed settings.

Ordinary captures and custom comparisons do not move a baseline. There is no fixed checkpoint limit. **Settings > Data retention** keeps data forever by default. A shorter window removes only unnamed, non-baseline snapshots older than 30, 90, 180, or 365 days. Cleanup is checked once a minute, runs when first due and then daily while the app is open, and also runs after successful automatic captures. It works even with automatic snapshots Off. Named checkpoints and every scope/access baseline are protected. A capture with no usable sources is not saved as a baseline.

**Clear local history**, under Settings, requires confirmation and deletes app-owned snapshots and expected marks. It retains source/mode preferences and the protected comparison key. It does not delete exported reports or modify Windows settings. This is not forensic erasure.

## Collection Sources And Limits

The app currently implements scoped subsets of 11 categories. It is not the complete 20-category roadmap.

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
| Network | User proxy or shared adapter DNS/DHCP, depending on scope. | Optional; no traffic capture, Wi-Fi passwords, network probing, or full VPN inventory. |
| PATH | Persisted user or machine PATH, including order, duplicates, and unexpanded references. | Optional; no arbitrary environment variables, executable discovery, or directory scanning. |

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

| Local data | Protection and behavior |
| --- | --- |
| Snapshot history and preferences | Local SQLite storage. The database itself is not encrypted. |
| Comparison key | Protected with current-user Windows DPAPI. Sensitive launch/proxy values use keyed fingerprints rather than plaintext storage. |
| Administrator helper exchange | A bounded local named pipe carries only fixed collector requests, an ephemeral key copy, and results. The helper is not given the history path or an arbitrary command/output path. |
| Exported reports | Saved only on request. Redacted, but potentially identifying; not encrypted by this app. |

Do not move only the database and assume hidden values will remain comparable under another account or a different key. A missing/incompatible key makes those comparisons unavailable. Back up app-owned history and key material securely, and retain the Windows user protection context; copying a DPAPI file does not make it decryptable elsewhere.

The scoped-history format uses metadata version 2. Older builds refuse that history rather than misinterpreting it. Legacy mixed-scope snapshots remain labeled as legacy. Back up important data before using an unreleased build. Installed-MSIX data redirection/reset/uninstall behavior still needs qualification; do not assume its lifecycle is identical to the MSI build.

The app has no telemetry, account requirement, invisible uploads, ads, automatic repairs, or executable-command replay. It does not read documents, browsing history, passwords, cookies, microphone content, or packet payloads as collection sources.

## Accessibility And Keyboard Use

Mode and comparison-target choices use radio controls with visible selected circles. Text labels identify change type and priority; color alone is not the meaning. The main font and control outlines have stronger contrast, and keyboard focus has a visible outline.

- **Tab / Shift+Tab** move between controls. Arrow keys change a focused radio selection or list choice; Space selects controls and Enter activates the focused action as appropriate.
- Date/time selectors are non-editable lists of retained snapshots; use the arrow keys to choose an observation.
- Every comparison date, snapshot field, search field, and icon-only action has an accessible name or label.
- Page titles, section headings, and each finding's name expose a heading level, so screen readers such as Narrator can jump directly between them instead of reading every line in order.
- Opening a finding's **Before / after** view moves keyboard focus into it, **Escape** or **Close** dismisses it, and **Tab** cycles only within it; the rest of the window is disabled while it is open so keyboard focus can never land on a control hidden behind it.
- **F1** or **Help me** opens this offline guide. In the help window, **Ctrl+F** focuses topic search and **Escape** closes the window.
- Search filters topics by title and body. Choose a topic to read it. **Text size** changes the help document's zoom without changing Windows settings.
- Windows high-contrast colors are respected. Standard controls remain keyboard-operable; full Narrator and all high-contrast configurations still require broader qualification.
- **Settings > Appearance** offers a Light/Dark theme, a font-family choice, and separate app-text, label, button-background, and button-text colors. Each color selector has named swatches: Default, Navy, Forest green, Maroon, and Purple. Choices apply immediately and survive restart; Default restores that role's theme color. Shades adapt to the theme, and Windows high contrast overrides custom colors. Tested combinations maintain at least 7:1 for app/label text, 4.5:1 for button text, and 3:1 for button outlines. Primary actions use automatically contrasting text.
- Windows' own display-scaling setting (Settings > System > Display > Scale) enlarges this app's text and controls together, since the app declares per-monitor DPI awareness; there is no separate in-app zoom for the main window.

The main window has a minimum supported size. The Comparison section can be collapsed while reviewing results; long content remains scrollable. No claim is made that the preview has passed every accessibility standard or assistive-technology combination.


## Troubleshooting

| Symptom | What to check |
| --- | --- |
| Nothing happens until scope is selected | Confirm at least one of Current user and Machine-wide. Both start selected. Launching alone does not capture data. |
| A date shows no snapshots | There was no saved observation that day. Choose another date; the app cannot reconstruct an earlier state. |
| Compare is rejected | Choose distinct, chronological snapshots with the same scope and access. Use separate times for two observations on one day. |
| Today cannot match an administrator snapshot | Use the explicit administrator-check action, or choose a standard-access reference. No automatic elevation occurs. |
| The current capture succeeded but coverage has gaps | Inspect the earlier snapshot's coverage, format, and key compatibility. Current success does not establish comparison coverage. |
| A source is outside scope | It belongs to the other scope. Changing scope is separate from granting administrator rights. |
| Administrator access was denied or blocked | Continue with standard access and accept documented gaps. Ask the device administrator about policy; do not bypass protection. |
| A source times out | It is recorded as incomplete. Try a later manual check, with a smaller selected source set if needed. |
| No baseline exists | Complete at least one usable check for that scope/access. All-failed captures are not saved. |
| Reports show older results after changing dates | Execute the comparison/check first. Reports describe the currently displayed observation, not pending selections. |
| The app says history cannot be opened | Close duplicate instances and check storage access, free space, and version compatibility. Existing data is not silently deleted. |
| A finding seems routine, such as a browser update | Inspect before/after values and coverage. Mark that occurrence expected only when appropriate; timing/publisher identity alone is not proof of safety. |
| JSON or CSV buttons are missing | Select Advanced. Text preview/copy/save remain available in Simple. |

When requesting support, share a previewed sanitized report and the app/Windows version. Do not share the raw database, comparison key, administrator credentials, or private configuration unless an appropriate secure process explicitly requires it.

## Preview And Installation Status

The planned product is one app at **$0.99 one-time**, including both modes and all features. This preview is incomplete and has no purchase flow.

The repository provides local x64 MSI and MSIX bundle packaging, not a portable ZIP. The MSI is a conventional machine-wide installer under Program Files; installation requires administrator approval, separate from runtime collection consent. The MSIX bundle uses the supplied Partner Center identity. Use one installation format, not both on the same computer.

Current local release artifacts are unsigned previews. They are not a trusted sideload distribution or proof of Microsoft Store publication. Signing, Store certification/capability approval, Windows 10/ARM64 qualification, real elevated workflows, and install/upgrade/uninstall qualification remain outstanding. MSI ICE validation was blocked by the build machine's policy. Do not disable SmartScreen or managed-device protection to run a preview.

Source changes marked **Unreleased** may be newer than existing installers. This guide is embedded when the application is built; it describes the source build that includes it. A release is not automatically rebuilt when source code or help changes.

Still planned: broader collectors, persistent event timeline, richer tags/rules/notes, configurable retention, report import and encrypted bundles, HTML/PDF export, native-speaker localization qualification, and resource-approved background monitoring. Collection consumes resources; no literal zero-CPU collection or universally harmless-change promise is made.