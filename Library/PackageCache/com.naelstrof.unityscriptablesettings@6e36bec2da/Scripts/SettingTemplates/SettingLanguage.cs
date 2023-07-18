using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UnityScriptableSettings {
    
[CreateAssetMenu(fileName = "New Language Setting", menuName = "Unity Scriptable Setting/Language", order = 54)]
public class SettingLanguage : SettingDropdown {
    public override void SetValue(int value) {
        SettingsManager.StaticStartCoroutine(ChangeLanguage(value));
    }
    private IEnumerator ChangeLanguage(int value) {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value];
    }

    public override int GetValue() {
        // Ignore whatever setting we have, and just try our best to return what is reported from LocalizationSettings.
        for(int i=0;i<LocalizationSettings.AvailableLocales.Locales.Count;i++) {
            if (LocalizationSettings.AvailableLocales.Locales[i].name == LocalizationSettings.SelectedLocale.name) {
                return i;
            }
        }
        return 0;
    }

    public override void Save() {
        // Handled by PlayerPrefLocaleSelector
    }

    public override void Load() {
        bool foundLocaleLoader = false;
        foreach (var loader in LocalizationSettings.StartupLocaleSelectors) {
            if (loader is PlayerPrefLocaleSelector) {
                foundLocaleLoader = true;
                break;
            }
        }

        if (!foundLocaleLoader) {
            Debug.LogError(
                "Language loader didn't find a PlayerPrefLocaleSelector in the StartupLocaleSelectors.\n" +
                " You should change the Project Settings to have it in order for it to load properly.");
        }

        SettingsManager.StaticStartCoroutine(OverrideDropdownWithLanguages());
    }
    private IEnumerator OverrideDropdownWithLanguages() {
        yield return LocalizationSettings.InitializationOperation;
        dropdownOptions = new string[LocalizationSettings.AvailableLocales.Locales.Count];
        for(int i=0;i<LocalizationSettings.AvailableLocales.Locales.Count;i++) {
            dropdownOptions[i] = LocalizationSettings.AvailableLocales.Locales[i].name;
        }
    }
}

}