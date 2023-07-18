using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UnityScriptableSettings {

[CreateAssetMenu(fileName = "New Audio Parameter", menuName = "Unity Scriptable Setting/Audio", order = 50)]
public class SettingAudio : SettingFloatClamped {
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private string audioExposedParamName;
    [SerializeField]
    private bool isVolumeParameter = true;
    public override void SetValue(float value) {
        if (isVolumeParameter) {
            audioMixer.SetFloat(audioExposedParamName, Mathf.Log(Mathf.Max(value,0.01f))*20f);
        } else {
            audioMixer.SetFloat(audioExposedParamName, value);
        }
        base.SetValue(value);
    }
}

}