using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureGameLogic : ProcedureBase
    {
        public override bool UseNativeDialog { get; }
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            //TODO
            //读取当前游戏场景
            //并且加载进入对应场景和流程
        }
    }
}