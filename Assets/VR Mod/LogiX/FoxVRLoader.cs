using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SpatialTracking; 
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit; 
using UnityScriptableSettings;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class FoxVRLoader : MonoBehaviour
{
#pragma warning disable CS0618  
    IEnumerator menuPositioner;
    IEnumerator activeBoldManager;

    public enum XRDevice
    {
        LeftHand,
        RightHand,
        LeftFeet,
        RightFeet,
        Head
    }

    Light[] containLights;

    private static FoxVRLoader instance;


    [SerializeField]
    private GameObject[] upcommingFeaturesGroup;


    [SerializeField] UnityEngine.EventSystems.EventSystem xrEvents;
    [SerializeField] UnityEngine.EventSystems.EventSystem uiEvents;

  
    [SerializeField]
    private Transform playerFeetCenter;
    private GameObject controlledKoboldOBJ;
    private GameObject gameManOBJ;
    private GameObject masterCanvOBJ;
    private GameObject fpsCanvOBJ;
    [SerializeField]
    private Transform leftFeet;
    [SerializeField]
    private Transform rightFeet;
    [SerializeField]
    private static Camera activeCamera;
    [SerializeField]
    private Transform mousePointerTRA;
    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Inputs.InputActionManager xrInteractor;
    [SerializeField]
    private GameObject[] controllerModelsOBJs;
    [SerializeField]
    private GameObject[] controllerHandsOBJs;
    [SerializeField]
    private GameObject[] controllersTRAs;
    [SerializeField]
    private Transform[] handAttachmentPoints;
    [SerializeField]
    private GameObject xrSetupOBJ;
  //  [SerializeField]
    //private GameObject streamingCameraOBJ;
    [SerializeField]
    private Camera uiRenderCameraPREF;
    [SerializeField]
    private Camera vrCamera; 
    [SerializeField]
    private Camera orbitCamera;
    [SerializeField]
    private Camera uiRenderCamera;
    [SerializeField]
    private GameObject antiSickOverlay;

    //private bool streamingCameraIsActive;
    public static float koboldScale;

    public static float triggerSqueezeR;
    public static float gripSqueezeR;
    public static float triggerSqueezeL;
    public static float gripSqueezeL;
    public static Vector2 thumbMoveR;
    public static Vector2 thumbMoveL;

    static string lastLoadedScene;
    public static bool activateHandTracking;
    private static bool koboldIsAvailable;  
    public static bool endedBlackout;
    public static bool gameLoaded;
    private bool firstLauhch;

    [SerializeField] InputActionReference triggerR;
    [SerializeField] InputActionReference triggerL;
    [SerializeField] InputActionReference gripR;
    [SerializeField] InputActionReference gripL;
    [SerializeField] InputActionReference touchR;
    [SerializeField] InputActionReference touchL;

    float tunnelingScale;
    float handShrink;
    int hudStyle;
    bool seatedMode;
    bool upcommingFeatures;

    private SettingFloat tunnelingOption;
    private SettingInt testFunctionsOption;
    private SettingInt shrinkHandsOption;
    private SettingInt seatedModeOption;
    private SettingInt HUDOption;

    #region Load Prefs

    void SetupForGame()
    {
        gameManOBJ.transform.localScale = Vector3.one; 

        StopCoroutine(activeBoldManager);
        activeBoldManager = GetActiveBold();
        StartCoroutine(GetActiveBold());
         
        StopCoroutine(menuPositioner);
        menuPositioner = ResetMenuPos();
        StartCoroutine(menuPositioner);

        OptimizeVRMode();         

        endedBlackout = true;
        print("successfull gameplay");
    }
     

    void SetupForMenu()
    {
        gameManOBJ.transform.localScale = Vector3.one;
        gameManOBJ.transform.position = Vector3.zero;
        xrSetupOBJ.transform.parent.localScale = Vector3.one * .75f;

        GetCamera(true); 
 

        endedBlackout = true;
        print("successfull menu");
    }

    #endregion



    #region Update Events

    IEnumerator LevelTracker()
    {
        print("Started Tracking Levels");
        xrEvents.enabled = true;
        uiEvents.enabled = false;
        menuPositioner = ResetMenuPos();
        activeBoldManager = GetActiveBold(); 

        while (true)
        {
            lastLoadedScene = SceneManager.GetActiveScene().name;

            StopCoroutine(activeBoldManager);
            StopCoroutine(menuPositioner);

            menuPositioner = ResetMenuPos();

            activateHandTracking = false;
            
            if (lastLoadedScene == "MainMenu")
            {
                ControllerVisibility(true);
                activeCamera = vrCamera;
                vrCamera.enabled = true;
                Invoke("SetupForMenu", 2);
            }
            else
            {
                Invoke("SetupForGame", 4);
            }

            print("Successfully loaded - " + SceneManager.GetActiveScene().name);

            yield return new WaitUntil(() => SceneManager.GetActiveScene().name != lastLoadedScene);

        }
    }


    IEnumerator ResetMenuPos()
    { 
        while (true)
        {

            ControllerVisibility(false);

            yield return new WaitUntil(()=> GameManager.instance.isPaused); 

            yield return new WaitForEndOfFrame();

            masterCanvOBJ.transform.position = activeCamera.transform.position + (activeCamera.transform.forward * 1.25f * (koboldScale));
            masterCanvOBJ.transform.LookAt(activeCamera.transform);
            masterCanvOBJ.transform.Rotate(new Vector3(0,180,0),Space.Self);

            ControllerVisibility(true);

            yield return new WaitUntil(() => !GameManager.instance.isPaused); 

        }
    }



    #region Public Data




    public static Camera GetUICamera()
    {
        return instance.uiRenderCamera;
    }
    public static bool GetSeatedMode()
    {
        return instance.seatedMode;
    } 
    public static float GetHandsSize()
    {
        return instance.handShrink;
    }

    public static Transform GetPauseScreenTransform()
    {
        return instance.masterCanvOBJ.transform;
    }

    public static void ActivateTunneling(bool _moving)
    {
       if(instance.tunnelingScale>.5f) instance.antiSickOverlay.GetComponent<Animator>().SetBool("Moving", _moving);
    }

    public static void SetActiveCamera(bool _inMenu)
    {
        instance.GetCamera(_inMenu);
    }

    public static Transform GetActiveCameraTransform()
    {
        return activeCamera.transform;
    }

    public static Vector3 GetTrackedDevicePosition(XRDevice _device)
    {
        switch (_device)
        {
            default:
                return instance.vrCamera.transform.position;

            case XRDevice.Head:
                return instance.vrCamera.transform.position;

            case XRDevice.LeftHand:
                return instance.controllersTRAs[1].transform.position;

            case XRDevice.RightHand:
                return instance.controllersTRAs[0].transform.position;

            case XRDevice.LeftFeet:
                return instance.leftFeet.transform.position;

            case XRDevice.RightFeet:
                return instance.rightFeet.transform.position;
        } 
    }

    /// <summary>
    /// Remove this one later
    /// </summary> 
    public static Transform GetTrackedDeviceTRANSFORM(XRDevice _device)
    {
        switch (_device)
        {
            default:
                return instance.vrCamera.transform;

            case XRDevice.Head:
                return instance.vrCamera.transform;

            case XRDevice.LeftHand:
                return instance.handAttachmentPoints[0].transform;

            case XRDevice.RightHand:
                return instance.handAttachmentPoints[1].transform;

            case XRDevice.LeftFeet:
                return instance.leftFeet.transform;

            case XRDevice.RightFeet:
                return instance.rightFeet.transform;
        }
    }

    public static Quaternion GetTrackedDeviceRotation(XRDevice _device)
    {
        switch (_device)
        {
            default:
                return instance.vrCamera.transform.rotation;

            case XRDevice.Head:
                return instance.vrCamera.transform.rotation;

            case XRDevice.LeftHand:
                return instance.controllersTRAs[1].transform.rotation;

            case XRDevice.RightHand:
                return instance.controllersTRAs[0].transform.rotation;

            case XRDevice.LeftFeet:
                return instance.leftFeet.transform.rotation;

            case XRDevice.RightFeet:
                return instance.rightFeet.transform.rotation;
        }
    }



    #endregion


    private void OptimizeVRMode()
    {
        containLights = GameObject.FindObjectsOfType<Light>();
        foreach (Light item in containLights)
            item.enabled = false;
    }



    private void FixedUpdate()
    {

        if (activeCamera == null) return;

        if (LevelLoader.loadingLevel || !koboldIsAvailable) return;

        antiSickOverlay.transform.position = activeCamera.transform.position;  //Jittery in 3DOF, same im Update and LateUpdate

        if (!activateHandTracking) return;

        xrSetupOBJ.transform.position = controlledKoboldOBJ.transform.position - (Vector3.up * .72f);
        orbitCamera.transform.rotation = vrCamera.transform.rotation;
    }



    private void LateUpdate()
    {

        if (activeCamera == null) return;

        Vector3 cursorPoint = Mouse.current.position.ReadValue();
        Vector3 lookPoint = activeCamera.ScreenToWorldPoint(new Vector3(cursorPoint.x, cursorPoint.y, .7f),
        Camera.MonoOrStereoscopicEye.Mono);
        mousePointerTRA.position = lookPoint;
        mousePointerTRA.localPosition = new Vector3(mousePointerTRA.localPosition.x, mousePointerTRA.localPosition.y, mousePointerTRA.localPosition.z);   //Dumb thing...

        if (LevelLoader.loadingLevel || !koboldIsAvailable) return;

        if (!activateHandTracking) return;




        /////////////////////////////////////////////////////////////////////////////////
        //                                                                             //
        //                     FIGURE OUT ON HOW TO USE S:VR PLUGIN                    //
        //                                                                             //
        //      so i can use svr bindings and controller models                        //                                                                              
        /////////////////////////////////////////////////////////////////////////////////




        gripSqueezeL = gripL.action.ReadValue<float>();
        gripSqueezeR = gripR.action.ReadValue<float>();
        triggerSqueezeL = triggerL.action.ReadValue<float>();
        triggerSqueezeR = triggerR.action.ReadValue<float>();
        //thumbMoveR = touchR.action.ReadValue<Vector2>();
        //thumbMoveL = touchL.action.ReadValue<Vector2>();  

        playerFeetCenter.localPosition = new Vector3(GetActiveCameraTransform().localPosition.x, 0, GetActiveCameraTransform().localPosition.z);    //Temporal thing
        playerFeetCenter.rotation = Quaternion.Euler(90, vrCamera.transform.rotation.eulerAngles.y, 0);                            //Temporal thing

    }



    #endregion




    IEnumerator GetActiveBold()
    {
        while (true)
        {
            koboldIsAvailable = false;
            endedBlackout = false;

            if (controlledKoboldOBJ == null)
            {
                yield return new WaitUntil(()=> GameObject.FindObjectOfType<PlayerPossession>() != null);  //Make it nicer, that just will search for GO every frame
                controlledKoboldOBJ = GameObject.FindObjectOfType<PlayerPossession>().transform.gameObject;

            }
            else if (!controlledKoboldOBJ.gameObject.activeSelf)
            {
                controlledKoboldOBJ.transform.parent.GetChild(0).GetComponent<HandIK>().controlledByPlayer = false;
                controlledKoboldOBJ.transform.parent.GetChild(0).GetComponent<FootIK>().controlledByPlayer = false;
                controlledKoboldOBJ = GameObject.FindObjectOfType<PlayerPossession>().transform.gameObject;

            }

            yield return new WaitForSeconds(2.5f);


            GetCamera(false);

            koboldIsAvailable = true;

            RescalePlayer();

            endedBlackout = true;

            yield return new WaitUntil(() => controlledKoboldOBJ == null || !controlledKoboldOBJ.gameObject.activeSelf);
        }
    }




    #region Camera Related



    public void GetCamera(bool _inMenu)
    {

        OrbitCamera.SetSeatedMode(seatedMode);

        vrCamera.GetComponent<AudioListener>().enabled = !seatedMode;

        ReloadControllers();

        if (_inMenu)
        {  

            if (!seatedMode)
            {

                activeCamera = vrCamera;

                xrSetupOBJ.transform.position = new Vector3(.2f, -1.427f, -10.15f);     //User pose for main menu idk...
            }
            else
            { 
                activeCamera = vrCamera;

                xrSetupOBJ.transform.position = new Vector3(.2f, -1.427f, -10.15f);     //User pose for main menu idk... 
            }

            SetupUIOverlays(_inMenu);

        }
        else
        {
            orbitCamera = GameObject.Find("OrbitCamera").GetComponent<Camera>();

            orbitCamera.GetComponent<AudioListener>().enabled = seatedMode;

            orbitCamera.enabled = seatedMode;
            vrCamera.enabled = !seatedMode;

            if (!seatedMode)
            {
                if (orbitCamera.GetComponent<TrackedPoseDriver>() != null) Destroy(orbitCamera.GetComponent<TrackedPoseDriver>());

                vrCamera.gameObject.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                activeCamera = vrCamera;

                controlledKoboldOBJ.transform.parent.GetChild(0).GetComponent<HandIK>().controlledByPlayer = true;

                SetupUIOverlays(_inMenu);
                 
                return;
            }
            else
            {
                activeCamera = orbitCamera;
                //activeCamera = vrCamera;
                //vrCamera.gameObject.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
                SetupUIOverlays(_inMenu);
                 
                if (orbitCamera.GetComponent<TrackedPoseDriver>() != null) return;

                orbitCamera.gameObject.AddComponent<TrackedPoseDriver>();
                orbitCamera.gameObject.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationOnly;

            }

        } 
    }


    void SetupUIOverlays(bool _inMenu)
    {
        masterCanvOBJ.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        if (uiRenderCamera != null) 
            Destroy(uiRenderCamera.gameObject);

        uiRenderCamera = Instantiate(uiRenderCameraPREF, activeCamera.transform);
        uiRenderCamera.transform.localPosition = Vector3.zero;
        uiRenderCamera.transform.localRotation = Quaternion.Euler(Vector3.zero); 

        activeCamera.GetUniversalAdditionalCameraData().cameraStack.Add(uiRenderCamera);

        if (_inMenu)
        {
            masterCanvOBJ.transform.localScale = Vector3.one * .0004f;
            masterCanvOBJ.transform.position = new Vector3(.2f, -.596f, -9.555f);
            masterCanvOBJ.transform.localRotation = Quaternion.Euler(45, 0, 0);

        }
        else
        {
            fpsCanvOBJ = GameObject.Find("Canvas - FPSView");
             
            fpsCanvOBJ.GetComponent<Canvas>().worldCamera = uiRenderCamera;
            fpsCanvOBJ.GetComponent<Animator>().SetInteger("style", hudStyle); 

        }
    }


    void RescalePlayer()
    {
        koboldScale = controlledKoboldOBJ.transform.parent.transform.localScale.y;

        orbitCamera.transform.localScale = Vector3.one * (koboldScale);

        xrSetupOBJ.transform.parent.localScale = Vector3.one * (koboldScale);

        masterCanvOBJ.transform.localScale = (Vector3.one * .0007f) * (koboldScale);

        fpsCanvOBJ.GetComponent<Canvas>().planeDistance = 2f;

        fpsCanvOBJ.transform.parent.localScale = Vector3.one * .65f / (koboldScale);
    }


    private void StreamingCameraToggle()
    {
        // streamingCameraIsActive = !streamingCameraIsActive;

        //streamingRT.height = Screen.height/2;
        //streamingRT.width = Screen.width/2;

        //streamingCameraOBJ.SetActive(streamingCameraIsActive);
    }



    #endregion




    #region Controllers Stuff



    private void ReloadControllers()
    {
        StartCoroutine(RefreshControllerInteractor());

        activateHandTracking = !seatedMode;

        controllersTRAs[0].gameObject.SetActive(!seatedMode);
        controllersTRAs[1].gameObject.SetActive(!seatedMode);

    }


    private IEnumerator RefreshControllerInteractor()
    {
        xrInteractor.enabled = false;

        yield return new WaitForEndOfFrame();

        xrInteractor.enabled = true;
    }
    

    void ControllerVisibility(bool _visible)
    {
        controllerHandsOBJs[0].SetActive(_visible);
        controllerHandsOBJs[1].SetActive(_visible);
        controllerModelsOBJs[0].SetActive(_visible);
        controllerModelsOBJs[1].SetActive(_visible); 
    }



    #endregion




    IEnumerator Start()
    {
        gameLoaded = false;
        gameManOBJ = gameObject; 
        masterCanvOBJ = transform.GetChild(0).gameObject;

        menuPositioner = ResetMenuPos();
        activeBoldManager = GetActiveBold();
        if (instance == null)
        {
            instance = this;
            StartCoroutine(LevelTracker());
        }
        yield return new WaitUntil(() => SettingsManager.GetSetting("VRTunneling") != null);
        tunnelingOption = SettingsManager.GetSetting("VRTunneling") as SettingFloat;
        tunnelingOption.changed += OnTunnelingChanged;
        OnTunnelingChanged(tunnelingOption.GetValue());

        yield return new WaitUntil(() => SettingsManager.GetSetting("VRHUD") != null);
        HUDOption = SettingsManager.GetSetting("VRHUD") as SettingInt;
        HUDOption.changed += OnHUDChanged;
        OnHUDChanged(HUDOption.GetValue());

        yield return new WaitUntil(() => SettingsManager.GetSetting("VRSeated") != null);
        seatedModeOption = SettingsManager.GetSetting("VRSeated") as SettingInt;
        seatedModeOption.changed += OnSeatedChanged;
        OnSeatedChanged(seatedModeOption.GetValue());

        yield return new WaitUntil(() => SettingsManager.GetSetting("VRShrinkHands") != null);
        shrinkHandsOption = SettingsManager.GetSetting("VRShrinkHands") as SettingInt;
        shrinkHandsOption.changed += OnShrinkHandsChanged;
        OnShrinkHandsChanged(shrinkHandsOption.GetValue());

        yield return new WaitUntil(() => SettingsManager.GetSetting("VRTests") != null);
        testFunctionsOption = SettingsManager.GetSetting("VRTests") as SettingInt;
        testFunctionsOption.changed += OnTestFunctionsChanged;
        OnTestFunctionsChanged(testFunctionsOption.GetValue());


        gameLoaded = true;
        //LevelLoader.instance.sceneLoadStart += SceneLoadStart;
        //  LevelLoader.instance.sceneLoadEnd += SceneLoadEnd;
    }


    private void OnEnable()
    {
        triggerR.action.Enable();
        triggerL.action.Enable();
        gripL.action.Enable();
        gripR.action.Enable();


        if (tunnelingOption != null)
        {
            tunnelingOption.changed += OnTunnelingChanged;
            OnTunnelingChanged(tunnelingOption.GetValue());
        }

        if (HUDOption != null)
        {
            HUDOption.changed += OnHUDChanged;
            OnHUDChanged(HUDOption.GetValue());
        }

        if (seatedModeOption != null)
        {
            seatedModeOption.changed += OnSeatedChanged;
            OnSeatedChanged(seatedModeOption.GetValue());
        }

        if (shrinkHandsOption != null)
        {
            shrinkHandsOption.changed += OnShrinkHandsChanged;
            OnShrinkHandsChanged(shrinkHandsOption.GetValue());
        }

        if (testFunctionsOption != null)
        {
            testFunctionsOption.changed += OnTestFunctionsChanged;
            OnTestFunctionsChanged(testFunctionsOption.GetValue());
        }
    }

     

    private void OnDisable()
    {
        triggerR.action.Disable();
        triggerL.action.Disable();
        gripL.action.Disable();
        gripR.action.Disable();


        if (tunnelingOption != null)
        {
            tunnelingOption.changed -= OnTunnelingChanged; 
        }

        if (HUDOption != null)
        {
            HUDOption.changed -= OnHUDChanged; 
        }

        if (seatedModeOption != null)
        {
            seatedModeOption.changed -= OnSeatedChanged; 
        }

        if (shrinkHandsOption != null)
        {
            shrinkHandsOption.changed -= OnShrinkHandsChanged; 
        }

        if (testFunctionsOption != null)
        {
            testFunctionsOption.changed -= OnTestFunctionsChanged; 
        }

        //LevelLoader.instance.sceneLoadStart -= SceneLoadStart;
        //LevelLoader.instance.sceneLoadEnd -= SceneLoadEnd;
    }

    void OnTunnelingChanged(float value)
    {
        tunnelingScale = value;
    }
    void OnHUDChanged(int value)
    { 
        hudStyle = value;
        if(fpsCanvOBJ != null) fpsCanvOBJ.GetComponent<Animator>().SetInteger("style", hudStyle);
    }
    void OnSeatedChanged(int value)
    {
        seatedMode = (value == 1); 
    }
    void OnShrinkHandsChanged(int value)
    {
        if (value == 1) handShrink = .75f;
        else handShrink = 1;
    }
    void OnTestFunctionsChanged(int value)
    { 
        upcommingFeatures = (value == 1);
    } 



}
