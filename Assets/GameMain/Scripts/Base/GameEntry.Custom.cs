using GameMain.Scripts.Component.GameFramework;

namespace GameMain.Scripts.Runtime.Base
{
    /// <summary>
    ///     游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        /// <summary>
        ///     获取进度组件。
        /// </summary>
        public static ProgressComponent Progress { get; private set; }
        private static void InitCustomComponents()
        {
            Progress = UnityGameFramework.Runtime.GameEntry.GetComponent<ProgressComponent>();
        }
    }
}