using GameMain.Scripts.Runtime.Base;

namespace GameMain.Scripts.Utility
{
    public static class UIUtility
    {
        public static void OpenUIForm(string AssetName, string UIGroupName, bool AllowMultiInstance, bool PauseCoveredUIForm, object userData)
        {
            if (!GameEntry.UI.HasUIGroup(UIGroupName))
                GameEntry.UI.AddUIGroup(UIGroupName);
            GameEntry.UI.OpenUIForm(AssetName, UIGroupName,PauseCoveredUIForm, userData);
        }
    }
}