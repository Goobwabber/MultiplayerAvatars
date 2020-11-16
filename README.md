# MultiplayerAvatars (PC Only) [![Build](https://github.com/Goobwabber/MultiplayerAvatars/workflows/Build/badge.svg?event=push)](https://github.com/Goobwabber/MultiplayerAvatars/actions?query=workflow%3ABuild)
A Beat Saber mod that adds [CustomAvatars](https://github.com/nicoco007/BeatSaberCustomAvatars) support to the [MultiplayerExtensions](https://github.com/Zingabopp/MultiplayerExtensions) mod. **This is a work in progress which has bugs.**

## Features
* Sends avatar data to other players who have the mod.
* Spawns other player's avatars.
* Scales other player's avatars.

## Installation
MultiplayerAvatars has not been released yet, but you can grab the latest build which is automagically generated. 
1. Ensure you have the [required mods](https://github.com/Goobwabber/MultiplayerAvatars#requirements).
1. Download the `MultiplayerAvatars` file listed under `Artifacts` **[Here](https://github.com/Goobwabber/MultiplayerAvatars/actions?query=workflow%3ABuild+branch%3Amain)** (pick the topmost successful build). 
   * You must be logged into GitHub to download builds from GitHub Actions.
2. Extract the zip file to your Beat Saber game directory (the one `Beat Saber.exe` is in).
   * The `MultiplayerAvatars.dll` (and `MultiplayerAvatars.pdb` if it exists) should end up in your `Plugins` folder (**NOT** the one in `Beat Saber_Data`).
3. **Optional**: Edit `Beat Saber IPA.json` (in your `UserData` folder) and change `Debug` -> `ShowCallSource` to `true`. This will enable BSIPA to get file and line numbers from the `PDB` file where errors occur, which is very useful when reading the log files. This may have a *slight* impact on performance.

## Requirements
Mods without a link can be downloaded from [BeatMods](https://beatmods.com/#/mods) or using Mod Assistant. **Do NOT use any of the DLLs in the `Refs` folder, they have been stripped of code and will not work.**
* [BeatSaberCustomAvatars](https://github.com/nicoco007/BeatSaberCustomAvatars) 5.0.3+
* [MultiplayerExtensions](https://github.com/Zingabopp/MultiplayerExtensions) 0.2.0+
* SiraUtil 2.1.0+

## Contributing
Anyone can feel free to contribute bug fixes or enhancements to MultiplayerAvatars. GitHub Actions for Pull Requests made from GitHub accounts that don't have direct access to the repository will fail. This is normal because the Action requires a `Secret` to download dependencies.
### Building
Visual Studio 2019 with the [BeatSaberModdingTools](https://github.com/Zingabopp/BeatSaberModdingTools) extension is the recommended development environment.
1. Check out the repository
2. Open `MultiplayerAvatars.sln`
3. Right-click the `MultiplayerAvatars` project, go to `Beat Saber Modding Tools` -> `Set Beat Saber Directory`
   * This assumes you have already set the directory for your Beat Saber game folder in `Extensions` -> `Beat Saber Modding Tools` -> `Settings...`
   * If you do not have the BeatSaberModdingTools extension, you will need to manually create a `MultiplayerAvatars.csproj.user` file to set the location of your game install. An example is showing below.
4. The project should now build.

**Example csproj.user File:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <BeatSaberDir>Full\Path\To\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```
