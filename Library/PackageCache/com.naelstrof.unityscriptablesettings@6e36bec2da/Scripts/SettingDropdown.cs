using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace UnityScriptableSettings {
    [CreateAssetMenu(fileName = "New Dropdown", menuName = "Unity Scriptable Setting/Dropdown", order = 34)]
    public class SettingDropdown : SettingInt {
        [SerializeField]
        protected string[] dropdownOptions;
        
        public string[] GetDropdownOptions() {
            return dropdownOptions;
        }
        public override void SetValue(int value) {
            value = Mathf.Clamp(value, 0, dropdownOptions.Length-1);
            base.SetValue(value);
        }
        public override void OnValidate() {
            base.OnValidate();
            defaultValue = Mathf.Clamp(defaultValue, 0, dropdownOptions.Length-1);
        }
    }
}
