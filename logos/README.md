# ChangeTracker Store Artwork

All nine exports are flat in this folder (no subfolders) and every one visibly includes the **ChangeTracker** name, matching a fixed reference size list: 44, 71, 150, 300, 512, 1024, 1080x1080 box art, 720x1080 poster, and 1920x1080 wide.

## Files

| File | Pixels | Background | Use |
| --- | --- | --- | --- |
| [ChangeTracker-Logo-44x44.png](ChangeTracker-Logo-44x44.png) | 44 x 44 | Transparent | MSIX manifest `Square44x44Logo` (app-list icon) and the package `Properties/Logo`. Also embedded as the tray icon source. |
| [ChangeTracker-Logo-71x71.png](ChangeTracker-Logo-71x71.png) | 71 x 71 | Transparent | MSIX manifest `uap:DefaultTile/Square71x71Logo` (small tile). |
| [ChangeTracker-Logo-150x150.png](ChangeTracker-Logo-150x150.png) | 150 x 150 | Transparent | MSIX manifest `Square150x150Logo` (medium tile). |
| [ChangeTracker-Logo-300x300.png](ChangeTracker-Logo-300x300.png) | 300 x 300 | Transparent | General 1:1 app tile/icon source, e.g. for a Partner Center app tile upload. |
| [ChangeTracker-Logo-512x512.png](ChangeTracker-Logo-512x512.png) | 512 x 512 | Transparent | General-purpose high-resolution source icon. |
| [ChangeTracker-Logo-1024x1024.png](ChangeTracker-Logo-1024x1024.png) | 1024 x 1024 | Transparent | Master source icon at the largest common app-icon size. |
| [ChangeTracker-BoxArt-1080x1080.png](ChangeTracker-BoxArt-1080x1080.png) | 1080 x 1080 | Brand (`#F1F7F6`) | 1:1 box art, e.g. for a Partner Center or MSI/EXE listing box-art field. |
| [ChangeTracker-Poster-720x1080.png](ChangeTracker-Poster-720x1080.png) | 720 x 1080 | Brand (`#F1F7F6`) | 2:3 poster art. |
| [ChangeTracker-Logo-1920x1080.png](ChangeTracker-Logo-1920x1080.png) | 1920 x 1080 | Brand (`#F1F7F6`) | 16:9 wide lockup (mark + wordmark side by side), e.g. for a social/repo banner. |

The six square `Logo-*` sizes keep a transparent background so Windows can composite its own tile/taskbar background behind them, matching the manifest's `BackgroundColor="transparent"`. Box art, poster, and the wide lockup use an opaque brand background instead, since they stand alone as listing/banner images. All images are 8-bit-per-channel RGBA PNG at 96 DPI, under the Store's 50-MB image limit; pixel dimensions, not DPI metadata, determine upload size.

**Note on the 1920x1080 file:** earlier revisions of this folder kept a dedicated, text-free "Super Hero" variant at this size specifically because Microsoft's Partner Center guidance prohibits text on that promotional field. This export now always carries the ChangeTracker wordmark, so it is a general-purpose wide banner, not a ready-made Super Hero upload; produce a separate text-free crop if you need that specific Partner Center field.

**Note on the tray icon:** the running app's system tray icon is embedded from `ChangeTracker-Logo-44x44.png` (see `src/PCChangeTracker.App/PCChangeTracker.App.csproj`). Windows shrinks it well below 44px for the actual tray, so the wordmark is not legible there; only the mark reads at that size. This is a cosmetic tray-icon limitation, not a packaging defect.

## Build And Upload

The MSIX package references three of these files directly by name from the manifest (`ChangeTracker-Logo-44x44.png` for both `Square44x44Logo` and the package `Properties/Logo`, `ChangeTracker-Logo-71x71.png` for the small tile, `ChangeTracker-Logo-150x150.png` for the medium tile); `packaging/Build-Package.ps1` copies exactly those into the package's `Assets/` folder. The remaining sizes are general-purpose source/listing images, not additional manifest-required files.

A desktop Store submission also needs real app screenshots, listing metadata, privacy/support information, appropriate signing, restricted-capability review, and certification; none of that is established by these logo files alone. Nothing here has been uploaded or certified, and no release installer was rebuilt.

## Reproduce And Validate

Run from the repository root using PowerShell 7 on Windows; no external graphics package is needed:

```powershell
./logos/Generate-Logos.ps1 -ValidateOnly
./logos/Generate-Logos.ps1 -Overwrite
```

Generation renders native bitmaps and downsamples to the exact export size. Existing images are not overwritten unless requested. Validation checks the exact 9-file flat set, PNG signature/channel format, exact dimensions, transparent corners for the six `Logo-*` icon sizes, and nonblank artwork. Generation also rejects a title that does not fit its safe area (stacked layout) or does not fit beside the mark (the 1920x1080 wide layout).

## Official Guidance

Requirements checked on 2026-09-20:

- [MSIX screenshots, Store logos, and other artwork](https://learn.microsoft.com/windows/apps/publish/publish-your-app/msix/screenshots-and-images).
- [Construct Windows app icons: sizes, scale factors, and theme variants](https://learn.microsoft.com/en-us/windows/apps/design/iconography/app-icon-construction).
- [MSI/EXE Store screenshots and logos](https://learn.microsoft.com/windows/apps/publish/publish-your-app/msi/screenshots-and-images).