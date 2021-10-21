using ToolbarExtensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Extensions.ToolbarExtensions.Editor.Extenders
{
    [InitializeOnLoad]
    static class ToolbarExFrameRate
    {
        private static SliderInt FrameRateElement;
        private static Label FPSlabel;
        private static int _cacheFps;

        static ToolbarExFrameRate()
        {
#if UNITY_2021_1_OR_NEWER
            FrameRateElement = new SliderInt(240, -1, SliderDirection.Horizontal, 1);

            FrameRateElement.value = Application.targetFrameRate;
            FrameRateElement.RegisterCallback<ChangeEvent<int>>(onFrameRateElementSliderValueChange);
            FrameRateElement.style.flexDirection = FlexDirection.Row;
        
            SetFrameRateElementStatus();
        
            FPSlabel = new Label("FPS");
            FPSlabel.style.minWidth = 30;
            FPSlabel.style.paddingLeft = 10;
            FrameRateElement.style.marginLeft = 0;
            FrameRateElement.Add(FPSlabel);

            var e = ToolbarElement.Create(FrameRateElement, ExtenderType.Left);
            ToolbarExtender.ToolbarExtend(e);
#else
            ToolbarExtender.RightToolbarGUI.Add(delegate
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("FPS:");
                //EditorGUILayout.LabelField("TScale",GUILayout.MaxWidth(50));
                _cacheFps = EditorGUILayout.IntSlider(_cacheFps, 0, 120, GUILayout.MaxWidth(120));
                if (Application.targetFrameRate != _cacheFps)
                {
                    using (ChangeEvent<int> pooled = ChangeEvent<int>.GetPooled(Application.targetFrameRate, _cacheFps))
                    {
                        onFrameRateElementSliderValueChange(pooled);
                    }
                }

                EditorGUILayout.EndHorizontal();
            });
#endif
            EditorApplication.playModeStateChanged += delegate(PlayModeStateChange change)
            {
                if (change == PlayModeStateChange.EnteredPlayMode)
                {
                    _cacheFps = Application.targetFrameRate;
#if UNITY_2021_1_OR_NEWER
            SetFrameRateElementStatus();
#endif
                }
            };
        }

        private static void onFrameRateElementSliderValueChange(ChangeEvent<int> evt)
        {
            Application.targetFrameRate = evt.newValue;
            var msg = Application.targetFrameRate == -1
                ? "Max"
                : Application.targetFrameRate.ToString();
            var views = SceneView.sceneViews;
            var sceneView = (SceneView)views[0];
            if (sceneView)
            {
                sceneView.Focus();
                sceneView.ShowNotification(new GUIContent(string.Format("FPS:{0}", msg)), 1f);
            }
#if UNITY_2021_1_OR_NEWER
            SetFrameRateElementStatus();
#endif
        }

        private static void SetFrameRateElementStatus()
        {
            var msg = Application.targetFrameRate == -1
                ? "Max"
                : Application.targetFrameRate.ToString();

            if (FrameRateElement != null)
            {
                FrameRateElement.label = msg;
                FrameRateElement.labelElement.style.minWidth = msg.Length * 10;
                FrameRateElement.style.width = 150f + msg.Length * 5;
            }
        }
    }
}