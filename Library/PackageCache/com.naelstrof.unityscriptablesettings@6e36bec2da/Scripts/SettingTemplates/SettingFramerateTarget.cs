using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UnityScriptableSettings {
[CreateAssetMenu(fileName = "New Framerate Target", menuName = "Unity Scriptable Setting/Framerate Target", order = 51)]
public class SettingFramerateTarget : SettingIntClamped {
    public override void SetValue(int fps) {
        if (fps == max) {
            Application.targetFrameRate = -1;
        } else {
            Application.targetFrameRate = fps;
        }
        base.SetValue(fps);
    }
}

}