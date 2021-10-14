//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Editor
{
    [CustomEditor(typeof(SettingComponent))]
    internal sealed class SettingComponentInspector : GameFrameworkInspector
    {
        private readonly HelperInfo<SettingHelperBase> m_SettingHelperInfo =
            new HelperInfo<SettingHelperBase>("Setting");

        private void OnEnable()
        {
            m_SettingHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (SettingComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_SettingHelperInfo.Draw();
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Setting Count", t.Count >= 0 ? t.Count.ToString() : "<Unknown>");
                if (t.Count > 0)
                {
                    var settingNames = t.GetAllSettingNames();
                    foreach (var settingName in settingNames)
                        EditorGUILayout.LabelField(settingName, t.GetString(settingName));
                }
            }

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Save Settings")) t.Save();
                if (GUILayout.Button("Remove All Settings")) t.RemoveAllSettings();
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_SettingHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
    }
}