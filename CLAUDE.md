# ConfigBase

NuGet library collection for reading configuration from JSON, INI, XML, YAML.
Publisher: Tech Tea Studio. Packages pushed automatically on push to `main`.

## Packages

| Package | csproj |
|---|---|
| `TechTeaStudio.Config` | `source/TechTeaStudio.Config/TechTeaStudio.Config.csproj` |
| `TechTeaStudio.Config.Ini` | `source/TechTeaStudio.Config.Ini/TechTeaStudio.Config.Ini.csproj` |
| `TechTeaStudio.Config.Xml` | `source/TechTeaStudio.Config.Xml/TechTeaStudio.Config.Xml.csproj` |
| `TechTeaStudio.Config.Yaml` | `source/TechTeaStudio.Config.Yaml/TechTeaStudio.Config.Yaml.csproj` |

Solution: `source/TechTeaStudio.Config/TechTeaStudio.Config.sln`
Targets: `net8.0;net9.0;net10.0`

## Build & Test

```bash
dotnet build source/TechTeaStudio.Config/TechTeaStudio.Config.sln
dotnet test source/TechTeaStudio.Config/TechTeaStudio.Config.sln
```

## Release flow

1. Bump `<Version>` in each changed `.csproj`
2. Commit and push to `main`
3. CI builds, packs, and pushes to nuget.org automatically (`--skip-duplicate` is set)

**Never push to nuget.org manually.**

## Commit Convention

Format: `vX.Y.Z <description>` — see global CLAUDE.md.
Version is 3-part (`X.Y.Z`). Bump the same version across all packages being changed.
**Update `<Version>` in the relevant `.csproj` files before committing.**
