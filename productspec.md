# ChangeTracker Product Specification

**See what changed on your PC. Understand what matters.**

| Document | Value |
| --- | --- |
| Revision | 3.0 draft, comprehensive product scope |
| Updated | 2026-09-18 |
| Product | ChangeTracker, with owner-supplied Partner Center identity |
| App model | One app, one installation, Simple and Advanced modes |
| Price | $0.99 one-time for the entire app; all features and both modes included |
| Platforms | Windows 10 and Windows 11, with explicitly qualified builds and architectures |
| Distribution | Microsoft Store, MSIX-packaged desktop app |
| Delivery model | Phased development governed by correctness, privacy, usability, and performance gates |
| Current state | Development build 1.0.2 implements an initial manual slice; see the README status matrix. Full product and Store certification remain incomplete. |

The owner has chosen a comprehensive product and accepts a longer development effort. This revision supersedes the former narrow MVP, short delivery estimate, checkpoint cap, and requirement numbering. The initial implementation can be incremental, but that first increment is not the full product contract.

Unchanged commitments: fixed $0.99 price, a single app with a freely switchable Simple/Advanced experience, local-first operation, no ads or default telemetry, and read-only observation of monitored Windows configuration. The broader scope does not authorize silent collection, automatic repairs, or a cloud service.

Background monitoring is in the product roadmap, but its previous 0% CPU/GPU condition is unresolved. Literal zero CPU during useful collection is impossible. Section 8 preserves the owner's approval gate and distinguishes proposed near-zero idle targets from measured performance. A longer schedule is not approval to relax that constraint.

## 1. Product Promise

> Before you fix your PC, understand what changed.

ChangeTracker brings selected Windows configuration, installation events, and device changes into one understandable history. It provides a current inventory, saved baselines, before-and-after comparisons, and evidence-backed explanations for everyday users, enthusiasts, developers, and support staff.

Every reported change should answer:

1. What was observed before, and what is observed now?
2. When was the difference detected, or what event time did Windows report?
3. Which data source supports the finding, and how complete is the coverage?
4. Why might the change matter to the user?
5. Where can the user inspect the relevant Windows setting or share the evidence?

Comprehensive means broad, documented coverage with visible limits, not a guarantee to see every Windows change. The app cannot reconstruct an arbitrary earlier configuration without evidence, prove causality from coincidence, or identify malware. A checkpoint is configuration evidence, not a backup or Windows restore point.

### Distinctive Value

- One understandable history instead of disconnected inventories and raw logs.
- Plain-language summaries backed by inspectable before/after evidence.
- Named checkpoints before installations, updates, docking changes, or troubleshooting.
- Visible unchanged areas and missing coverage, not just a list of differences.
- Local, privacy-reviewed support reports that make sense without installing the app.
- Broad technical depth without requiring a novice to navigate a professional dashboard.

## 2. Audiences And Inclusion

| Audience | Typical job | Experience |
| --- | --- | --- |
| Everyday Windows users | Understand a changed browser, microphone, startup behavior, or recently installed app. | Calm summaries, clear dates, familiar names, and a relevant settings action. |
| Enthusiasts and gamers | Compare changes after utilities, device software, drivers, or peripheral installations. | Device/app history with filtering and readable configuration differences. |
| Developers | Inspect PATH, environment, tool registrations, certificates, and optional local listeners. | Ordered diffs, source identity, saved checkpoints, and structured export. |
| Family and small-business support | Review what changed around a reported problem. | Redacted reports, notes, selected periods, and clear evidence limitations. |
| Users with accessibility needs | Complete the same workflows without mouse, color cues, or small text. | Keyboard and assistive-technology support, flexible text sizing, and high contrast. |

