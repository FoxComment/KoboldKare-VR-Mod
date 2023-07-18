using System;
using UnityEngine;

namespace UnityScriptableSettings {
    [CreateAssetMenu(fileName = "New String", menuName = "Unity Scriptable Setting/String", order = 13)]
    public class SettingString : Setting {
        public delegate void SettingStringAction(string newValue);
        public event SettingStringAction changed;
        [SerializeField]
        protected string defaultValue;
    
        protected string selectedValue;
        public virtual void SetValue(string value) {
            if (selectedValue == value) {
                return;
            }
            selectedValue = value;
            changed?.Invoke(selectedValue);
        }
        public virtual string GetValue() {
            return selectedValue;
        }

        public override void ResetToDefault() {
            base.ResetToDefault();
            SetValue(defaultValue);
        }

        public override void Save() {
            base.Save();
            PlayerPrefs.SetString(name, GetValue());
        }
        public override void Load() {
            base.Load();
            SetValue(PlayerPrefs.GetString(name, defaultValue));
        }
    }
}