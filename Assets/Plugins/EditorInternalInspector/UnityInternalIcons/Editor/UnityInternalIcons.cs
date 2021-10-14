/*
 *	Created by Philippe Groarke on 2016-08-28.
 *	Copyright (c) 2016 Tarfmagougou Games. All rights reserved.
 *
 *	Dedication : I dedicate this code to Gabriel, who makes kickass extensions. Now go out and use awesome icons!
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorInternalInspector
{
    internal struct BuiltinIcon : IEquatable<BuiltinIcon>, IComparable<BuiltinIcon>
    {
        public GUIContent icon;
        public GUIContent name;

        public override bool Equals(object o)
        {
            return o is BuiltinIcon && Equals((BuiltinIcon)o);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public bool Equals(BuiltinIcon o)
        {
            return name.text == o.name.text;
        }

        public int CompareTo(BuiltinIcon o)
        {
            return name.text.CompareTo(o.name.text);
        }
    }

    public class UnityInternalIcons : EditorWindow
    {
        private readonly List<BuiltinIcon> _icons = new List<BuiltinIcon>();
        private GUIContent _refresh_button;
        private Vector2 _scroll_pos;

        private void OnEnable()
        {
            _refresh_button = new GUIContent(EditorGUIUtility.IconContent("d_preAudioLoopOff").image,
                "Refresh : Icons are only loaded in memory when the appropriate window is opened.");

            FindIcons();
        }

        private void OnGUI()
        {
            _scroll_pos = EditorGUILayout.BeginScrollView(_scroll_pos);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button(_refresh_button, EditorStyles.toolbarButton)) FindIcons();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("总共找到 " + _icons.Count + " 个图标");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("双击复制图标名称", UnityInternalIconHelperUII.GetMiniGreyLabelStyle());

            EditorGUILayout.Space();

            EditorGUIUtility.labelWidth = 100;
            for (var i = 0; i < _icons.Count; ++i)
            {
                EditorGUILayout.LabelField(_icons[i].icon, _icons[i].name);

                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) &&
                    Event.current.type == EventType.MouseDown && Event.current.clickCount > 1)
                {
                    EditorGUIUtility.systemCopyBuffer = _icons[i].name.text;
                    Debug.Log(_icons[i].name.text + " 已拷贝到剪贴板.");
                }
            }

            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Window/编辑器扩展/Unity内置图标检查器")]
        public static void ShowWindow()
        {
            var w = GetWindow<UnityInternalIcons>();
            UnityInternalIconHelperUII.SetWindowTitle(w, "Unity内置图标检查器");
        }

        /* Find all textures and filter them to narrow the search. */
        private void FindIcons()
        {
            _icons.Clear();

            var t = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (var x in t)
            {
                if (x.name.Length == 0)
                    continue;

                if (x.hideFlags != HideFlags.HideAndDontSave &&
                    x.hideFlags != (HideFlags.HideInInspector | HideFlags.HideAndDontSave))
                    continue;

                if (!EditorUtility.IsPersistent(x))
                    continue;

                /* This is the *only* way I have found to confirm the icons are indeed unity builtin. Unfortunately
                 * it uses LogError instead of LogWarning or throwing an Exception I can catch. So make it shut up. */
                UnityInternalIconHelperUII.DisableLogging();
                var gc = EditorGUIUtility.IconContent(x.name);
                UnityInternalIconHelperUII.EnableLogging();

                if (gc == null)
                    continue;
                if (gc.image == null)
                    continue;

                _icons.Add(new BuiltinIcon
                {
                    icon = gc,
                    name = new GUIContent(x.name)
                });
            }

            _icons.Sort();
            Resources.UnloadUnusedAssets();
            GC.Collect();
            Repaint();
        }
    }
}