using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityScriptableSettings {
[CreateAssetMenu(fileName = "AmbientOcclusionSetting", menuName = "Unity Scriptable Setting/KoboldKare/Ambient Occlusion", order = 1)]
public class AmbientOcclusionSetting : SettingLocalizedDropdown {
    public UniversalRendererData forwardRenderer;
    public override void SetValue(int value) {
        foreach(var feature in forwardRenderer.rendererFeatures) {
            if (feature.name.Contains("AmbientOcclusion") ) {
                feature.SetActive(value != 0);
                break;
            }
        }
        base.SetValue(value);
    }
}

}