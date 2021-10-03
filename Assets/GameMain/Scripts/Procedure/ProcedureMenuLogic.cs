using GameFramework.DataTable;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameMain.Scripts.DataTable;
using GameMain.Scripts.Runtime.Utility;
using GameMain.Scripts.Scene;
using GameMain.Scripts.UI.Extensions;
using GameEntry = GameMain.Scripts.Runtime.Base.GameEntry;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureMenuLogic : ProcedureBase
    {
        public override bool UseNativeDialog { get; }
        private IFsm<IProcedureManager> m_CurrentOwner;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            m_CurrentOwner = procedureOwner;
            GameEntry.Scene.RefreshMainCamera();

            IDataTable<DRUIForm> dRuiForms = GameEntry.DataTable.GetDataTable<DRUIForm>();
            var row = dRuiForms.GetDataRow((int) UIFormId.MenuForm);
            if (!GameEntry.UI.HasUIGroup(row.UIGroupName))
                GameEntry.UI.AddUIGroup(row.UIGroupName);
            GameEntry.UI.OpenUIForm(AssetUtility.GetUIFormAsset(row.AssetName), row.UIGroupName,this);
        }

        public void StartGame()
        {
            ChangeState<ProcedureGameLogic>(m_CurrentOwner);
        }
    }
}