using System;
using UnityEngine;

namespace UnityScriptableSettings {
    [CreateAssetMenu(fileName = "New Float", menuName = "Unity Scriptable Setting/Float", order = 12)]
    public class SettingFloat : Setting {
        public delegate void SettingFloatAction(float newValue);
        public event SettingFloatAction changed;
        [SerializeField]
        protected float defaultValue;
        protected float selectedValue;
        public virtual void SetValue(float value) {
            if (selectedValue == value) {
                return;
            }
            selectedValue = value;
            changed?.Invoke(selectedValue);
        }
        public virtual float GetValue() {
            return selectedValue;
        }

        public override void ResetToDefault() {
            base.ResetToDefault();
            SetValue(defaultValue);
        }

        public override void Save() {
            base.Save();
            PlayerPrefs.SetFloat(name, GetValue());
        }
        public override void Load() {
            base.Load();
            SetValue(PlayerPrefs.GetFloat(name, defaultValue));
        }
    }
}