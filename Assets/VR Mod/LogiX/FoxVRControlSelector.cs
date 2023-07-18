using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityScriptableSettings;
public class FoxVRControlSelector : MonoBehaviour
{
#pragma warning disable CS0618
    //fig ot PlayerPrefs. nt wrkn 
    [SerializeField] TMPro.TextMeshProUGUI tipTEXT;

    public void ThreeDOF()
    {
        (SettingsManager.GetSetting("VRSeated") as SettingInt).SetValue(1);
        FoxVRLoader.SetActiveCamera(true);
        SettingsManager.Save(); //Thank you so much Nael!
        PlayerPrefs.SetInt("FirstSetup", 1);
    }


    public void ResetThingy()
    { 
        PlayerPrefs.SetInt("FirstSetup", 0);
        FoxVRLoader.SetActiveCamera(true);
        SettingsManager.Save(); //Thank you so much Nael!
    }
    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void SixDOF()
    {
        (SettingsManager.GetSetting("VRSeated") as SettingInt).SetValue(0);
        SettingsManager.Save(); //Thank you so much Nael!
        FoxVRLoader.SetActiveCamera(true);
        PlayerPrefs.SetInt("FirstSetup", 1);
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => FoxVRLoader.gameLoaded);
        yield return new WaitForSeconds(1);
        if(PlayerPrefs.GetInt("FirstSetup" ) == 1) gameObject.SetActive(false);
        GetComponent<Animator>().SetBool("loaded", true);
    }
    public void ThreeHover() => tipTEXT.text = "3 DOF Control scheme is good if you don't have/want any XR interactable controllers. Like when you sit in a chair in front of PC";
    public void SixHover() => tipTEXT.text = "Full VR controlls";
    //Use Localiziation thingy later~
}
