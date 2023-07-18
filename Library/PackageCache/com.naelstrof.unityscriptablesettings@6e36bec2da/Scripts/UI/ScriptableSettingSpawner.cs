using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace UnityScriptableSettings {
public class ScriptableSettingSpawner : MonoBehaviour {
    public delegate void FinishSpawningAction();
    public FinishSpawningAction doneSpawning;
    public enum NavigationMode {
        Auto,
        Override
    }
    public NavigationMode navigationMode;
    [Tooltip("If navigation mode is set to override, selecting right (while not on a slider) will select this object.")]
    public Selectable rightSelect;
    [Tooltip("If navigation mode is set to override, selecting left (while not on a slider) will select this object.")]
    public Selectable leftSelect;
    [Tooltip("If navigation mode is set to override, overflowing off the bottom of the list will select this object. Leaving it null will cause it to loop.")]
    public Selectable downSelect;
    [Tooltip("If navigation mode is set to override, overflowing off the top of the list will select this object. Leaving it null will cause it to loop.")]
    public Selectable upSelect;
    public GameObject slider;
    public GameObject dropdown;
    public GameObject textInput;
    public GameObject groupTitle;
    public List<GameObject> titles = new List<GameObject>();
    private Dictionary<Setting,Slider> sliders = new Dictionary<Setting, Slider>();
    private Dictionary<Setting,TMP_Dropdown> dropdowns = new Dictionary<Setting, TMP_Dropdown>();
    private Dictionary<Setting,TMP_InputField> textInputs = new Dictionary<Setting, TMP_InputField>();
    private bool ready;
    [SerializeField]
    private SettingGroup targetGroup;
    Selectable GetSelectable(Setting option) {
        if (sliders.ContainsKey(option)) {
            return sliders[option];
        }
        if (dropdowns.ContainsKey(option)) {
            return dropdowns[option];
        }
        if (textInputs.ContainsKey(option)) {
            return textInputs[option];
        }
        return null;
    }

    private void CleanUp() {
        foreach (var title in titles) {
            Destroy(title);
        }
        foreach (var pair in sliders) {
            Destroy(pair.Value);
        }
        foreach (var pair in dropdowns) {
            Destroy(pair.Value);
        }
        foreach (var pair in textInputs) {
            Destroy(pair.Value);
        }
    }

    private int mod(int x, int m) { return (x%m + m)%m; }
    public IEnumerator WaitUntilReadyThenStart() {
        if (ready) {
            yield break;
        }
        CleanUp();
        yield return LocalizationSettings.InitializationOperation;
        yield return null;
        SettingGroup currentGroup = null;
        foreach(Setting option in SettingsManager.GetSettings()) {
            if (targetGroup != null && option.group != targetGroup) {
                continue;
            }
            if (currentGroup != option.group || currentGroup == null) {
               // CreateTitle(option.group.localizedName);
                currentGroup = option.group;
            }

            if (option is SettingDropdown dropdown) {
                CreateDropDown(dropdown);
                dropdown.changed += (o) => {
                    var dropLookup = dropdowns[option];
                    List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();
                    foreach(string str in dropdown.GetDropdownOptions()) {
                        data.Add(new TMP_Dropdown.OptionData(str));
                    }
                    dropLookup.ClearOptions();
                    dropLookup.options = data;
                    dropLookup.SetValueWithoutNotify(o);
                };
                continue;
            }

            if (option is SettingLocalizedDropdown localizedDropdown) {
                CreateDropDown(localizedDropdown);
                localizedDropdown.changed += (o) => { dropdowns[option].SetValueWithoutNotify(o); };
                continue;
            }

            if (option is SettingFloatClamped floatSlider) {
                CreateSlider(option);
                floatSlider.changed += (o) => {sliders[option].SetValueWithoutNotify(o);};
                continue;
            }
            
            if (option is SettingIntClamped intSlider) {
                CreateSlider(option);
                intSlider.changed += (o) => {sliders[option].SetValueWithoutNotify(o);};
                continue;
            }

            if (option is SettingInt justInt) {
                CreateStringInput(justInt);
                justInt.changed += (o) => { textInputs[option].SetTextWithoutNotify(o.ToString()); };
                continue;
            }
            if (option is SettingFloat justFloat) {
                CreateStringInput(justFloat);
                justFloat.changed += (o) => { textInputs[option].SetTextWithoutNotify(o.ToString()); };
                continue;
            }
            if (option is SettingString justString) {
                CreateStringInput(justString);
                justString.changed += (o) => { textInputs[option].SetTextWithoutNotify(o); };
                continue;
            }
        }
        if (navigationMode == NavigationMode.Override) {
            int startRange = -1;
            int endRange = -1;
            if (targetGroup != null) {
                for(int i=0;i<SettingsManager.GetSettings().Count;i++) {
                    if (targetGroup == SettingsManager.GetSettings()[i].group && startRange == -1) {
                        startRange = i;
                    }
                    if (targetGroup == SettingsManager.GetSettings()[i].group) {
                        endRange = i+1;
                    }
                }
            } else {
                startRange = 0;
                endRange = SettingsManager.GetSettings().Count;
            }
            int len = endRange-startRange;
            // Set up the navigation.
            for (int i=startRange;i<endRange;i++) {
                var option = SettingsManager.GetSettings()[i];
                int nextOptionIndex = ((i+1-startRange)%len)+startRange;
                int prevOptionIndex = mod(i-1-startRange,len)+startRange;
                var nextOption = SettingsManager.GetSettings()[nextOptionIndex];
                var prevOption = SettingsManager.GetSettings()[prevOptionIndex];
                Navigation nav = GetSelectable(option).navigation;
                if (downSelect == null) {
                    nav.selectOnDown = GetSelectable(nextOption);
                } else {
                    nav.selectOnDown = nextOptionIndex == startRange ? downSelect : GetSelectable(nextOption);
                }
                if (upSelect == null) {
                    nav.selectOnUp = GetSelectable(prevOption);
                } else {
                    nav.selectOnUp = prevOptionIndex == endRange ? upSelect : GetSelectable(prevOption);
                }
                if (!sliders.ContainsKey(option)) {
                    if (rightSelect != null) {
                        nav.selectOnRight = rightSelect;
                    }
                    if (leftSelect != null) {
                        nav.selectOnLeft = leftSelect;
                    }
                }
                nav.mode = Navigation.Mode.Explicit;
                GetSelectable(option).navigation = nav;
            }
        }

        ready = true;
        doneSpawning?.Invoke();
        LocalizationSettings.SelectedLocaleChanged += StringChanged;
    }

    private void Awake() {
        ready = false;
    }

    void OnEnable() {
        StartCoroutine(WaitUntilReadyThenStart());
        StartCoroutine(WaitAndThenSelect());
    }
    private IEnumerator WaitAndThenSelect() {
        yield return new WaitUntil(()=>ready);
        Setting topOption = SettingsManager.GetSettings()[0];
        if (sliders.ContainsKey(topOption)) {
            sliders[topOption].Select();
        }
        if (dropdowns.ContainsKey(topOption)) {
            dropdowns[topOption].Select();
        }
        if (textInputs.ContainsKey(topOption)) {
            textInputs[topOption].Select();
        }
    }
    public void CreateTitle(LocalizedString group) {
        GameObject title = GameObject.Instantiate(groupTitle, Vector3.zero, Quaternion.identity);
        title.transform.SetParent(this.transform);
        title.transform.localScale = Vector3.one; 
        title.transform.localPosition = Vector3.zero;
        title.transform.localRotation = Quaternion.Euler(Vector3.zero);

            foreach ( TMP_Text t in title.GetComponentsInChildren<TMP_Text>()) {
            if (t.name == "Label") {
                t.text = group.GetLocalizedString();
            }
        }
        title.GetComponentInChildren<LocalizeStringEvent>().StringReference = group;
        titles.Add(title);
    }
    private void CreateSlider(Setting option) {
        GameObject s = GameObject.Instantiate(slider, Vector3.zero, Quaternion.identity);
        s.transform.SetParent(this.transform);
        s.transform.localScale = Vector3.one;
        s.transform.localPosition = Vector3.zero;
        s.transform.localRotation = Quaternion.Euler(Vector3.zero);

        foreach ( TMP_Text t in s.GetComponentsInChildren<TMP_Text>()) {
            if (t.name == "Label") {
                t.text = option.localizedName.GetLocalizedString();
                t.GetComponent<LocalizeStringEvent>().StringReference = option.localizedName;
            }
            if (option is SettingIntClamped intClamped) {
                if (t.name == "Min") {
                    t.text = intClamped.GetMin().ToString();
                } else if (t.name == "Max") {
                    t.text = intClamped.GetMax().ToString();
                }
            } else if (option is SettingFloatClamped floatClamped) {
                if (t.name == "Min") {
                    t.text = floatClamped.GetMin().ToString();
                } else if (t.name == "Max") {
                    t.text = floatClamped.GetMax().ToString();
                }
            }
        }
        Slider slid = s.GetComponentInChildren<Slider>();
        if (option is SettingIntClamped intClampedA) {
            slid.minValue = intClampedA.GetMin();
            slid.maxValue = intClampedA.GetMax();
            slid.SetValueWithoutNotify(intClampedA.GetValue());
            slid.wholeNumbers = true;
            slid.onValueChanged.AddListener((newValue)=>{intClampedA.SetValue(Mathf.RoundToInt(newValue)); });
        } else if (option is SettingFloatClamped floatClampedA) {
            slid.minValue = floatClampedA.GetMin();
            slid.maxValue = floatClampedA.GetMax();
            slid.SetValueWithoutNotify(floatClampedA.GetValue());
            slid.wholeNumbers = false;
            slid.onValueChanged.AddListener(floatClampedA.SetValue);
        }
        sliders.Add(option, slid);
    }
    private void CreateStringInput(Setting option) {
        GameObject d = GameObject.Instantiate(textInput, Vector3.zero, Quaternion.identity);
        d.transform.SetParent(this.transform); 
        d.transform.localScale = Vector3.one;
        d.transform.localPosition = Vector3.zero;
        d.transform.localRotation = Quaternion.Euler(Vector3.zero);

            foreach ( TMP_Text t in d.GetComponentsInChildren<TMP_Text>()) {
            if (t.name == "Label") {
                //t.text = o.type.ToString();
                t.text = option.localizedName.GetLocalizedString();
                t.GetComponent<LocalizeStringEvent>().StringReference = option.localizedName;
            }
        }
        TMP_InputField inputField = d.GetComponentInChildren<TMP_InputField>();
        if (option is SettingInt intSetting) {
            inputField.SetTextWithoutNotify(intSetting.GetValue().ToString());
            inputField.onSubmit.AddListener( (s) => {
                if (int.TryParse(s, out int v)) {
                    intSetting.SetValue(v);
                } else {
                    inputField.SetTextWithoutNotify(intSetting.GetValue().ToString());
                }
            });
            inputField.onDeselect.AddListener((s) => {
                if (int.TryParse(s, out int v)) {
                    intSetting.SetValue(v);
                } else {
                    inputField.SetTextWithoutNotify(intSetting.GetValue().ToString());
                }
            });
        } else if (option is SettingFloat floatSetting) {
            inputField.SetTextWithoutNotify(floatSetting.GetValue().ToString());
            inputField.onSubmit.AddListener((s) => {
                if (float.TryParse(s, out float v)) {
                    floatSetting.SetValue(v);
                } else {
                    inputField.SetTextWithoutNotify(floatSetting.GetValue().ToString());
                }
            });
            inputField.onDeselect.AddListener((s) => {
                if (float.TryParse(s, out float v)) {
                    floatSetting.SetValue(v);
                } else {
                    inputField.SetTextWithoutNotify(floatSetting.GetValue().ToString());
                }
            });
        } else if (option is SettingString settingString) {
            inputField.SetTextWithoutNotify(settingString.GetValue());
            inputField.onSubmit.AddListener((s) => {
                settingString.SetValue(s);
            });
            inputField.onDeselect.AddListener((s) => {
                settingString.SetValue(s);
            });
        }
        textInputs.Add(option, inputField);
    }
    public void CreateDropDown(SettingInt option) {
        GameObject d = GameObject.Instantiate(dropdown, Vector3.zero, Quaternion.identity);
        d.transform.SetParent(this.transform);
        d.transform.localScale = Vector3.one; 
        d.transform.localPosition = Vector3.zero;
        d.transform.localRotation = Quaternion.Euler(Vector3.zero);

            foreach ( TMP_Text t in d.GetComponentsInChildren<TMP_Text>()) {
            if (t.name == "Label") {
                //t.text = o.type.ToString();
                t.text = option.localizedName.GetLocalizedString();
                t.GetComponent<LocalizeStringEvent>().StringReference = option.localizedName;
            }
        }
        List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();
        if (option.GetType().IsSubclassOf(typeof(SettingLocalizedDropdown)) || option is SettingLocalizedDropdown) {
            foreach(LocalizedString str in ((SettingLocalizedDropdown)option).GetLocalizedDropdowns()) {
                data.Add(new TMP_Dropdown.OptionData(str.GetLocalizedString()));
            }
        } else if (option.GetType().IsSubclassOf(typeof(SettingDropdown)) || option is SettingDropdown) {
            foreach(string str in ((SettingDropdown)option).GetDropdownOptions()) {
                data.Add(new TMP_Dropdown.OptionData(str));
            }
        }
        TMP_Dropdown drop = d.GetComponentInChildren<TMP_Dropdown>();
        drop.options = data;
        drop.value = Mathf.RoundToInt(option.GetValue());
        drop.SetValueWithoutNotify(option.GetValue());
        drop.onValueChanged.AddListener(option.SetValue);
        dropdowns.Add(option, drop);
    }
    private void StringChanged(Locale locale) {
        StopAllCoroutines();
        SettingsManager.StaticStartCoroutine(ChangeStrings());
    }
    IEnumerator ChangeStrings() {
        var otherAsync = LocalizationSettings.SelectedLocaleAsync;
        yield return new WaitUntil(()=>otherAsync.IsDone);
        yield return new WaitForSecondsRealtime(0.1f);
        if (otherAsync.Result != null){
            yield return LocalizationSettings.InitializationOperation;
            foreach (Setting option in SettingsManager.GetSettings()) {
                if (dropdowns.ContainsKey(option)) {
                    if (!option.GetType().IsSubclassOf(typeof(SettingLocalizedDropdown)) && !(option is SettingLocalizedDropdown)) {
                        continue;
                    }
                    dropdowns[option].ClearOptions();
                    for(int i=0;i<((SettingLocalizedDropdown)option).GetLocalizedDropdowns().Length;i++) {
                        while (dropdowns[option].options.Count <= i) {
                            dropdowns[option].options.Add(new TMP_Dropdown.OptionData((option as SettingLocalizedDropdown).GetLocalizedDropdowns()[i].GetLocalizedString()));
                        }
                        dropdowns[option].options[i].text = (option as SettingLocalizedDropdown).GetLocalizedDropdowns()[i].GetLocalizedString();
                    }
                    dropdowns[option].SetValueWithoutNotify(1);
                    dropdowns[option].SetValueWithoutNotify(0);
                    dropdowns[option].SetValueWithoutNotify((option as SettingInt).GetValue());
                }
            }
        }
    }
}

}