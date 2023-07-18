using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UnityScriptableSettings {
    
    
[CreateAssetMenu(fileName = "New Graphics Quality", menuName = "Unity Scriptable Setting/Graphics Quality", order = 53)]
public class SettingGraphicsQuality : SettingLocalizedDropdown {
    public override void SetValue(int value) {
        QualitySettings.SetQualityLevel(value);
        Graphics.activeTier = value == 0 ? UnityEngine.Rendering.GraphicsTier.Tier1 : UnityEngine.Rendering.GraphicsTier.Tier3;
        base.SetValue(value);
    }
    public override void Save() {
        PlayerPrefs.SetInt("UnityGraphicsQuality", selectedValue);
    }
    public override void Load() {
        SetValue(PlayerPrefs.GetInt("UnityGraphicsQuality", QualitySettings.GetQualityLevel()));
    }
}

}