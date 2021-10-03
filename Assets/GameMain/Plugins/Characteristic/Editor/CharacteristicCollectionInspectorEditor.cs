using UnityEngine;

#if UNITY_EDITOR
// ReSharper disable once CheckNamespace
namespace Characteristic.Editor
{
    using Characteristic.Runtime.Scripts.ScriptableObject;
    using UnityEditor;

    [CustomEditor(typeof(CharacteristicCollection))]
    public class CharacteristicCollectionInspectorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("ResetAllValue"))
            {
                ((CharacteristicCollection)target).ResetAllValue();
            }
        }
    }
}
#endif