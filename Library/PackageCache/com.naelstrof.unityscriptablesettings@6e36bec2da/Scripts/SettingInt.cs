using System;
using UnityEngine;

namespace UnityScriptableSettings {
    [CreateAssetMenu(fileName = "New Int", menuName = "Unity Scriptable Setting/Int", order = 11)]
    public class SettingInt : Setting {
        public delegate void SettingIntAction(int newValue);
        public event SettingIntAction changed;
        [SerializeField]
        protected int defaultValue;
        protected int selectedValue;
        public virtual void SetValue(int value) {
            if (selectedValue == value) {
                return;
            }
            selectedValue = value;
            changed?.Invoke(selectedValue);
        }
        public virtual int GetValue() {
            return selectedValue;
        }

        public override void ResetToDefault() {
            base.ResetToDefault();
            SetValue(defaultValue);
        }

        public override void Save() {
            base.Save();
            PlayerPrefs.SetInt(name, GetValue());
        }
        public override void Load() {
            base.Load();
            SetValue(PlayerPrefs.GetInt(name, defaultValue));
        }
        public void NotifyChange() {
            changed?.Invoke(selectedValue);
        }
    }
}