# AccSaber Plugin
Beat Saber PC plugin for AccSaber leaderboards.

## Compatibility
This is a Beat Saber PC mod. It does not target Quest standalone.
This fork targets Beat Saber `1.40.8`.
It uses the AccSaber Reloaded API at `https://api.accsaberreloaded.com/v1` instead of the legacy `https://api.accsaber.com` endpoints.

Minimum runtime dependencies for this branch:
* `BSIPA` `4.3.6+`
* `BeatSaberMarkupLanguage` `1.12.5+`
* `SiraUtil` `3.2.1+`
* `LeaderboardCore` `1.7.0+`
* `SongCore` `3.15.3+`

## Current Behavior
* The AccSaber tab stays visible even when the selected map is not ranked on AccSaber Reloaded.
* Ranked map lookup is live by song hash and difficulty against AccSaber Reloaded.
* Player cards use Reloaded profile data, including HMD when the API provides it.

## Release Note
Ported to Beat Saber `1.40.8` and migrated to the AccSaber Reloaded API.

## Reporting Issues
* The best way to report issues is to click on the `Issues` tab at the top of the GitHub page. This allows any contributor to see the problem and attempt to fix it, and others with the same issue can contribute more information. **Please try the troubleshooting steps before reporting the issues listed there. Please only report issues after using the latest build, your problem may have already been fixed.**
* Include in your issue:
  * A detailed explanation of your problem (you can also attach videos/screenshots)
  * **Important**: The log file from the game session the issue occurred (restarting the game creates a new log file).
    * The log file can be found at `Beat Saber\Logs\_latest.log` (`Beat Saber` being the folder `Beat Saber.exe` is in).
* If you ask for help on Discord, at least include your `_latest.log` file in your help request.

## Contributing
Anyone can feel free to contribute bug fixes or enhancements to the AccSaber plugin! Fork, make your changes and pull request it!

### Building
Any recent Windows MSBuild environment that can build `.NET Framework 4.8` projects should work. `dotnet build` on Windows also works.

1. Check out the repository
2. Set `BeatSaberDir` to a valid Beat Saber `1.40.8` install, or provide extracted refs in `Refs`
3. Build `AccSaber.sln`

Example `dotnet` build:
```powershell
dotnet build .\AccSaber.sln -c Release /p:BeatSaberDir="C:\Path\To\Beat Saber"
```

If you prefer Visual Studio:
1. Open `AccSaber.sln`
2. Right-click the `AccSaber` project, go to `Beat Saber Modding Tools` -> `Set Beat Saber Directory`
  * This assumes you have already set the directory for your Beat Saber game folder in `Extensions` -> `Beat Saber Modding Tools` -> `Settings...`
  * If you do not have the BeatSaberModdingTools extension, you will need to manually create an `AccSaber.csproj.user` file to set the location of your game install. An example is showing below.
3. The project should now build.

**Example csproj.user File:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <BeatSaberDir>Full\Path\To\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```

## License
This project is licensed under the GNU GPL v3.0 License - see the [LICENSE](LICENSE) file for details.
