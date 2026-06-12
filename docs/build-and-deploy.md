# Build and deploy

## Prerequisites

- **Windows 10/11 x64**
- **.NET 9 SDK** ([download](https://dotnet.microsoft.com/download))
- **Visual Studio 2022** (optional; 17.8+ with .NET desktop workload)
- SFTP credentials for the Titan CDN (admin deploy only)

## Build the launcher

From `D:/titan/support/launcher_redux/`:

```powershell
dotnet restore Kaadu/Kaadu.csproj
dotnet build Kaadu/Kaadu.csproj -c Release
```

### Publish single-file executable

The project is configured for self-contained, compressed, single-file publish:

```powershell
dotnet publish Kaadu/Kaadu.csproj -c Release -r win-x64 --self-contained true
```

Output directory:

```
Kaadu/bin/Release/net9.0-windows/win-x64/publish/Kaadu.exe
```

Before releasing, bump `LauncherVersion` in `Kaadu/Models/AppConfiguration.cs`.

## Build manifest tool

```powershell
dotnet build KaaduManifestGenerator/KaaduManifestGenerator.csproj -c Release
```

Published CLI name: **`kaadu-manifest`** (assembly name override in csproj).

### Commands

| Command | Purpose |
|---------|---------|
| `generate` | Build manifest from a local directory |
| `sftp-generate` | Build manifest by scanning remote SFTP paths |
| `sftp-upload` | Upload a manifest file to SFTP |
| `sftp-list` | List remote directory |
| `verify` | Verify local files against a manifest |
| `hash` | SHA-256 hash of one file |

Run `kaadu-manifest --help` for full option lists.

### Examples

Generate base game manifest from a staged client tree:

```powershell
kaadu-manifest generate `
  -d "D:\titan\exe\win32_rel" `
  -v "1.0.0" `
  -o base_manifest.json
```

Generate from SFTP (remote base tree):

```powershell
kaadu-manifest sftp-generate `
  -r /sftp/sftpuser/upload/base/ `
  -v "1.0.0" `
  -u admin `
  -p secret `
  -o base_manifest.json
```

Generate update manifest from patch folder:

```powershell
kaadu-manifest generate `
  -d "D:\titan\exe\win32_rel" `
  -v "1.0.1" `
  -o update_manifest.json
```

Verify a local install:

```powershell
kaadu-manifest verify -d "C:\Users\Public\Titan\SWG" -m update_manifest.json
```

Upload manifest to CDN config path:

```powershell
kaadu-manifest sftp-upload `
  -m update_manifest.json `
  -r /sftp/sftpuser/upload/config/update_manifest.json `
  -u admin `
  -p secret
```

### Manifest JSON schema

Manifests share the `UpdateManifest` shape:

```json
{
  "latest_version": "1.0.1",
  "generated_at": "2026-06-09T12:00:00Z",
  "total_size": 1234567890,
  "files": [
    {
      "path": "SwgTitan_r.exe",
      "checksum": "<sha256 hex lowercase>",
      "size": 1048576
    }
  ]
}
```

- **`path`** — relative path under the install root (base) or under `updates/` (patches)
- **`checksum`** — SHA-256 hex digest
- **`latest_version`** — semver string stored locally in `version.json` after sync

Example reference files in the repo root: `base_manifest.json`, `update_manifest.json`.

## Deploy game files to SFTP

Typical operator workflow:

1. Stage a full client tree (e.g. from `client` build + `tre/bundle.ps1` output)
2. Upload contents to SFTP `base/` (initial) or `updates/` (delta files only)
3. Regenerate manifests with `kaadu-manifest`
4. Upload manifests to SFTP `config/`:
   - `base_manifest.json`
   - `update_manifest.json`

See [administration.md](administration.md) for the full CDN directory layout.

## Deploy launcher updates

Build `Kaadu.Deploy` (separate from `Kaadu.slnx`):

```powershell
dotnet build Kaadu.Deploy/Kaadu.Deploy.csproj -c Release
```

Upload a published launcher:

```powershell
Kaadu.Deploy.exe 1.0.2 "Kaadu\bin\Release\net9.0-windows\win-x64\publish\Kaadu.exe" `
  --host test.swgtitan.org `
  --user admin `
  --password secret
```

Or use environment variables: `SFTP_HOST`, `SFTP_USER`, `SFTP_PASSWORD`.

`Kaadu.Deploy` will:

1. Upload all files under the publish directory to `/sftp/sftpuser/upload/launcher/v{version}/`
2. Write `/sftp/sftpuser/upload/config/launcher.json`:

```json
{
  "Version": "1.0.2",
  "Filename": "Kaadu.exe",
  "ReleaseNotes": "Version 1.0.2 release",
  "MinimumVersion": null
}
```

Players receive a prompt on next startup if the remote version is newer.

## Development tips

- **Debug without SFTP:** Point manifests locally and temporarily stub `SftpService`, or use `verify` against a dev install tree.
- **D3D9 launch issues:** Always set `WorkingDirectory` to the install path and `UseShellExecute = false` (already enforced in `UpdateService`).
- **Solution scope:** Add `Kaadu.Deploy` to `Kaadu.slnx` if you want one-click builds for all three projects.

## Version checklist

Before shipping a release:

- [ ] Bump `AppConfiguration.LauncherVersion`
- [ ] Publish `Kaadu.exe` and run `Kaadu.Deploy`
- [ ] Regenerate and upload `update_manifest.json` if game files changed
- [ ] Smoke-test: fresh install, resume download, repair, auto-login, self-update prompt
