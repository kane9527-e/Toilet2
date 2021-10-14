﻿using UnityEditor;

namespace VisualGraphEditor
{
    public static class VisualGraphSettings
    {
        public static bool autoSave = true;

        public static void Save()
        {
            if (autoSave) AssetDatabase.SaveAssets();
        }
    }
}