using System;
using UnityEngine;

namespace UnityScriptableSettings {
    [CreateAssetMenu(fileName = "New Int Clamped", menuName = "Unity Scriptable Setting/Clamped Int", order = 30)]
    public class SettingIntClamped : SettingInt {
        [SerializeField]
        protected int min;
        [SerializeField]
        protected int max;
        public int GetMin() => min;
        public int GetMax() => max;
        public override void SetValue(int value) {
            value = Mathf.Clamp(value, min, max);
            base.SetValue(value);
        }

        public override void OnValidate() {
            defaultValue = Mathf.Clamp(defaultValue, min, max);
            base.OnValidate();
        }
    }
}