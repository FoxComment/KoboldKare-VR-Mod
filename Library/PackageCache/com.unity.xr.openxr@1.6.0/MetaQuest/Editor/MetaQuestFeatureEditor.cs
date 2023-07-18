using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;

using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

namespace UnityEditor.XR.OpenXR.Features.MetaQuestSupport
{
    [CustomEditor(typeof(MetaQuestFeature))]
    internal class MetaQuestFeatureEditor : Editor
    {
        struct TargetDeviceProperty
        {
            public SerializedProperty property;
            public GUIContent label;
        }

        private List<TargetDeviceProperty> targetDeviceProperties;
        private Dictionary<string, bool> activeTargetDevices;

        void InitActiveTargetDevices()
        {
            activeTargetDevices = new Dictionary<string, bool>();

            OpenXRSettings androidOpenXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            var questFeature = androidOpenXRSettings.GetFeature<MetaQuestFeature>();

            if (questFeature == null)
                return;

            foreach (var dev in questFeature.targetDevices)
            {
                activeTargetDevices.Add(dev.manifestName, dev.active);
            }
        }
        void OnEnable()
        {
            targetDeviceProperties = new List<TargetDeviceProperty>();
            InitActiveTargetDevices();
            if (activeTargetDevices.Count == 0)
                return;
            var targetDevicesProperty = serializedObject.FindProperty("targetDevices");

            for (int i = 0; i < targetDevicesProperty.arraySize; ++i)
            {
                var targetDeviceProp = targetDevicesProperty.GetArrayElementAtIndex(i);
                var propManifestName = targetDeviceProp.FindPropertyRelative("manifestName");

                // don't present inactive target devices to the user
                if (propManifestName == null || activeTargetDevices[propManifestName.stringValue] == false)
                    continue;
                var propEnabled = targetDeviceProp.FindPropertyRelative("enabled");
                var propName = targetDeviceProp.FindPropertyRelative("visibleName");
                TargetDeviceProperty curTarget = new TargetDeviceProperty {property = propEnabled, label = EditorGUIUtility.TrTextContent(propName.stringValue)};
                targetDeviceProperties.Add(curTarget);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Target Devices", EditorStyles.boldLabel);

            foreach (var device in targetDeviceProperties)
            {
                EditorGUILayout.PropertyField(device.property, device.label);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}