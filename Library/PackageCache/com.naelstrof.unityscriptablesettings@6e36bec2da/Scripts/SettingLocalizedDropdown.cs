using UnityEngine;
using UnityEngine.Localization;

namespace UnityScriptableSettings {
    [CreateAssetMenu(fileName = "New Localized Dropdown", menuName = "Unity Scriptable Setting/Localized Dropdown", order = 35)]
    public class SettingLocalizedDropdown : SettingInt {
        [SerializeField]
        protected LocalizedString[] dropdownOptions;
        public LocalizedString[] GetLocalizedDropdowns() {
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
