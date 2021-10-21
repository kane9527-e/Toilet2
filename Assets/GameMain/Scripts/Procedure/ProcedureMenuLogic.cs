using GameFramework.Fsm;
using GameFramework.Procedure;
using GameMain.Scripts.DataTable;
using GameMain.Scripts.Runtime.Utility;
using GameMain.Scripts.UI.Extensions;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Runtime.Base.GameEntry;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureMenuLogic : ProcedureBase
    {
        private IFsm<IProcedureManager> m_CurrentOwner;
        public override bool UseNativeDialog { get; }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            m_CurrentOwner = procedureOwner;
            GameEntry.Scene.RefreshMainCamera();

            var dRuiForms = GameEntry.DataTable.GetDataTable<DRUIForm>();
            var row = dRuiForms.GetDataRow((int)UIFormId.MenuForm);
            if (!GameEntry.UI.HasUIGroup(row.UIGroupName))
                GameEntry.UI.AddUIGroup(row.UIGroupName);
            GameEntry.UI.OpenUIForm(AssetUtility.GetUIFormAsset(row.AssetName), row.UIGroupName, this);
            //方便开发直接进入游戏
            StartGame();
        }

        public void StartGame()
        {
            var progressName = GameEntry.Progress.SaveProgressName;
            m_CurrentOwner.SetData<VarString>("ProgressName", progressName);
            ChangeState<ProcedureGameLogic>(m_CurrentOwner);
        }
    }
}