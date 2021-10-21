//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameMain.Scripts.Runtime.Base;

namespace GameMain.Scripts.Runtime.Utility
{
    public static class AssetUtility
    {
        public static string GetConfigAsset(string assetName, bool fromBytes)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/Configs/{0}.{1}", assetName,
                fromBytes ? "bytes" : "txt");
        }

        public static string GetDataTableAsset(string assetName, bool fromBytes)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/DataTables/{0}.{1}", assetName,
                fromBytes ? "bytes" : "txt");
        }

        public static string GetDictionaryAsset(string assetName, bool fromBytes)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/Localization/{0}/Dictionaries/{1}.{2}",
                GameEntry.Localization.Language, assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetFontAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/Fonts/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/Entities/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/UI/UISounds/{0}.wav", assetName);
        }

        public static string GetMissionConfigAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/MissionConfigs/{0}.asset", assetName);
        }
        
        public static string GetInventoryItemAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/Inventories/Items/{0}.asset", assetName);
        }
        public static string GetNarrativeGraphAsset(string assetName)
        {
            return GameFramework.Utility.Text.Format("Assets/GameMain/NarrativeGraphs/StoryGraphs/{0}.asset", assetName);
        }
    }
}