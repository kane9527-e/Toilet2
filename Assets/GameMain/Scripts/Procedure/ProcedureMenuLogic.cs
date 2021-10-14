using GameFramework.Fsm;
using GameFramework.Procedure;
using GameMain.Scripts.DataTable;
using GameMain.Scripts.Runtime.Base;
using GameMain.Scripts.Runtime.Utility;
using GameMain.Scripts.UI.Extensions;

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
        }

        public void StartGame()
        {
            ChangeState<ProcedureGameLogic>(m_CurrentOwner);
        }
    }
}