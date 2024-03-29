﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Editor
{
    [CustomEditor(typeof(ReferencePoolComponent))]
    internal sealed class ReferencePoolComponentInspector : GameFrameworkInspector
    {
        private readonly HashSet<string> m_OpenedItems = new HashSet<string>();

        private readonly Dictionary<string, List<ReferencePoolInfo>> m_ReferencePoolInfos =
            new Dictionary<string, List<ReferencePoolInfo>>(StringComparer.Ordinal);

        private SerializedProperty m_EnableStrictCheck;

        private bool m_ShowFullClassName;

        private void OnEnable()
        {
            m_EnableStrictCheck = serializedObject.FindProperty("m_EnableStrictCheck");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            var t = (ReferencePoolComponent)target;

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                var enableStrictCheck = EditorGUILayout.Toggle("Enable Strict Check", t.EnableStrictCheck);
                if (enableStrictCheck != t.EnableStrictCheck) t.EnableStrictCheck = enableStrictCheck;

                EditorGUILayout.LabelField("Reference Pool Count", ReferencePool.Count.ToString());
                m_ShowFullClassName = EditorGUILayout.Toggle("Show Full Class Name", m_ShowFullClassName);
                m_ReferencePoolInfos.Clear();
                var referencePoolInfos = ReferencePool.GetAllReferencePoolInfos();
                foreach (var referencePoolInfo in referencePoolInfos)
                {
                    var assemblyName = referencePoolInfo.Type.Assembly.GetName().Name;
                    List<ReferencePoolInfo> results = null;
                    if (!m_ReferencePoolInfos.TryGetValue(assemblyName, out results))
                    {
                        results = new List<ReferencePoolInfo>();
                        m_ReferencePoolInfos.Add(assemblyName, results);
                    }

                    results.Add(referencePoolInfo);
                }

                foreach (var assemblyReferencePoolInfo in m_ReferencePoolInfos)
                {
                    var lastState = m_OpenedItems.Contains(assemblyReferencePoolInfo.Key);
                    var currentState = EditorGUILayout.Foldout(lastState, assemblyReferencePoolInfo.Key);
                    if (currentState != lastState)
                    {
                        if (currentState)
                            m_OpenedItems.Add(assemblyReferencePoolInfo.Key);
                        else
                            m_OpenedItems.Remove(assemblyReferencePoolInfo.Key);
                    }

                    if (currentState)
                    {
                        EditorGUILayout.BeginVertical("box");
                        {
                            EditorGUILayout.LabelField(m_ShowFullClassName ? "Full Class Name" : "Class Name",
                                "Unused\tUsing\tAcquire\tRelease\tAdd\tRemove");
                            assemblyReferencePoolInfo.Value.Sort(Comparison);
                            foreach (var referencePoolInfo in assemblyReferencePoolInfo.Value)
                                DrawReferencePoolInfo(referencePoolInfo);

                            if (GUILayout.Button("Export CSV Data"))
                            {
                                var exportFileName = EditorUtility.SaveFilePanel("Export CSV Data", string.Empty,
                                    Utility.Text.Format("Reference Pool Data - {0}.csv", assemblyReferencePoolInfo.Key),
                                    string.Empty);
                                if (!string.IsNullOrEmpty(exportFileName))
                                    try
                                    {
                                        var index = 0;
                                        var data = new string[assemblyReferencePoolInfo.Value.Count + 1];
                                        data[index++] =
                                            "Class Name,Full Class Name,Unused,Using,Acquire,Release,Add,Remove";
                                        foreach (var referencePoolInfo in assemblyReferencePoolInfo.Value)
                                            data[index++] = Utility.Text.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                                referencePoolInfo.Type.Name, referencePoolInfo.Type.FullName,
                                                referencePoolInfo.UnusedReferenceCount.ToString(),
                                                referencePoolInfo.UsingReferenceCount.ToString(),
                                                referencePoolInfo.AcquireReferenceCount.ToString(),
                                                referencePoolInfo.ReleaseReferenceCount.ToString(),
                                                referencePoolInfo.AddReferenceCount.ToString(),
                                                referencePoolInfo.RemoveReferenceCount.ToString());

                                        File.WriteAllLines(exportFileName, data, Encoding.UTF8);
                                        Debug.Log(Utility.Text.Format(
                                            "Export reference pool CSV data to '{0}' success.", exportFileName));
                                    }
                                    catch (Exception exception)
                                    {
                                        Debug.LogError(Utility.Text.Format(
                                            "Export reference pool CSV data to '{0}' failure, exception is '{1}'.",
                                            exportFileName, exception.ToString()));
                                    }
                            }
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.Separator();
                    }
                }
            }
            else
            {
                EditorGUILayout.PropertyField(m_EnableStrictCheck);
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        private void DrawReferencePoolInfo(ReferencePoolInfo referencePoolInfo)
        {
            EditorGUILayout.LabelField(
                m_ShowFullClassName ? referencePoolInfo.Type.FullName : referencePoolInfo.Type.Name,
                Utility.Text.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", referencePoolInfo.UnusedReferenceCount.ToString(),
                    referencePoolInfo.UsingReferenceCount.ToString(),
                    referencePoolInfo.AcquireReferenceCount.ToString(),
                    referencePoolInfo.ReleaseReferenceCount.ToString(), referencePoolInfo.AddReferenceCount.ToString(),
                    referencePoolInfo.RemoveReferenceCount.ToString()));
        }

        private int Comparison(ReferencePoolInfo a, ReferencePoolInfo b)
        {
            if (m_ShowFullClassName)
                return a.Type.FullName.CompareTo(b.Type.FullName);
            return a.Type.Name.CompareTo(b.Type.Name);
        }
    }
}