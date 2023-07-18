using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace UnityScriptableSettings {
public class SettingsManager : MonoBehaviour {
    private static SettingsManager instance;
    [SerializeField, SerializeReference]
    private List<Setting> settings;

    private ReadOnlyCollection<Setting> readOnlySettings;
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        readOnlySettings = settings.AsReadOnly();
    }

    public static Coroutine StaticStartCoroutine(IEnumerator routine) {
        return instance.StartCoroutine(routine);
    }

    public static void AddSetting(Setting newSetting) {
        instance.settings.Add(newSetting);
        instance.settings.Sort((a,b)=>String.Compare(a.group.name.ToString(), b.group.name.ToString(), StringComparison.InvariantCulture));
        newSetting.Load();
    }

    public static void RemoveSetting(Setting setting) {
        instance.settings.Remove(setting);
        instance.settings.Sort((a,b)=>String.Compare(a.group.name.ToString(), b.group.name.ToString(), StringComparison.InvariantCulture));
    }

    public static Setting GetSetting(string name) {
        foreach(Setting s in instance.settings) {
            if (s.name == name) {
                return s;
            }
        }
        return null;
    }

    public static ReadOnlyCollection<Setting> GetSettings() {
        return instance.readOnlySettings;
    }

    private void Start() {
        settings.Sort((a,b)=>String.Compare(a.group.name.ToString(), b.group.name.ToString(), StringComparison.InvariantCulture));
        foreach(var setting in settings) {
            setting.Load();
        }
    }
    public static void Save() {
        foreach(var setting in instance.settings) {
            setting.Save();
        }
        PlayerPrefs.Save();
    }
    public static void ResetToDefault(SettingGroup group) {
        foreach(var setting in instance.settings) {
            if (setting.group == group || group == null) {
                setting.ResetToDefault();
            }
        }
    }
    public static void ResetToDefault() {
        foreach(var setting in instance.settings) {
            setting.ResetToDefault();
        }
    }
}

}
