using System;
using UnityEngine;
using UnityEngine.Localization;

namespace UnityScriptableSettings {
    public class Setting : ScriptableObject {
        [Tooltip("Label of the option.")]
        public LocalizedString localizedName;
        [Tooltip("Name of the group that the setting belongs to (audio, graphics, gameplay...")]
        public SettingGroup group;
        public virtual void ResetToDefault() { }
        public virtual void Save() { }
        public virtual void Load() { }
        public virtual void OnValidate() { }
    }
}