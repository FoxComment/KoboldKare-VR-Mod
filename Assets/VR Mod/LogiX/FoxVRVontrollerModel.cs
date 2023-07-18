using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction;
using UnityEngine.XR.LegacyInputHelpers;
using UnityEngine.XR.Management;
using UnityEngine.XR.Provider;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.SpatialTracking; 
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Rendering.Universal;
using UnityScriptableSettings;

//A dumb effortless script
//I'll re-write it after my surgery

public class FoxVRVontrollerModel : MonoBehaviour
{
    [SerializeField]
    private bool rightHand;//turn into dropdown
    [SerializeField]
    private Transform controllerVisual;
    [SerializeField]
    private Transform handVisual;
    private List<InputDevice> devices = new List<InputDevice>(); 
    [SerializeField]
    private List<aboutController> controllerData = new List<aboutController>();
    [System.Serializable]
    private class aboutController
    {
        public string name;
        [Space(5)]
        public Mesh mesh;
        [Space (15)]
        public Vector3 modelPositionOffset;
        public Vector3 modelRotationOffset;
        [Space(5)]
        public Vector3 handPositionOffset;
        public Vector3 handRotationOffset;
    }


    void OnEnable()
    {
        InputDevices.deviceConnected += DeviceConnected; 
        InputDevices.GetDevices(devices);
        foreach (var device in devices)
            DeviceConnected(device);
    }
    void OnDisable()
    {
        InputDevices.deviceConnected -= DeviceConnected;
    }
    void DeviceConnected(InputDevice device) //Make it go into one handed/seated mode if one/none controllers presented
    { 
        if ((device.characteristics & InputDeviceCharacteristics.Left) != 0)
        {
            GetModel(device);
        } 
        else if ((device.characteristics & InputDeviceCharacteristics.Right) != 0)
        {
            GetModel(device);
        }
    }


    private void GetModel(InputDevice device)
    { 
        if (rightHand)
        controllerVisual.localScale = new Vector3(-1, 1, 1);

        foreach (aboutController controller in controllerData)   
        {
            
            if (device.name.Contains(controller.name))
            {

                controllerVisual.GetComponent<MeshFilter>().mesh = controller.mesh;

                controllerVisual.localPosition = controller.modelPositionOffset;
                controllerVisual.localRotation = Quaternion.Euler(controller.modelRotationOffset);

                handVisual.localPosition = controller.handPositionOffset;
                handVisual.localRotation = Quaternion.Euler(controller.handRotationOffset);
                return;

            }
        }
    }


}
