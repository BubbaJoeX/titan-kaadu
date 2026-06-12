# Server administration

Guide for operators hosting the Kaadu CDN and publishing game updates for SWG: Titan.

For player-facing help, see [../README.md](../README.md).

## CDN directory layout

Kaadu expects this structure on the SFTP server (defaults in `AppConfiguration.cs`):

```
/sftp/sftpuser/upload/
├── base/                         # Full base game files (first install)
│   ├── SwgTitan_r.exe
│   ├── SwgTitanSetup_r.exe
│   ├── *.tre
│   ├── *.dll
│   └── ...
├── updates/                      # Patch files (same relative paths as install root)
│   └── ...
├── config/
│   ├── base_manifest.json        # Lists every base/ file + SHA-256
│   ├── update_manifest.json      # Lists patch files + latest_version
│   └── launcher.json             # Current Kaadu.exe version metadata
└── launcher/
    └── v{version}/               # Published launcher build per version
        └── Kaadu.exe
```

Path constants in code:

| Constant | Remote path |
|----------|-------------|
| `BaseGamePath` | `/sftp/sftpuser/upload/base/` |
| `UpdatesPath` | `/sftp/sftpuser/upload/updates/` |
| `ConfigPath` | `/sftp/sftpuser/upload/config/` |
| `LauncherVersionPath` | `.../config/launcher.json` |
| `LauncherBinariesPath` | `/sftp/sftpuser/upload/launcher/` |

## Publishing a base game release

Use this when standing up a new shard or resetting the baseline install.

1. Copy a complete, tested client tree to SFTP `base/` (from `D:/titan/exe/win32_rel/` or your release bundle).
2. Generate manifest:

   ```powershell
   kaadu-manifest sftp-generate -r /sftp/sftpuser/upload/base/ -v "1.0.0" -u USER -p PASS -o base_manifest.json
   ```

   Or generate locally and upload:

   ```powershell
   kaadu-manifest generate -d "D:\release\client" -v "1.0.0" -o base_manifest.json
   kaadu-manifest sftp-upload -m base_manifest.json -r /sftp/sftpuser/upload/config/base_manifest.json -u USER -p PASS
   ```

3. Confirm `config/base_manifest.json` is readable by the SFTP user Kaadu uses at runtime.

Players without `.base_installed` will download every file listed in the base manifest.

## Publishing a patch

1. Upload changed files to SFTP `updates/` using the **same relative paths** as the client install root (e.g. `SwgTitan_r.exe`, `data/foo.tre`).
2. Regenerate `update_manifest.json` with a new `latest_version`.
3. Upload to `config/update_manifest.json`.

On next launch, Kaadu compares `version.json` to `latest_version`. If they differ, it downloads files from `updates/` that are missing or fail hash check.

### Patch-only vs full re-list

The update manifest typically lists **all files you want verified** at the current version, not only deltas. Files unchanged on disk are skipped when SHA-256 matches.

## Launcher updates

1. Build and publish Kaadu (see [build-and-deploy.md](build-and-deploy.md)).
2. Run `Kaadu.Deploy.exe {version} {path/to/Kaadu.exe}`.
3. Optionally set `MinimumVersion` in `launcher.json` to force upgrades.

Remote `launcher.json` fields:

| Field | Purpose |
|-------|---------|
| `Version` | Latest launcher semver |
| `Filename` | Entry executable name (default `Kaadu.exe`) |
| `ReleaseNotes` | Shown in the update dialog |
| `MinimumVersion` | If local version is older, update is mandatory |

## Web endpoints

Kaadu authenticates against HTTPS endpoints (also in `AppConfiguration.cs`):

| Endpoint | Purpose |
|----------|---------|
| `https://swgtitan.org/auth.php` | Username/password validation |
| `https://swgtitan.org/admin_check.php` | Admin level for Admin panel visibility |

Game connection uses `GameLoginServerAddress` (default `test.swgtitan.org`) and port `44453` in launch arguments.

These URLs are compile-time constants today; fork operators must change `AppConfiguration.cs` and rebuild for their infrastructure.

## Admin panel (in launcher)

Users with admin level from `admin_check.php` see **Admin** and **Console** in the launcher header.

`AdminViewModel` exposes operations such as:

- Force update check / download
- Repair (re-verify all manifest files)
- Manifest generation helpers (uses local paths + logging)

Use the admin console for advanced diagnostics during CDN or auth incidents.

## Troubleshooting CDN issues

| Player symptom | Likely cause | Action |
|----------------|--------------|--------|
| "Base game manifest not found" | Missing or unreadable `config/base_manifest.json` | Upload manifest; check SFTP ACLs |
| Files re-download every launch | Hash mismatch (wrong manifest vs files) | Regenerate manifest from actual CDN bytes |
| "Update server unavailable" | SFTP down or blocked | Check host/firewall; launcher still allows play if base is installed |
| Login works, download fails | SFTP credentials or path drift | Verify `SftpHost` matches CDN; test with `kaadu-manifest sftp-list` |
| Launcher never self-updates | Stale `launcher.json` or version not bumped | Redeploy with `Kaadu.Deploy` |

## Resetting a player install

Operators can advise players to:

1. Delete `{InstallPath}/.base_installed` to force base re-verify
2. Delete `{InstallPath}/version.json` to force version re-sync
3. Use launcher **Repair** if available
4. Change install directory via **Directory** (does not move files; re-download if empty)

## Security notes

- SFTP credentials are embedded in the shipped launcher for read-only CDN access (via `SftpService` — review source before production hardening).
- Auto-login stores the password in memory and passes it on the game command line.
- Auth `secretKey` is hardcoded in `AuthService.cs`; rotate on the server side if compromised.

## Related documentation

- [architecture.md](architecture.md) — internal service design
- [build-and-deploy.md](build-and-deploy.md) — CLI tools and publish commands
- [../README.md](../README.md) — player install guide
