using System;
using System.Collections.Generic;
using System.Reflection;
using GameMain.Scripts.Base.Struct;
using GameMain.Scripts.DataTable;
using GameMain.Scripts.GameLogic.Base;
using UnityEngine;
using UnityGameFramework.Runtime;
using AssetUtility = GameMain.Scripts.Runtime.Utility.AssetUtility;
using GameEntry = GameMain.Scripts.Runtime.Base.GameEntry;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Scripts.Procedure
{
    public class ProcedureGameLogic : ProcedureBase
    {
        private List<GameLogicBase> _gameLogics = new List<GameLogicBase>();

        private readonly string _defaultProgressName = "ToiletText";

        private ProcedureOwner m_CurrentOwner;

        private GameLogicBase currentLogic; //当前逻辑
        private bool sceneisloaded;


        public override bool UseNativeDialog { get; }

        #region ProcedureBehaviour

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            _gameLogics = GetAllReflectionClassIns<GameLogicBase>("Assembly-CSharp");
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            //TODO
            //读取当前游戏场景
            //并且加载进入对应场景和流程
            m_CurrentOwner = procedureOwner;


            var progressName = m_CurrentOwner.GetData<VarString>("ProgressName").Value;
            progressName = string.IsNullOrWhiteSpace(progressName) ? _defaultProgressName : progressName;
            currentLogic = GetGameLogicByProgressName(progressName);
            if (currentLogic != null) //如果找到游戏处理逻辑类的实例
            {
                if (GameEntry.Progress.HasProgress(currentLogic.Progress.name))
                    currentLogic.Progress = GameEntry.Progress.LoadProgress(currentLogic.Progress.name);
            }


            if (currentLogic == null || currentLogic.Progress == null) return;
            var sceneId = currentLogic.Progress.sceneId;
            sceneisloaded = SceneHasLoaded(sceneId);
            if (!sceneisloaded) //如果场景没有加载
            {
                //加载场景
                var owner = procedureOwner;
                owner.SetData<VarInt32>("NextSceneId", sceneId);
                ChangeState<ProcedureChangeScene>(owner);
            }

            if (sceneisloaded)
                currentLogic.OnEnter();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (sceneisloaded)
                currentLogic?.OnUpdate();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            if (sceneisloaded)
            {
                currentLogic.OnLeave();
                sceneisloaded = false;
            }

            SaveProgress();
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
            //SaveProgress();
        }

        #endregion


        #region PublicMethod

        /// <summary>
        /// 保存游戏数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="data">数据</param>
        public void SaveGameData(string key, object data) => currentLogic.Progress.SetData(key, data);

        /// <summary>
        /// 加载游戏数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public T LoadGameData<T>(string key) => currentLogic.Progress.GetData<T>(key);

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 使用进度名称获取游戏逻辑处理类
        /// </summary>
        /// <param name="progressName"></param>
        /// <returns></returns>
        private GameLogicBase GetGameLogicByProgressName(string progressName) =>
            _gameLogics.Find(logic => logic.Progress.name.Equals(progressName));


        private bool SceneHasLoaded(int sceneId)
        {
            var dtScene = GameEntry.DataTable.GetDataTable<DRScene>();
            var drScene = dtScene.GetDataRow(sceneId);
            var sceneisloaded = false;
            if (drScene != null)
            {
                sceneisloaded = GameEntry.Scene.SceneIsLoaded(AssetUtility.GetSceneAsset(drScene.AssetName));
            }

            return sceneisloaded;
        }

        /// <summary>
        /// 保存进度
        /// </summary>
        private void SaveProgress()
        {
            if (currentLogic != null && currentLogic.Progress != null)
                GameEntry.Progress.SaveProgress(currentLogic.Progress);
        }

        /// <summary>
        /// 获取所有指定的反射类实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetAllReflectionClassIns<T>(string assemblyName) where T : class
        {
            List<T> reference = new List<T>();
            var types = Assembly.Load(assemblyName).GetTypes();
            foreach (var type in types)
            {
                if (type.BaseType == typeof(T) && type != typeof(T))
                {
                    T ins = Activator.CreateInstance(type) as T;
                    reference.Add(ins);
                }
            }

            return reference;
        }

        #endregion
    }
}