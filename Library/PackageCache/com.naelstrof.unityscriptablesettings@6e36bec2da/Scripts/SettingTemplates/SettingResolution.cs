using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UnityScriptableSettings {
    
[CreateAssetMenu(fileName = "New Resolution", menuName = "Unity Scriptable Setting/Resolution", order = 55)]
public class SettingResolution : SettingDropdown {
    public override void SetValue(int value) {
        Resolution r = Screen.resolutions[Mathf.RoundToInt(value)];
        if (Screen.currentResolution.width != r.width || Screen.currentResolution.height != r.height || Screen.currentResolution.refreshRate != r.refreshRate) {
            Screen.SetResolution(r.width, r.height, Screen.fullScreenMode, r.refreshRate);
        }
        base.SetValue(value);
    }
    public override void Save() {
        // This apparently can report 0x0_0 if the game is minimized or otherwise not displaying??
        //Resolution r = Screen.currentResolution;
        Resolution r = Screen.resolutions[Mathf.RoundToInt(GetValue())];
        PlayerPrefs.SetInt ("Screenmanager Resolution Height", r.height);
        PlayerPrefs.SetInt ("Screenmanager Resolution Width", r.width);
        PlayerPrefs.SetInt ("Screenmanager Refresh Rate", r.refreshRate);
    }
    public override void Load() {
        int height = PlayerPrefs.GetInt ("Screenmanager Resolution Height", Screen.resolutions[0].height);
        int width = PlayerPrefs.GetInt ("Screenmanager Resolution Width", Screen.resolutions[0].width);
        int refreshRate = PlayerPrefs.GetInt ("Screenmanager Refresh Rate", Screen.resolutions[0].refreshRate);
        for(int i=0;i<Screen.resolutions.Length;i++) {
            if (Screen.resolutions[i].width == width && Screen.resolutions[i].height == height && Screen.resolutions[i].refreshRate == refreshRate) {
                selectedValue = i;
                break;
            }
        }
        int count = Screen.resolutions.Length;
        dropdownOptions = new string[count];
        for(int i=0;i<count;i++) {
            dropdownOptions[i] = $"{Screen.resolutions[i].width}x{Screen.resolutions[i].height}_{Screen.resolutions[i].refreshRate}";
        }
    }
}

}