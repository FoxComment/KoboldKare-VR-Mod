using System;
using UnityEngine;

namespace UnityScriptableSettings {
    
[CreateAssetMenu(fileName = "New Vsync", menuName = "Unity Scriptable Setting/VSync", order = 56)]
public class SettingVSync : SettingLocalizedDropdown {
    //[Header("VSync can be from 0 to 4, being Off, Single Buffered, Double Buffered, Triple Buffered, and Quad buffered. I usually just put two localized strings Off, and On.")]
    public override void SetValue(int value) {
        QualitySettings.vSyncCount = Mathf.Clamp(Mathf.RoundToInt(value),0,4);
        base.SetValue(value);
    }
}

}