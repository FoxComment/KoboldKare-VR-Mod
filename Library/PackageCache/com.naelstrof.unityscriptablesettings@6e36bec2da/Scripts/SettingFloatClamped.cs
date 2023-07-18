using System;
using UnityEngine;

namespace UnityScriptableSettings {
    [CreateAssetMenu(fileName = "New Clamped Float", menuName = "Unity Scriptable Setting/Clamped Float", order = 32)]
    public class SettingFloatClamped : SettingFloat {
        [SerializeField]
        private float min;
        [SerializeField]
        private float max;
        public float GetMin() => min;
        public float GetMax() => max;
        public override void SetValue(float value) {
            value = Mathf.Clamp(value, min, max);
            base.SetValue(value);
        }

        public override void OnValidate() {
            defaultValue = Mathf.Clamp(defaultValue, min, max);
            base.OnValidate();
        }
    }
}