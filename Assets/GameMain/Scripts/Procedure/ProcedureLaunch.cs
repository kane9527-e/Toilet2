using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureLaunch : ProcedureBase
    {
        public override bool UseNativeDialog { get; }
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            ChangeState<ProcedurePreload>(procedureOwner);
        }
    }
}