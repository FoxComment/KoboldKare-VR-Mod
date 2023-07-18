using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UnityScriptableSettings {
[CreateAssetMenu(fileName = "New Fullscreen Mode", menuName = "Unity Scriptable Setting/Fullscreen Mode", order = 52)]
public class SettingFullscreenMode : SettingLocalizedDropdown {
    public override void SetValue(int value) {
        switch(value) {
            case 0: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 2: Screen.fullScreenMode = FullScreenMode.MaximizedWindow; break;
            case 3: Screen.fullScreenMode = FullScreenMode.Windowed; break;
        }
        base.SetValue(value);
    }
    public override void Save() {
        PlayerPrefs.SetInt("Screenmanager Fullscreen mode", (int)Screen.fullScreenMode);
    }
    public override void Load() {
        SetValue(PlayerPrefs.GetInt("Screenmanager Fullscreen mode", (int)Screen.fullScreenMode));
    }
}
}