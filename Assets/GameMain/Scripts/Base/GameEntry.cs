using UnityEngine;

namespace GameMain.Scripts.Runtime.Base
{
    /// <summary>
    ///     游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        private void Start()
        {
            InitBuiltinComponents();
            InitCustomComponents();
        }
    }
}