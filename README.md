# PointSaber
A party mode leaderboard for Beat Saber

Scores can be viewed at https://www.pointsaber.com/

## Installation

### Dependencies:
* IPA Modified Installation of Beat Saber (If you already have other mods installed via Beat Saber Mod Installer then you're probably fine)
* `Harmony0.dll` available to Beat Saber
    * Steam: `\steamapps\common\Beat Saber\Beat Saber_Data\Managed\0Harmony.dll`
    * Oculus: `\Oculus Apps\Software\hyperbolic-magnetism-beat-saber\Beat Saber_Data\Managed\0Harmony.dll`
    
  
### Instructions
* Download the dll for your platform (steam or oculus) from the [releases page](https://github.com/hoovercj/beatsaber-leaderboard-plugin/releases).
* Copy it to your plugins directory:
    * Steam: `\steamapps\common\Beat Saber\Plugins`
    * Oculus: `\Oculus Apps\Software\hyperbolic-magnetism-beat-saber\Plugins`

### Configuration
By default, the game will upload scores to `http://www.pointsaber.com/#/<YOUR STEAM OR OCULUS ID>`. If you'd like to customize the URL that your scores are available at, modify `modprefs.ini` to have the lines:
    
* Steam: `\steamapps\common\Beat Saber\UserData`
* Oculus: `\Oculus Apps\Software\hyperbolic-magnetism-beat-saber\UserData`

```
[PointSaber]
LeaderboardName=<YOUR NAME HERE>
```

After running the game and completing a song, you'll be able to see your scores at `http://www.pointsaber.com/#/<YOUR NAME HERE>`

### Note:
I have only tested this on Steam.
