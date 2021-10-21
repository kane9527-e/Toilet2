using System;
using ToolbarExtensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Extensions.ToolbarExtensions.Editor.Extenders
{
    [InitializeOnLoad]
    static class ToolbarExTimeScaleSlider
    {
        private static Slider timeScaleElement;
        private static float _cacheTimeScale = Time.timeScale;

        static ToolbarExTimeScaleSlider()
        {
#if UNITY_2021_1_OR_NEWER
            timeScaleElement = new Slider(0f, 1f);
            timeScaleElement.style.width = 150f;
            timeScaleElement.value = Time.timeScale;
            timeScaleElement.RegisterCallback<ChangeEvent<float>>(onTimeScaleSliderValueChange);


            timeScaleElement.style.flexDirection = FlexDirection.RowReverse;
            timeScaleElement.label = Time.timeScale.ToString();
            timeScaleElement.labelElement.style.minWidth = 50;
            timeScaleElement.labelElement.style.paddingLeft = 10;

            VisualElement iconVE = new VisualElement();
            iconVE.AddToClassList("unity-editor-toolbar-element__icon");
            //"d_UnityEditor.AnimationWindow@2x"
            //"d_Profiler.Record"
            var icon = Background.FromTexture2D(EditorGUIUtility.FindTexture("d_UnityEditor.AnimationWindow@2x"));
            iconVE.style.backgroundImage = icon;
            iconVE.style.height = 16;
            iconVE.style.width = 16;
            timeScaleElement.style.marginLeft = 0;
            timeScaleElement.Add(iconVE);

            //timeScaleElement.Add(container);
            var e = ToolbarElement.Create(timeScaleElement, ExtenderType.Right);
            ToolbarExtender.ToolbarExtend(e);
#else
            ToolbarExtender.LeftToolbarGUI.Add(DrawTimeScaleSlider);
#endif

            EditorApplication.playModeStateChanged += delegate(PlayModeStateChange change)
            {
                if (change == PlayModeStateChange.EnteredPlayMode)
                {
                    _cacheTimeScale = Time.timeScale;
#if UNITY_2021_1_OR_NEWER
            UpdateTimeScaleLabel();
#endif
                }
            };
        }

        private static void DrawTimeScaleSlider()
        {
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.BeginHorizontal();
            var icon = Background.FromTexture2D(EditorGUIUtility.FindTexture("d_UnityEditor.AnimationWindow"));
            GUILayout.Box(icon.texture);
            //EditorGUILayout.LabelField("TScale",GUILayout.MaxWidth(50));
            _cacheTimeScale =
                EditorGUILayout.Slider(_cacheTimeScale, 0, 1, GUILayout.MaxWidth(120));

            if (Math.Abs(Time.timeScale - _cacheTimeScale) > 0)
            {
                using (ChangeEvent<float> pooled = ChangeEvent<float>.GetPooled(Time.timeScale, _cacheTimeScale))
                {
                    onTimeScaleSliderValueChange(pooled);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        // private static void UpdateTimeScaleAndNotification()
        // {
        //     if (Math.Abs(Time.timeScale - _cacheTimeScale) > 0)
        //     {
        //         Time.timeScale = _cacheTimeScale;
        //         var views = SceneView.sceneViews;
        //         var sceneView = (SceneView)views[0];
        //         if (sceneView)
        //         {
        //             sceneView.Focus();
        //             sceneView.ShowNotification(new GUIContent(string.Format("时间缩放\nx{0}", Time.timeScale)), 1f);
        //         }
        //     }
        // }

        private static void onTimeScaleSliderValueChange(ChangeEvent<float> evt)
        {
            Time.timeScale = evt.newValue;
            //timeScaleElement.label = string.Format("时间缩放:{0}", Time.timeScale);
            var views = SceneView.sceneViews;
            var sceneView = (SceneView)views[0];
            if (sceneView)
            {
                sceneView.Focus();
                sceneView.ShowNotification(new GUIContent(string.Format("时间缩放\nx{0}", Time.timeScale)), 1f);
            }

#if UNITY_2021_1_OR_NEWER
            UpdateTimeScaleLabel();
#endif
        }

        private static void UpdateTimeScaleLabel()
        {
            if (timeScaleElement != null)
                timeScaleElement.label = Time.timeScale.ToString();
        }
    }
}