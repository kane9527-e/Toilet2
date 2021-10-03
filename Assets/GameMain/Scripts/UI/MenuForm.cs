using GameMain.Scripts.Procedure;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.UI
{
    public class MenuForm : UIFormLogic
    {
        #region SerializeFields

        [SerializeField] private Button startGameButton;

        #endregion

        private ProcedureMenuLogic m_ProcedureMenu = null;

        protected override void OnInit(object userData)
        {
            if (startGameButton)
                startGameButton.onClick.AddListener(OnStartGameButtonClick);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnOpen(object userData)
#else
        protected internal override void OnOpen(object userData)
#endif
        {
            m_ProcedureMenu = (ProcedureMenuLogic) userData;
            if (m_ProcedureMenu == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
        }

        private void OnStartGameButtonClick()
        {
            m_ProcedureMenu.StartGame();
        }
    }
}