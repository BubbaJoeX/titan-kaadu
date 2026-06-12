# Kaadu Launcher documentation

**Kaadu** is the official Windows launcher for **SWG: Titan**. It handles account login, manifest-based game downloads and updates, first-run setup, and auto-login into `SwgTitan_r.exe`.

| Audience | Document |
|----------|----------|
| Players | [../README.md](../README.md) — install, play, troubleshooting |
| Developers | [architecture.md](architecture.md) — solution layout, services, data flow |
| Build / release | [build-and-deploy.md](build-and-deploy.md) — compile, publish, manifests, SFTP |
| Server operators | [administration.md](administration.md) — CDN layout, manifest workflow, launcher updates |

## Repository location

In the Titan monorepo:

```
D:/titan/support/launcher_redux/
├── Kaadu.slnx                 # Solution (Kaadu + KaaduManifestGenerator)
├── Kaadu/                     # WPF launcher (Kaadu.exe)
├── KaaduManifestGenerator/    # CLI: kaadu-manifest
├── Kaadu.Deploy/              # CLI: upload launcher builds to SFTP
├── base_manifest.json         # Example base-game manifest (local reference)
├── update_manifest.json       # Example update manifest (local reference)
└── docs/                      # This documentation
```

## Quick reference

| Item | Value |
|------|-------|
| Launcher version | Defined in `Kaadu/Models/AppConfiguration.cs` (`LauncherVersion`) |
| Default install path | `C:\Users\Public\Titan\SWG\` |
| Game executable | `SwgTitan_r.exe` |
| Setup executable | `SwgTitanSetup_r.exe` |
| User settings | `%LocalAppData%\Kaadu\settings.json` |
| Local game version | `{InstallPath}\version.json` |
| Base install marker | `{InstallPath}\.base_installed` |
| .NET | 9.0 (launcher: `net9.0-windows`, tools: `net9.0`) |
| Publish target | `win-x64` single-file, self-contained |

## Related Titan paths

| Path | Role |
|------|------|
| `D:/titan/client/` | SwgTitan client source; build output staged to `exe/win32_rel` or `exe/win64_rel` |
| `D:/titan/exe/win32_rel/` | Typical source tree for base/update manifests |
| `D:/titan/support/launcher/` | Earlier TitanLauncher planning docs (superseded for production by launcher_redux) |
