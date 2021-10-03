// ReSharper disable once CheckNamespace

using Narrative.Runtime.Scripts.NarrativeDataBase;
using UnityEditor;
using UnityEngine;

namespace Narrative.Editor.Scripts
{
    [CustomEditor(typeof(NarrativeDataBase), true)]
    public class NarrativeDataBaseInspectorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var database = (NarrativeDataBase)target;

            foreach (var key in database.SavedKeys)
            {
                EditorGUILayout.SelectableLabel(key);
            }

            if (GUILayout.Button("ClearDataBasePerfs"))
            {
                database.ClearAllData();
            }
            //
            if (GUILayout.Button("ClearAllPerfs"))
            {
                PlayerPrefs.DeleteAll();
            }
        }
    }
}