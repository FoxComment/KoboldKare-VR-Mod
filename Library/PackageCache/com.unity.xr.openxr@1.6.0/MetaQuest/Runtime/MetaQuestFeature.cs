using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
#endif

[assembly:InternalsVisibleTo("Unity.XR.OpenXR.Features.OculusQuestSupport")]
[assembly:InternalsVisibleTo("Unity.XR.OpenXR.Features.MetaQuestSupport.Editor")]
namespace UnityEngine.XR.OpenXR.Features.MetaQuestSupport
{
    /// <summary>
    /// Enables the Meta mobile OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with Quest.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Meta Quest Support",
        Desc="Necessary to deploy a Meta Quest compatible app.",
        Company = "Unity",
        DocumentationLink = "https://developer.oculus.com/downloads/package/oculus-openxr-mobile-sdk/",
        OpenxrExtensionStrings = "XR_OCULUS_android_initialize_loader",
        Version = "1.0.0",
        BuildTargetGroups = new []{BuildTargetGroup.Android},
        CustomRuntimeLoaderBuildTargets = new []{BuildTarget.Android},
        FeatureId = featureId
    )]
#endif
    public class MetaQuestFeature : OpenXRFeature
    {
        [Serializable]
        internal struct TargetDevice
        {
            public string visibleName;
            public string manifestName;
            public bool enabled;
            [NonSerialized] public bool active;
        }
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.unity.openxr.feature.metaquest";

#if UNITY_EDITOR
        /// <summary>
        /// Adds devices to the supported devices list in the Android manifest.
        /// </summary>
        [SerializeField]
        internal List<TargetDevice> targetDevices;

        public new void OnEnable()
        {
            // add known devices
            AddTargetDevice("quest", "Quest", true);
            AddTargetDevice("quest2", "Quest 2", true);
            AddTargetDevice("cambria", "Quest Pro", true);
        }
        /// <summary>
        /// Adds additional target devices to the devices list in the MetaQuestFeatureEditor. Added target devices will
        /// be serialized into the settings asset and will persist across editor sessions, but will only be visible to users
        /// and the manifest if they've been added in the active editor session.
        /// </summary>
        /// <param name="manifestName">Target device name that will be added to AndroidManifest</param>
        /// <param name="visibleName">Device name that will be displayed in feature configuration UI</param>
        /// <param name="enabledByDefault">Target device should be enabled by default or not</param>
        public void AddTargetDevice(string manifestName, string visibleName, bool enabledByDefault)
        {
            if (targetDevices == null)
                targetDevices = new List<TargetDevice>();

            // don't add devices that already exist, but do mark them active for this session
            for (int i = 0; i < targetDevices.Count; ++i)
            {
                var dev = targetDevices[i];

                if (dev.manifestName == manifestName)
                {
                    dev.active = true;
                    targetDevices[i] = dev;
                    return;
                }
            }

            TargetDevice targetDevice = new TargetDevice { manifestName = manifestName, visibleName = visibleName, enabled = enabledByDefault, active = true };
            targetDevices.Add(targetDevice);
        }

        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            rules.Add(new ValidationRule(this)
            {
                message = "Only the Oculus Touch Interaction Profile is supported right now.",
                checkPredicate = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    if (null == settings)
                        return false;

                    bool touchFeatureEnabled = false;
                    bool otherInteractionFeatureEnabled = false;
                    foreach (var feature in settings.GetFeatures<OpenXRInteractionFeature>())
                    {
                        if (feature.enabled)
                        {
                            if (feature is OculusTouchControllerProfile)
                                touchFeatureEnabled = true;
                            else
                                otherInteractionFeatureEnabled = true;
                        }
                    }
                    return touchFeatureEnabled && !otherInteractionFeatureEnabled;
                },
                fixIt = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
                    if (null == settings)
                        return;

                    foreach (var feature in settings.GetFeatures<OpenXRInteractionFeature>())
                    {
                        feature.enabled = (feature is OculusTouchControllerProfile);
                    }
                },
                error = true,
            });

            rules.Add(new ValidationRule(this)
            {
                message = "No Quest target devices selected.",
                checkPredicate = () =>
                {
                    foreach (var device in targetDevices)
                    {
                        if (device.enabled)
                            return true;
                    }

                    return false;
                },
                fixIt = () =>
                {
                    var window = MetaQuestFeatureEditorWindow.Create(this);
                    window.ShowPopup();
                },
                error = true,
                fixItAutomatic = false,
            });
        }

        internal class MetaQuestFeatureEditorWindow : EditorWindow
        {
            private Object feature;
            private Editor featureEditor;

            public static EditorWindow Create(Object feature)
            {
                var window = EditorWindow.GetWindow<MetaQuestFeatureEditorWindow>(true, "Meta Quest Feature Configuration", true);
                window.feature = feature;
                window.featureEditor = Editor.CreateEditor(feature);
                return window;
            }

            private void OnGUI()
            {
                featureEditor.OnInspectorGUI();
            }
        }
#endif
    }
}