The competitive opportunity is usability and connected evidence, not an empty market. [Sysinternals Suite](https://apps.microsoft.com/detail/9p7knl5rwt25), [RegistryChangesView](https://www.nirsoft.net/utils/registry_changes_view.html), [WhatChanged](https://apps.microsoft.com/detail/xpfcxz8h6lcr66), and [System Observatory](https://apps.microsoft.com/detail/9pd7qnt8fxrs) cover overlapping diagnostic or comparison needs. Earlier research established listing claims, not hands-on quality or sales figures. Demand and differentiated usefulness still require user validation.

ChangeTracker is the selected public product name. The owner supplied Partner Center identity `BulentOzkir.ChangeTracker`, publisher `CN=06D08AF4-6BB1-40DF-9B96-5DF27BEE0635`, publisher display name **Bulent Ozkir**, and Store ID `9PGK3NF5MK42`. Full identity metadata and Store links are recorded in [readme.md](readme.md). This does not establish trademark clearance, certification, or publication. Do not rename the repository/remote or existing local-history directory as part of branding.

## 3. One App, Two Modes

Place recognizable **Simple / Advanced** radio controls in the main window, with visible selection indicators, accessible names, and explanatory help. Default to Simple and remember the selection. Anyone can switch modes without a payment, account, restart, or rescan. Mode selection never changes collector consent, monitoring state, permissions, privacy, or retained data. Date-based comparisons and complete available before/after evidence remain available in both modes; Advanced adds inline technical context and structured exports in the current build.

| Capability | Simple mode | Advanced mode |
| --- | --- | --- |
| Current state | Friendly summaries for all enabled categories. | Detailed records, scope, source identifiers, and collection diagnostics. |
| Change history | Today, Yesterday, This week, Since last check, or a named checkpoint. | Full interval controls, compound filters, item history, and source-level views. |
| Comparisons | Guided baseline-versus-current or before/after selection. | Any compatible saved pair, field-level diffs, and normalized/raw-safe views. |
| Checkpoints | Save this setup, friendly name, and optional note. | Tagging, pinning, baseline management, and bulk selection. |
| Monitoring | Opt-in control and clear last-checked/paused status. | Per-source eligible triggers, budgets, and monitoring diagnostics. |
| Noise control | Mark expected and hide expected items from the default summary. | Scoped ignore rules, rule preview, expiry, and notification preferences. |
| Reports | Copy, text/PDF preview, and guided sharing options. | Also JSON, CSV, HTML, custom report fields, and imported-report analysis. |
| Problem investigation | Guided question and time-window selection. | Evidence filtering and inspectable correlations without causal overclaiming. |
| Settings | Privacy, collection coverage, monitoring, storage, and accessibility. | Additional source, retention, and diagnostic controls. |

These are presentation and workflow differences, not commercial tiers. Both modes share one data model, change engine, privacy policy, and feature entitlement. Advanced controls are enabled by the mode switch; they are not a paid upgrade. Sensitive collectors still require specific consent in either mode.

Switching preserves the active comparison, filters, notes, checkpoints, and monitoring preferences. Simple must still show the coverage and outcome of a comparison configured in Advanced; hiding controls must not hide important facts or modify the underlying result.

## 4. Comprehensive Collector Coverage

All rows describe target product capabilities, not claims that each row is fully implemented. Current scoped implementation and limitations are recorded in [readme.md](readme.md). Phase names refer to section 14. Each source must pass standard-user access, privacy, correctness, and performance tests independently before being advertised. A source can support manual snapshots while remaining ineligible for background monitoring.

| ID | Category | Planned coverage | Boundary and delivery |
| --- | --- | --- | --- |
| COL-01 | Installed applications | Desktop uninstall registrations, current-user Store/MSIX packages, versions, publisher, scope, architecture, and recognized package-manager metadata where locally available. | Foundation/E1. No `Win32_Product`, forced installation checks, untrusted executable invocation, or claim to discover every portable app. Package-manager attribution requires actual evidence. |
| COL-02 | Startup applications | User/machine Run and RunOnce entries, Startup folders/shortcuts, and exposed packaged startup registrations; changes to launch configuration and reliably exposed enabled state. | Foundation/E1. Startup tasks/services reference their canonical records to avoid double counting. Presence alone does not prove execution; do not execute shortcuts. |
| COL-03 | Scheduled tasks | Added/removed tasks and changes to enabled state, actions, triggers, settings, and non-secret principal metadata. | E1. Accessible Task Scheduler sources only; no task execution, credential access, or persistence of full raw task XML. Hidden/protected scopes remain explicit gaps. |
| COL-04 | Windows services | Added/removed services, startup/delayed-start settings, dependencies, sanitized launch configuration, and exposed account/configuration changes. | E1. Runtime running/stopped observations are separate from configuration differences and optional to avoid noise. No frequent state polling or service control. |
| COL-05 | Windows Update | Locally exposed successful installs/uninstalls, cumulative/security/feature/optional updates, and driver/firmware events where identified by the source. | E1. History is an event stream, not a complete installed-update inventory. Never infer uninstall from history pruning or initiate an update scan/download. |
| COL-06 | Drivers and firmware metadata | Device/driver associations, provider, version, package identifiers, and exposed firmware version metadata. | E1. Stable device identity is required; driver dates are not installation times. No firmware probing/flashing, hardware stress tests, or complete-firmware claims. |
| COL-07 | Default applications | Browser protocols, mail, and selected PDF/image/media/archive/text/data associations, with a configurable allowlist. | Foundation/E1. Query effective handlers; do not manipulate protected UserChoice values. Begin with HTTP/HTTPS/PDF, then expand tested coverage. |
| COL-08 | Audio configuration | Default playback, recording, and communications endpoints; connection/availability and friendly-name changes where exposed. | E1. No audio capture or recording permission. Separate endpoint rename, disconnection, and default-role change; app-specific overrides are not inferred. |
| COL-09 | Displays | Connected monitors, primary display, resolution, refresh rate, orientation, scaling, and HDR state where supported by public APIs. | E1. Display and docking events trigger bounded rechecks only after resource approval. Unsupported HDR/scaling fields remain unknown, not off. |
| COL-10 | Devices and printers | Exposed PnP/USB/Bluetooth device presence, relevant configuration, printers, and default-printer changes. | E1/E2. No scanning for nearby devices or reading document/print contents. Persistent serials/hardware identifiers are sensitive and omitted from default exports. |
| COL-11 | Network configuration | Adapters, DNS, proxy, route configuration, exposed Wi-Fi/VPN profile metadata, and connection-profile changes. | E2. Expanded metadata remains opt-in; implemented read-only DNS/DHCP/proxy checks now default on. No passwords, packet capture, browsing/traffic history, external network probes, or assumption that temporary DHCP changes are faults. |
| COL-12 | Developer environment | Persisted user/system variables, ordered PATH, and locally exposed toolchain/package-manager registrations. | Foundation/E2. Hidden values use keyed fingerprints; plaintext only for reviewed safe fields. Preserve order/duplicates and do not run discovered tools to identify them. |
| COL-13 | Browser extensions | Extension identifiers, names, versions, and reliably exposed enabled/permission metadata for consented Edge/Chrome/Firefox profiles. | E2. Browser-specific adapters must declare supported versions. No browsing history, cookies, passwords, extension execution, or automatic inspection of all profiles. |
| COL-14 | Shell and context menus | Accessible shell extension registrations, context-menu handlers, and selected file-class integration. | E2. No handler loading/execution. Packaged/legacy registrations may differ; unsupported locations are disclosed. |
| COL-15 | Windows policies | Selected readable effective policy values and locally exposed GPO/MDM-related changes. | E2. Opt-in; do not claim a particular administrator or management system caused a change without direct evidence. No policy bypass or credential use. |
| COL-16 | Selected registry configuration | Reviewed built-in key sets and explicit user-selected locations with depth, value-type, and size limits. | E2. No whole-registry sweep, credential stores, or SAM/SECURITY hive collection. Sensitive fields remain fingerprinted or excluded. |
| COL-17 | Certificate metadata | Selected current-user/machine certificate-store metadata: subject/issuer, validity, store membership, and stable certificate identity. | E2. Opt-in. Never read private keys or export certificate material by default; names and thumbprints may be sensitive. |
| COL-18 | Local listening endpoints | Optional snapshot of local listening ports, protocol, binding scope, and safe owning-process identity. | E2. Opt-in, initially manual. No remote traffic history or process command lines; listeners are transient observations, not proof of persistent configuration or malicious behavior. |
| COL-19 | Selected configuration files | Explicitly selected configuration-file metadata and optional content fingerprints with path/size limits. | E2. No stored file contents, arbitrary document collection, volume-wide recursion, or executable evaluation. Hashing costs count toward the resource budget. |
| COL-20 | Selected Windows settings | Publicly readable settings such as firewall profile state, exposed protection settings, and power configuration. | E2. Reviewed allowlists only; do not disable controls, calculate a security score, or present observations as a protection guarantee. |

### Collection Profiles

- **Recommended:** all currently implemented checks supported by the selected scope default to enabled, including the limited network configuration and persisted PATH checks. Preserve saved per-source choices and show the exact checklist before the first capture. Unsupported sources stay disabled. This does not authorize unimplemented sensitive collectors or change the resource-qualification gates.
- **Extended:** additional enabled categories chosen explicitly, with disclosures for sensitive browser, policy, network, certificate, listener, registry, and file metadata.
- **Custom:** select supported sources and reviewed fields, with per-source enable/disable, coverage, and last-check information.

Profiles control collection; Simple/Advanced controls presentation. A mode change never broadens the collection profile. Adding a newly implemented sensitive collector after an update requires fresh consent. Capture results record the profile/version actually used.

### Product Boundaries

Comprehensive observation does not include cleaning, performance boosting, disabling startup items, service/task management, driver rollback, antivirus claims, automatic restoration, remote control, or account/cloud sync. No third-party plugins may execute inside the app without a separately reviewed future design. Optional AI analysis is not part of this release plan. These boundaries preserve trust even with a larger feature set.

## 5. Required Workflows

| ID | Workflow | Required behavior |
| --- | --- | --- |
| FR-01 | First capture and baseline | Show collection scope, capture on request, save available results, and distinguish inventory from changes. No usable baseline when all sources fail. |
| FR-02 | Manual check | Capture enabled sources with progress, cancellation, and recoverable partial results. Preserve the user's chosen baseline. |
| FR-03 | Mode switching | Expose Simple/Advanced workflows without payment, rescan, permission change, or lost state. |
| FR-04 | Named checkpoints | Create, rename, tag, pin, and annotate snapshots; designate or replace a baseline explicitly. No artificial count cap or paid history tier. |
| FR-05 | Endpoint comparison | Compare two compatible snapshots, including baseline/current and before/after installation checkpoints. Show changed fields and coverage for each selected endpoint. |
| FR-06 | Historical timeline | Browse observed changes by date, category, item, source, and importance; keep occurrence evidence separate from detection time. |
| FR-07 | Optional automatic collection | After resource approval, support eligible event-driven capture and explicitly configured scheduled snapshots with catch-up and pause/resume controls. No silent activation. |
| FR-08 | Tray and notifications | After lifecycle/resource qualification, offer optional tray operation, separate start-at-sign-in consent, and coalesced notifications for user-selected important changes. |
| FR-09 | Noise management | Mark expected, annotate, group routine changes, and preview scoped ignore rules with expiry/undo. Preserve original evidence and coverage. |
| FR-10 | Guided investigation | Ask what changed for the user and when, then filter relevant observed categories. Report related evidence and limitations, not an invented diagnosis. |
| FR-11 | Reports and sharing | Preview/copy/save current-state, comparison, or timeline reports as text, JSON, CSV, HTML, or PDF using one safe projection. Never share automatically. |
| FR-12 | Portable support reports | Import versioned sanitized reports for offline inspection; offer separately protected support bundles after security review. Treat imports as untrusted data, never an executable instruction or local baseline. |
| FR-13 | Source controls | Select collection profiles, view permissions/unsupported fields, retry a failed source, and disable individual collectors without disabling unrelated categories. |
| FR-14 | History management | Show data size, retention policy, pinned items, export/archive options, and explicitly confirmed deletion. Preserve data through compatible app updates. |
| FR-15 | Accessibility and localization | Keyboard, Narrator, large text, high contrast, regional formats, resource-backed translations, and accessible mode switching across every workflow. |
| FR-16 | Safe lifecycle | Single instance, bounded native workers, transactional captures, graceful shutdown, crash recovery, migration, and offline standard-user operation. |

## 6. Experience And Navigation

### First Launch

1. Open the actual application, not a marketing page. Display the name, Simple/Advanced switch, current collection profile, and **Create baseline**.
2. Explain briefly that snapshots stay local and the app observes rather than repairs settings. Let the user inspect the exact collection checklist.
3. Run asynchronously with per-source progress, elapsed time, cancellation, and safe error details.
4. Save a partial baseline if useful sources succeeded and clearly identify missing areas. Existing items are not presented as newly installed.
5. Show **Baseline ready. No comparison yet.** Give immediate access to the current inventory and Check now.
6. Offer monitoring, schedules, sign-in startup, and notifications separately only when those features have passed the resource gate. Declining them preserves all manual workflows.

A first-run historical Windows event query can display earlier OS-reported events in a separately labeled history view. It cannot reconstruct prior configuration or fabricate a pre-installation snapshot.

### Primary Destinations

| Destination | Simple presentation | Advanced presentation |
| --- | --- | --- |
| Overview | Review first: up to three explained findings, total review count, collapsed routine activity, and a persistent coverage strip. Do not show 20 category tiles. | Same triage with source diagnostics, classification reasons, and richer filters. |
| Timeline | Today, Yesterday, This week, Since last check, and checkpoint presets. | Exact intervals, item history, compound filters, and grouped/ungrouped events. |
| Compare | Guided before/after selectors and readable changes. | Field-level side-by-side views, scope/source keys, ordered PATH, and evidence details. |
| Snapshots | Named saved setups with dates, notes, and pinning. | Tags, profile metadata, bulk management, and compatibility/coverage inspection. |
| Reports | Friendly preview and format/share choices. | Structured fields, redaction review, source diagnostics, and imported reports. |
| Settings | Collection, monitoring, privacy, storage, accessibility, and about/support. | Detailed collector options, ignore rules, retention, and local diagnostics. |

Use a compact, work-focused layout, native control conventions, restrained category accents, and readable before/after values. Long paths and labels wrap; they never overlap controls. Use icon commands with tooltips and accessible names, toggles for binary preferences, and visible labels for the mode switch. Respect high contrast and reduced motion; no mandatory animation or GPU-driven background effects.

### Review First, Not An Inventory Dump

The collector matrix is an engineering coverage map, not the home screen. Simple mode answers **What should I look at?** using three groups:

| Group | Admission rule | Presentation |
| --- | --- | --- |
| Review first | An observed difference matches a tested impact rule: protection reduced, automatic launch configuration added/changed, or a user-facing default/network/tool-resolution setting changed. | Show at most three findings initially, the full count, and View all. Important sorts ahead of Review; problem-relevant items can be prioritized without changing their evidence or declaring a cause. |
| Routine activity | A narrow rule identifies an ordinary update to an existing item with sufficient comparable coverage and no observed review-triggering change, or the user explicitly marked that occurrence expected. | Collapse to a count and short summary. Label user-marked expectations separately from rule-based routine updates; neither means safe or malware-free. |
| Other changes | A valid observed difference lacks enough evidence for either classification. | Show a count and expandable list labeled Impact not assessed. Never silently classify an unfamiliar or uncertain change as routine. |

Coverage is a separate always-visible status: failed, stale, and disabled sources are not routine activity or unclassified changes. Show **Some areas could not be checked** even when the review count is zero. Use **No review-priority changes detected in checked areas**, never **Your PC is safe**.

Each review finding has four essentials: **Before / After**, **Why shown**, **What is not known**, and **Open relevant settings**. Offer **Mark this change expected** as a secondary action. The user should not need to read Registry paths, judge a publisher reputation, or interpret a raw diff before understanding the observation. Advanced exposes those details and the rule/evidence behind the same result.

Example, illustrative only: **Your Private-profile firewall changed from On to Off. Why shown: protection for this profile is now disabled. What is not known: who changed it or whether it was intentional. Open Windows Security.** A new startup registration gets a different explanation about possible sign-in launch, not a claim that it is malicious or actually executed.

### Example Change

The following is illustrative content, not a real observation:

```text
ChangeTracker                             [Simple | Advanced]

Your default microphone changed
Before: Jabra Evolve 75
Now: HD Webcam Microphone
Detected between: 17 Sep, 18:00 and 18 Sep, 09:00

Apps using the Windows default microphone may use a different device.
The source of this change is not known.

[Open Sound settings]   [Copy details]
```

The timeline may also show a nearby driver/update event, but the interface must not say it caused the microphone change unless direct evidence supports that claim.

### Important States

- **No baseline:** show current-state capability and Create baseline, not invented changes.
- **No comparison yet:** keep baseline-only status distinct from no changes.
- **No important differences:** retain routine changes and list only successfully compared unchanged areas.
- **Partial, stale, or paused:** show last valid observation and exactly what could not be checked; never a blanket all-clear.
- **Source newly enabled:** establish its own baseline; do not label its entire inventory as newly added.
- **Capture canceled:** preserve the previous committed state and selection.
- **Storage/corruption problem:** preserve original files and offer a separately confirmed recovery path; do not silently reset history.
- **Imported or cross-PC report:** prominently distinguish external evidence from this PC's observations.

## 7. Change And Evidence Semantics

### Identity And Coverage

Use stable category-specific keys including relevant scope/architecture/profile, not display name, version, or enumeration order alone. A product changing its registration key may remain an addition/removal pair unless a tested identity rule can safely match it. Record collector, schema, normalization, and rule versions.

Collector outcomes are `success`, `partial`, `failed`, `unsupported`, or `disabled`. Successfully empty is a real observation; denied access is not. A complete coverage unit is the comparison boundary. Initially that may be a whole category; finer partitioning is permitted only when completeness can be proven per partition. Never infer removal from a missing record in an incomplete unit.

Explicit two-snapshot comparisons use exactly the selected endpoints and show gaps; they do not silently substitute a different snapshot. The rolling timeline may bridge failed captures using the previous successful compatible observation for a unit, with the wider interval displayed. A source without a prior comparable observation becomes a baseline, not a mass-added event.

### Time And History

- Store snapshot and collector start/end in UTC with recorded local offsets. A multi-source snapshot is not an atomic image of the operating system.
- Use conservative **Detected between A and B** windows for state differences. Keep authoritative OS event time, event-source identity, and precision separate.
- Do not convert driver dates, uninstall registration dates, or file timestamps into an exact configuration-change time without evidence.
- A-to-B-to-A can produce two observed timeline events and no endpoint difference. Label timeline and net-comparison reports differently.
- Detect duplicate source events. Missing entries in a pruned update/event log never prove an uninstall or rollback.
- Preserve gaps during sleep, sign-out, disabled monitoring, or failed collection. No real-time-completeness claim.

### Importance And Explanation

| Label | Meaning | Typical examples |
| --- | --- | --- |
| Info | Useful record with no identified high-impact difference; not automatically routine or safe. | A known app-version change eligible for a narrow routine rule, or an observed change whose impact has not been assessed. |
| Review | Configuration changed in a way the user may want to inspect. | New apps/tasks, changed audio/default apps, PATH reorder, device or network settings. |
| Important | A deterministic rule identifies a high-impact configuration change worth surfacing. | A newly configured auto-start mechanism or a selected policy/protection setting changing, with scope and uncertainty stated. |

Importance is not a threat probability. No malware verdict, invented health score, or automatic Critical label. Use factual language such as "Startup entry added" when enabled state or actual execution is unknown. Explain potential effects without predicting a fault or prescribing deletion.

Classification uses the actual before/after fields, comparable source coverage, stable identity, and explicit user expectations, not the category name alone. A familiar publisher, valid signature if available, successful installation, recurring event, or a change made during a named checkpoint never establishes safety. Same-time events cannot cancel one another's review flags.

| Observed evidence | Triage outcome | Reason shown to the user |
| --- | --- | --- |
| A readable, comparable firewall/protection setting changes from enabled to disabled. | Important; Review first. | The named protection setting is now disabled; intent and cause are not established. |
| A startup entry is added, or an existing task/service launch configuration materially changes. | Important for verified automatic-launch configuration; otherwise Review with the limitation. | A new or changed background launch mechanism was observed; enabled state is shown only when reliably known. |
| Browser/PDF handler, microphone default, DNS/proxy, or PATH precedence changes. | Review; Review first. | The concrete behavior this setting controls may differ. This does not prove a fault or malicious action. |
| Only the version of a stable existing app changes, with required related source coverage available and no observed review-triggering differences. | Info; Routine activity, unless the selected symptom makes it relevant to inspect. | An existing application version changed; no additional review-triggering differences were observed in the checked sources. |
| A new setting or ambiguous identity has no reliable impact rule. | Info or Review as justified by observed fields; Other changes if impact is not assessed. | A difference exists, but its significance is not known. |

Routine rules specify exactly which fields and coverage units they depend on, and are versioned and fixture-tested. Missing required coverage disqualifies an automatic routine judgment; a gap elsewhere remains separately visible. Related changes may be grouped for readability, but a routine update never hides a new startup entry or a protection change. Symptom filters such as **Sound**, **Internet**, or **Opening files** only prioritize related evidence and must offer a return to all findings.

ChangeTracker helps users prioritize investigation; it cannot reliably divide all changes into harmful and harmless. For security concerns, open Windows Security or the user's security product for assessment. Do not run a scan, claim an antivirus integration, or create a threat verdict merely from snapshot differences.

### Attribution

Separate **observation source**, **related component**, and **cause**. Show no actor by default. A related installation/update can be labeled as a correlation with its supporting evidence; timing or publisher match alone does not prove causality. Confirmed attribution requires a direct, reliable record linking the actor to the specific change. If the source cannot establish that, say **Source of change not available**.

### Expected Changes And Rules

Marking expected adds a reversible annotation; it does not modify Windows, rewrite evidence, or change when the event was observed. Ignore rules affect presentation/notifications by default, not collection. Show their match criteria, affected sample events, scope, and expiration. A separate collection-disable action is required to stop reading a source. Never suppress collection errors with an ignore rule intended for normal changes.

By default, **Mark this change expected** applies only to that occurrence, not all future changes from the same app, publisher, or category. Original importance and its reason remain visible in history. Broad routine/ignore rules must not silently hide a newly reduced protection setting or materially changed automatic-launch configuration; an explicit event acknowledgment can collapse that occurrence without erasing it.

## 8. Monitoring And Resource Contract

### Approval Still Required

**Owner constraint:** background monitoring and complex collectors must have 0% CPU/GPU overhead. Literal zero CPU during collection cannot be guaranteed. Notification handling, snapshots, hashing, comparison, persistence, and garbage collection all cost CPU; some also use disk I/O. A rounded Task Manager 0% display is not proof of zero work.

Expanded feature scope and acceptance of a longer schedule do not approve a different resource budget. The following remains a proposal for owner review, not an approved relaxation:

| State | Proposed requirement | Measurement |
| --- | --- | --- |
| Monitoring disabled | Unregister monitors and stop automatic collection; the visible app still works on demand. | Verify no background subscriptions, automatic captures, or spinning retry loops. |
| Explicit Exit | Terminate app workers and unregister live callbacks; no monitoring process remains. | Verify process exit and cleanup. A separately consented sign-in startup preference may remain registered until the user disables it. |
| Quiet monitor idle | Event-driven blocking waits; no polling, recurring idle timer, scans, animation, rendering, or periodic database/log writes. Proposed ceiling: 0.1% of one logical CPU averaged over 30 minutes after initialization. | Sum CPU time across all app processes; include runtime housekeeping and inspect attributable OS work separately. |
| Event or scheduled capture | Bounded CPU/I/O bursts are unavoidable and must have per-collector limits agreed from measurements. | Record CPU-seconds, wall time, peak memory, reads/writes, and impact of event bursts; idle averages cannot conceal expensive scans. |
| Background GPU | No GPU compute, offscreen rendering, UI animation, or app-submitted GPU work while the window is hidden. | Measure app/worker GPU activity and account separately for shared Windows composition. |
| Visible UI | Normal WPF rendering can use CPU/GPU; avoid unnecessary redraws. | Measure separately; do not force CPU rendering simply to hide GPU utilization. |

The proposed idle ceiling is 1.8 CPU-seconds across app-owned processes over 30 minutes, without dividing by logical core count. Low process utilization does not excuse work induced in Windows services, repeated disk access, battery drain, or a large memory footprint. Profiles must include an observer-baseline run and document hardware, Windows build, and enabled sources. Reference measurements are not a universal guarantee.

Until the owner accepts a measurable nonzero-work budget and profiling passes, background monitoring and automatic schedules remain unavailable to users. If literal zero remains required, retain manual capture and full process exit when closed. Additional manual collectors still need measured performance and cannot be described as zero-cost. Development smoke-test timings are not qualification of the proposed background resource budget.

### Event-Driven Operation

- Monitoring is off by default and independent of the mode switch. Each eligible source has explicit collection consent, an approved trigger, and a bounded execution strategy.
- Prefer native notifications for scoped Registry keys, files, package/device changes, and Core Audio endpoints where suitable. Event receipt marks data stale; it is not itself proof of the exact change or its cause.
- Coalesce event bursts using bounded queues and one-shot delays, with one snapshot commit at a time. Do not enumerate every collector for every callback.
- No broad permanent event trace, WMI polling loop, or full-inventory timer. Without a reliable low-cost trigger, use manual capture or an explicitly enabled, budgeted scheduled capture instead of frequent probing.
- On overflow, access loss, or event storms, mark the interval incomplete and pause/degrade the affected source. Do not spin, silently drop evidence, or claim complete coverage.
- Background results append to history without moving the pinned baseline or replacing an actively viewed comparison. Show a New results available action.
- Pause/resume controls explain what will no longer be observed. Start/resume reconciliation is bounded active work, not a zero-cost idle operation.

### Schedules, Tray, And Notifications

Scheduled daily/weekly snapshots are a full-product goal, separately opt-in after resource approval. They are user-requested captures, not idle polling. Catch up once at the next eligible launch/resume after a missed interval; never synthesize missed snapshots, wake the PC, or run while signed out through a privileged service. Prevent duplicate captures after clock/time-zone changes and repeated launches. Separate scheduling targets must be tested on battery and constrained machines.

Keep **Monitor changes**, **Start with Windows**, and **Notifications** independently understandable. Per the current lifecycle requirement, Close and Minimize always hide the main window to the tray; explicit tray Exit stops collection and terminates the app. Normal launches start maximized, including a launch that reopens the app while it runs in the tray; tray Open restores the last visible window state, while sign-in launches remain hidden. Start-at-sign-in remains separately opt-in, and disabling it removes only the app's own startup registration; never override a Windows or organizational block. These lifecycle choices do not establish resource-budget qualification.

Aggregate eligible Important changes into at most one notification per completed capture, with deduplication and user-controlled quiet preferences. Respect Windows notification suppression. Do not notify for a baseline, routine Info changes, repeated report viewing, or every collector failure. Lock-screen text omits paths/commands and other sensitive details. Notification activation opens the relevant saved interval in the existing instance or relaunches the app without silently enabling disabled monitoring.

## 9. History And Data Model

There is no artificial checkpoint-count limit or paid history restriction. Capacity is governed by available storage and a user-visible retention policy, not by the selected mode. A pinned baseline and named/pinned checkpoints are never silently pruned.

| Entity | Required contract |
| --- | --- |
| Snapshot | Local ID, capture interval/offset, schema version, collection-profile version, manual/event/scheduled source, OS/build and lineage metadata. |
| CollectorRun | Category and coverage unit, version, status, scope, start/end, count, safe diagnostics, and performance measurements where recorded. |
| Record | Stable key, normalized typed comparison fields, safe display fields, sensitivity tags, fingerprint/key version, and optional source event identity/time. |
| Change | Before/after observation references, kind, changed fields, detection interval, separate source event time, importance, explanation/rule version, and evidence references. |
| Checkpoint | Snapshot reference, name, tags, note, pin status, and baseline role. Annotation changes do not change the capture time. |
| AnnotationRule | Expected/ignored state, explicit match scope, creator action time, optional expiry, and undo/history information. |
| Comparison | Exact selected endpoints or timeline interval, changes, unchanged checked units, incompatible units, and gaps. |
| Report | Versioned current-state/comparison/timeline projection, provenance, coverage, redaction metadata, and limitations. |
| Preferences | Mode, source consent, monitor/schedule/lifecycle options, baseline, retention, report defaults, accessibility, and localization. No edition or mode-entitlement flag. |

Use SQLite in per-user local application data with migrations, foreign keys, transactional snapshot/derived-change writes, and indexed metadata. Versioned typed payloads can remain JSON where appropriate; do not duplicate an entire snapshot for every view. Lazy-load or paginate large histories and preserve raw evidence needed to reproduce a report within the privacy contract.

Choose retention explicitly during monitoring setup: keep until deleted, a selected time window, or a storage budget. Show a preview of what expires and what remains pinned. Without an accepted policy, do not silently prune; stop new writes safely if storage is exhausted. Preserve a comparison anchor before a retained timeline boundary where possible, and label a missing anchor otherwise. Pruning must not corrupt selected reports or foreign-key references.

Protect app-owned writes against interruption. Back up before schema migration where feasible, reject unsupported future schemas safely, and preserve a corrupt store for user-directed recovery. History deletion requires confirmation, respects pinned/baseline warnings, and never deletes exports saved outside the app. Document actual package reset/uninstall behavior and avoid claims of forensic erasure.

## 10. Privacy And Security

Comprehensive collection increases privacy risk. Every collector has a field inventory, sensitivity classification, export policy, and explicit justification before activation. Collect the minimum evidence for a change, not everything an API returns.

- No passwords, tokens, private keys, cookies, browsing history, keystrokes, microphone content, document contents, packet payloads, or credential-bearing Registry hives.
- Startup/task/service commands and general environment values may contain secrets. Keep safe metadata plus per-installation keyed fingerprints for hidden-value changes; do not persist raw arguments or expose hidden fingerprints in reports.
- Use a reviewed safe-field allowlist for plaintext environment values. Name-based secret detection alone is insufficient. Preserve PATH ordering locally but never run or traverse discovered directories automatically.
- Protect fingerprint keys using current-user Windows data protection. Key loss/version mismatch makes affected comparisons unavailable or requires explicit rebaselining; it must not produce false mass changes.
- Browser profiles, network names, device IDs, certificate subjects, policy details, file paths, annotations, and app names may identify users or organizations. Treat them as potentially sensitive even without credentials.
- No invisible upload, analytics, automatic crash reporting, or online reputation enrichment. Local diagnostics use safe error codes and redacted messages, with user-initiated sharing only.
- Ask for current-user or machine-wide scope before first collection. Machine-wide means shared configuration, not blanket access to other users' profiles. Remember the scope and per-scope sources, never administrator consent; changing scope or presentation mode must not start a check or request elevation.
- Never request elevation. The app and every check run only in the signed-in user's default, unelevated security context (Microsoft Store policy 10.6.3 denied the restricted `allowElevation` capability). Checks in either scope report inaccessible sources as coverage gaps rather than retrying with administrator privileges; there is no administrator action or UAC prompt.
- Keep the UI and collector workers unelevated: the executable manifest requests `asInvoker`, workers are direct child processes that inherit the app's token, and an elevated launch is rejected. There is no privileged helper, service, persistent grant, or administrator-profile capture. Snapshots saved with administrator access by earlier versions stay readable and comparable with each other but never seed a new check.
- Record scope and access on each snapshot/report and keep compatible baselines separate. Never infer changes or reassurance across user/machine/elevation/legacy boundaries; retain older mixed-scope evidence without relabeling it.
- Use least-privilege file permissions and reviewed protection for sensitive retained fields. Do not claim whole-database encryption merely because DPAPI protects a key; database/archive encryption claims require their own implementation and verification.
- Treat imported data, source strings, names, paths, and links as untrusted. Escape display/export content; never evaluate a command or follow a report-supplied settings URI.

Read-only means the app does not change monitored Windows configuration. It can write its own history/preferences, explicitly requested exports, and its own consented startup registration. Inspection actions use tested allowlisted Windows settings or management destinations, with safe fallbacks. There are no repair buttons disguised as inspection.

## 11. Reports And Support Workflows

All formats derive from one privacy-filtered report model. A report records whether it is current state, a net endpoint comparison, or a historical timeline. Include selected/actual dates, offsets, source versions, checked coverage, gaps, changes, counts, annotations when consented, and interpretation limits.

| Format | Purpose | Required safeguards |
| --- | --- | --- |
| Text | Copy/paste into support messages. | Clear headings, stable ordering, safe values, and no hidden diagnostic dump. |
| JSON | Machine-readable support and future compatibility. | Explicit schema version, typed data, provenance, redaction metadata, and strict bounded parsing on import. |
| CSV | Spreadsheet review of flat change records. | Correct quoting/encoding and protection against formula injection in untrusted cells. |
| HTML | Readable offline report with linked sections. | Escape all data, no script execution, no remote assets/tracking, and no embedded action commands. |
| PDF | Shareable, printable summary and detail report. | Selectable text, readable pagination, long-value wrapping, accessible structure where supported, and sanitized metadata. |

Default redaction applies throughout titles, safe values, evidence, diagnostics, notes, and metadata. Replace personal profile paths, omit unnecessary identifiers, exclude raw arguments/hidden values/fingerprints, and omit free-text annotations unless the user includes them after preview. Advanced may select additional retained non-secret fields; it does not reveal discarded secrets or disable mandatory secret filtering. Explain that redaction reduces risk but does not guarantee anonymity.

Preview before save, confirm overwrite, and report failures without changing history. Do not email, upload, or publish a report automatically. An optional encrypted support bundle is in the full-product plan only after reviewed authenticated-encryption and key-sharing workflows exist; reuse maintained cryptography libraries from approved sources, never invent a cipher or hide a key in the bundle.

Imported reports open read-only in an external-evidence view, with file-size/count limits, schema validation, bounded decompression if used, and no active content. Do not merge them into the local baseline or execute source actions. Cross-PC report comparison is a labeled inventory comparison, not a claim that one device changed into another; incompatible identities, redaction, and per-installation fingerprints remain incomparable.

Required limitation:

> This report describes selected configuration observations and available event records. It may miss changes between observations or in unavailable areas. Related events do not establish causation or diagnose a computer problem.

## 12. Architecture And Platform

Use **C# / .NET 10 LTS, WPF with MVVM, and SQLite** as the implementation direction, subject to supported-OS/dependency and packaged-build validation. Keep the app local; no backend is required. Add Windows App SDK components only for concrete needed capabilities, not because WPF packaging requires them.

Separate ownership of domain models/comparison, Windows collectors/native adapters, persistence/reporting, and desktop presentation/lifecycle. Maintain a shared collection/result contract rather than separate Simple and Advanced implementations. A bounded worker process is appropriate for native calls that cannot reliably be canceled; it remains part of the same product and resource accounting, not an elevated service or second edition.

### Processing Pipeline

1. Resolve consented profile, trigger, and collector eligibility.
2. Collect with per-source timeout/isolation, progress, and cancellation.
3. Apply minimization and stable normalization, retaining coverage and provenance.
4. Atomically store observations and derive deterministic changes against compatible history.
5. Apply presentation rules/annotations without rewriting original evidence.
6. Present, notify, and export through the same result model and privacy controls.

Native adapters must release handles/COM objects, bound queues and timeouts, isolate failures, and expose fixture-testable normalization. Avoid `Win32_Product`, repeated whole-machine enumeration, direct credential-store access, unreviewed shell execution, and undocumented write APIs. Store/packaged context can affect readable state; compare packaged and unpackaged observations during qualification.

### Platform Coverage

- Qualify Windows 10 22H2 x64 and supported Windows 11 x64 releases first.
- Include native Windows 11 ARM64 in the full-product target, with actual hardware/native-dependency tests before advertising support.
- Decide older Windows 10/LTSC and x86 separately based on supported SDK/runtime combinations and demonstrated demand; do not claim all Windows versions by default.
- Test standard users, organizational restrictions, multiple profiles, offline PCs, laptops, docking/peripheral transitions, high DPI, and regional settings.
- Technical compatibility does not extend Windows servicing/support lifecycle. Document tested builds and source limitations per release.

### Approved Package Feeds

All package installation, restore, and generated package-manager configuration must use approved feeds only:

- NuGet: `https://packagefeedproxy.microsoft.io/nuget/v3/index.json`
- PyPI, only if future tooling needs it: `https://packagefeedproxy.microsoft.io/pypi/simple`

Do not fall back to public feeds, add unapproved sources, or download third-party utilities as a substitute for collectors. Report blockers. Pin/review dependencies and their licenses; Python is not required by the application runtime. Signing material, credentials, real snapshots, and private reports stay out of source control.

## 13. Price And Store Positioning

The product price is fixed at **$0.99 one-time for the entire app**. All planned features and both modes are included. The price is not a hypothesis to raise after validation. No separate Pro SKU, mode fee, subscription, in-app purchase, or developer-operated activation server.

Microsoft Store acquisition/licensing may involve Microsoft's account and network services; the app has no product account and does not upload snapshots. A transient license lookup failure must not delete data or lock mode switching. Verify Store licensing/offline behavior before release, independently of feature presentation.

**Selected Store title:** ChangeTracker

**Tagline:** See what changed on your PC. Understand what matters.

**Full-product description draft:**

> Compare your PC over time. Follow changes to apps, startup items, services, tasks, Windows updates, drivers, default apps, audio, devices, and other settings. Get clear before-and-after explanations in Simple mode, or switch to Advanced for detailed evidence, checkpoints, filters, and support reports. Local-first. Read-only. No ads. One purchase includes both modes.

Publish only the subset actually implemented, qualified, and included in the submitted build. Do not use this full-product draft to market an early foundation build. Mention optional monitoring only after resource approval and runtime validation. Do not advertise zero overhead, guaranteed root-cause diagnosis, security protection, or an exact change time unsupported by evidence.

The package now uses the owner-supplied Partner Center identity. Store readiness still requires name/branding clearance, privacy and support URLs, appropriate capability declarations, accessible screenshots of real features, and applicable certification checks. Install/update/reset/uninstall, offline use, signing, and retention behavior must be tested on every advertised architecture/build. No Store submission or independent reservation check has been performed by this work.

## 14. Phased Delivery

Longer development is accepted. There is no fixed short-build deadline in this specification. Estimate each phase after its API, privacy, and packaging risks are understood; do not compress missing capabilities into misleading placeholders. Staged betas may ship clearly labeled subsets while the full product remains the goal.

| Phase | Scope | Exit gate |
| --- | --- | --- |
| Foundation | Packaged shell, mode switch, collector contracts, transactional storage, consent/coverage UI, app/startup/default/PATH baseline collectors, checkpoints, comparison, text/JSON. | End-to-end manual workflow, deterministic fixture tests, honest partial coverage, and no sensitive-data leaks. This is an engineering baseline, not a permanent four-source cap. |
| E1: Everyday coverage | Store apps, expanded startup/defaults, tasks, services, update history, drivers, audio, displays, device/printer history, timeline, expected-change annotations. | Per-category correctness and permission matrix, native-call isolation, and understandable novice workflows. |
| E2: Advanced coverage | Environment, network, browser extensions, shell integration, selected policies/registry/files/settings, certificates, and optional local listeners. | Per-source consent/privacy review, resource measurement, version compatibility, and no hidden elevation or execution. |
| M: Monitoring | Resource prototype, eligible event subscriptions, optional schedules, tray/startup lifecycle, pause/resume, aggregated notifications. | Owner-approved measurable budget and passing idle/active/OS/GPU profiling. This phase cannot be marked complete on an untested zero-overhead claim. |
| R: Reports and investigation | PDF/CSV/HTML, safe imports, cross-PC report inspection, rule management, richer notes, guided problem windows, and reviewed encrypted bundles. | Format parity, redaction/active-content tests, accessible reports, and truthful correlation language. |
| Release hardening | Large-history performance, recovery/migration, localization, x64/ARM64 qualification, accessibility, Store assets/licensing/certification, and support documentation. | Release acceptance matrix passes for all advertised capabilities; remaining gaps are explicit. |

Phases can progress independently where contracts are stable, but reliability/privacy gates are not optional. If the resource constraint remains unresolved, manual features can proceed and be released with monitoring absent and clearly disclosed. Do not replace the full-product goal with an arbitrary one-feature expansion rule or retain the previous checkpoint-count limit.

Maintain a feature-status register during implementation: planned, prototype, validated manual, validated background, unavailable on this system, or deferred with reason. A checkmark in the roadmap is not evidence of implemented behavior. Scope changes that introduce system mutation, cloud data flows, paid tiers, or relaxed performance constraints require new approval.

## 15. Verification And Release Acceptance

| ID | Scenario | Pass condition |
| --- | --- | --- |
| AC-01 | First capture | Current inventory is not new-change history; all-failed capture cannot become a usable baseline. Consent checklist matches captured sources. |
| AC-02 | Baselines and checkpoints | Explicit baseline changes retain capture time; names/tags/notes/pins survive restart and mode changes; no artificial 20-item restriction. |
| AC-03 | App and startup identity | Version/scope/view and launch-value fixtures are correct; uncertain identities or enabled state are not guessed. |
| AC-04 | Tasks and services | Added/removed/modified configuration is accurate; inaccessible units never create mass removals; runtime state is distinguishable. |
| AC-05 | Updates and drivers | Event IDs deduplicate, pruning never implies uninstall, source event dates are honest, and driver dates are not install times. |
| AC-06 | Defaults, audio, and displays | Effective settings match the tested Windows surfaces; missing endpoint/override and query failure are distinguished; unsupported fields are unknown. |
| AC-07 | Devices and network | Transient presence/configuration distinctions are correct; no network probing, password capture, or default export of persistent identifiers. |
| AC-08 | Environment and PATH | User/system scopes, order, duplicates, references, and hidden-value changes are correct; keys/secrets never leak. |
| AC-09 | Extended sources | Browser/policy/registry/shell/certificate/file/listener adapters honor opt-in scope, limits, and documented version/permission constraints. |
| AC-10 | Partial coverage and recovery | Exact endpoint comparisons retain gaps; timeline recovery uses a previous compatible observation with a widened window; new sources establish baselines. |
| AC-11 | Time and history | A-to-B-to-A differs from net endpoint equality; occurrence versus detection, local boundaries, DST/time-zone changes, and gaps remain clear. |
| AC-12 | Mode round-trip | No payment/restart/rescan or hidden collection; filters, selected evidence, privacy, baseline, and consent remain consistent. |
| AC-13 | Expected/ignored items | Annotation is reversible, rule previews are accurate, evidence is retained, and errors/coverage do not disappear. |
| AC-14 | Monitoring resource gate | Background release requires accepted budgets and measured app/worker/OS CPU, I/O, wakeups, memory, GPU, and battery behavior; rounded 0% is insufficient. |
| AC-15 | Monitor failure and storms | Bounded queues/coalescing, cancellation, overflow gaps, source pause, and no full-source rescan per event are proven. |
| AC-16 | Schedules and lifecycle | Explicit consent, one catch-up, no wake/service surprise, sleep/sign-out gaps, single instance, correct Close behavior, and complete Exit cleanup. |
| AC-17 | Notifications | Eligible results aggregate and deduplicate; user/Windows suppression works; safe lock-screen text and correct activation interval. |
| AC-18 | Report formats | All formats agree on selected evidence/counts/coverage; CSV formulas, HTML injection, PDF metadata, and long values are handled safely. |
| AC-19 | Import and encrypted sharing | Malformed/oversized/active reports are rejected safely; external evidence stays separate; authenticated encryption and key-sharing behavior pass security review. |
| AC-20 | Privacy | Field-level tests cover credentials in names/arguments/values/notes/errors, profile paths, identifiers, hidden fingerprints, and key loss/version mismatch. |
| AC-21 | Storage and recovery | Transaction rollback, interruption, full disk, corruption, migrations, retention anchors, pins, and explicit deletion preserve the documented evidence contract. |
| AC-22 | Accessibility and localization | Both modes and every workflow pass keyboard/Narrator/large-text/high-contrast checks with long translated strings and regional formats. |
| AC-23 | Packaging and permissions | Standard-user/offline use and install/update/reset/uninstall pass on advertised Windows x64/ARM64 targets; managed restrictions remain explicit. |
| AC-24 | Claims and completeness | Listing/help/screenshots match feature status; fixed $0.99 includes all features; no unsupported performance, historical, security, or causal claims. |
| AC-25 | Review-first triage | A mixed fixture shows explained review findings ahead of collapsed routine updates; Other changes and coverage gaps remain visible. A routine app update cannot absorb a new startup entry or protection downgrade, and top-three presentation retains the full review count and access to remaining items. |
| AC-26 | Expectation and uncertainty | Mark expected affects only the selected occurrence by default; missing rule-dependent coverage prevents automatic routine classification; identity/publisher/signature/timing alone cannot produce a safe verdict. Broad rules cannot silently suppress new high-impact protection/launch changes. |
| AC-27 | Scope and standard permissions | First-run scope choice does not capture; every check runs unelevated in both scopes and never shows a UAC prompt. The package declares no `allowElevation` capability, the executable runs `asInvoker`, and an elevated launch is refused. Inaccessible machine sources appear as coverage gaps. Scope/access changes retain separate baselines and prevent false additions/removals. |
| AC-28 | Selected comparison dates | Both modes allow local-date filters and exact saved-snapshot picks, including multiple observations on one day, or a saved observation versus a new check today. Empty dates never invent evidence, saved comparisons never collect, and the selected reference is not replaced by the baseline. Chronology and scope/access guards remain enforced. |
| AC-29 | Readable controls and offline help | Mode controls expose radio semantics; change type, priority, and group counts have meaningful text. Contrast, focus, labels, high-contrast colors, and smaller-window layout are tested. The embedded Helpme.md guide opens offline from Help me/F1 with native formatting, searchable topics, scalable text, and no remote assets or scripts. |

### Test Strategy

Use synthetic fixtures and adapter contracts for every source, with unit tests for normalization, identity, change policies, evidence wording, and redaction. Add SQLite/migration/retention and format/import integration tests. Live changes to startup/tasks/services/associations/audio/network/certificates/policies belong in disposable or explicitly approved test environments, never the developer's normal PC as an incidental validation step.

Capture manual and automated UI evidence for both modes, empty/partial states, long paths, filtering, consent, large text, reports, and recovery. Build a documented Windows/architecture/source-coverage matrix. Include real devices and managed-PC restrictions where virtual machines cannot validate behavior faithfully.

### Performance Qualification

Proposed UI goals, to verify on declared reference hardware: usable shell within 2 seconds, first timeline page/filter response within 500 ms for a 100,000-change indexed history, and responsive progress/cancellation throughout collection. Set active capture CPU/I/O/time/memory limits per collector after profiling rather than promising one duration for a tiny default-app query and a large driver inventory.

Background profiling includes initialization, 30-minute quiet idle, a single relevant event, event storms, full selected-source capture, visible UI, sleep/resume, battery use, and Exit. Count collector worker processes and inspect attributable OS/service/compositor costs. Do not add a continuously updating resource dashboard merely to measure the app. Record baseline observer cost. Initial unit, storage/privacy, collector smoke, and real-window tests exist; the background resource budget and complete release matrix have not been qualified. See [readme.md](readme.md) for exact commands and implemented scope.

## 16. Validation And Remaining Decisions

Recruit novice, enthusiast, developer, support, and accessibility testers throughout the phases. Validate real tasks, not just preference for a feature list. The product's broad scope is approved direction, but commercial demand is not established.

- Novices should establish a baseline and distinguish no history from no changes without coaching.
- Technical users should find the mode switch, compare checkpoints, inspect evidence, and produce useful reports without separate tools for each step.
- Support recipients should understand time uncertainty, redaction, and missing coverage without access to the sender's PC.
- Record real instances where a change helped explain behavior, plus misleading/noisy results and missing-source requests.
- Measure through voluntary research and support/Store feedback, not default telemetry. Price remains $0.99 regardless of feedback.

Decisions still needed before affected capabilities ship:

| Decision | Constraint |
| --- | --- |
| Resource budget | Owner must resolve literal 0% versus measured near-zero idle and bounded active work; expanded scope does not resolve it. |
| Source defaults | Approve the Recommended profile and consent wording; sensitive collectors are opt-in, not enabled by Advanced mode. |
| Platform qualification | Confirm exact Windows/runtime builds, native ARM64 dependencies, and test hardware before advertising coverage. |
| Retention defaults | Approve time/storage options, pin protection, and no-silent-deletion behavior. |
| Store release | Use the supplied ChangeTracker identity; complete branding, signing, privacy/support URLs (Partner Center support contact or website is required), and certification. Do not declare `allowElevation`; it was denied under policy 10.6.3. Repository/remote names and local data paths remain unchanged. |
| Localization | Prioritize languages based on users and review capacity; architecture and accessibility must accommodate them from the start. |
| Portable encrypted reports | Approve a reviewed encryption/key-sharing and untrusted-import design before availability. |

There are no separate editions and no additional payment for Advanced features. Broad functionality remains subject to truthful evidence, explicit user control, and the same privacy and resource requirements in both modes.