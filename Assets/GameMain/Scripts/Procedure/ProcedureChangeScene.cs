﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Event;
using GameMain.Scripts.DataTable;
using GameMain.Scripts.Definition.Constant;
using GameMain.Scripts.Runtime.Utility;
using GameMain.Scripts.Scene;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Runtime.Base.GameEntry;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;


namespace GameMain.Scripts.Procedure
{
    public class ProcedureChangeScene : ProcedureBase
    {
        private bool m_ChangeToMenu;

        private bool m_IsChangeSceneComplete;

        public override bool UseNativeDialog { get; }
        //private int m_BackgroundMusicId = 0;


        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_IsChangeSceneComplete = false;

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            GameEntry.Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);

            // 停止所有声音
            GameEntry.Sound.StopAllLoadingSounds();
            GameEntry.Sound.StopAllLoadedSounds();

            // 隐藏所有实体
            GameEntry.Entity.HideAllLoadingEntities();
            GameEntry.Entity.HideAllLoadedEntities();

            // 卸载所有场景
            var loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            for (var i = 0; i < loadedSceneAssetNames.Length; i++)
                GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);

            // 还原游戏速度
            GameEntry.Base.ResetNormalGameSpeed();

            int sceneId = procedureOwner.GetData<VarInt32>("NextSceneId");
            m_ChangeToMenu = sceneId == (int)SceneId.MenuScene;


            var dtScene = GameEntry.DataTable.GetDataTable<DRScene>();
            var drScene = dtScene.GetDataRow(sceneId);
            if (drScene == null)
            {
                Log.Warning("Can not load scene '{0}' from data table.", sceneId.ToString());
                return;
            }

            GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset,
                this);
            //m_BackgroundMusicId = drScene.BackgroundMusicId;
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            GameEntry.Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_IsChangeSceneComplete) return;

            if (m_ChangeToMenu)
                ChangeState<ProcedureMenuLogic>(procedureOwner);
            else
                ChangeState<ProcedureGameLogic>(procedureOwner);
        }

        private void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            var ne = (LoadSceneSuccessEventArgs)e;
            if (ne.UserData != this) return;

            Log.Info("Load scene '{0}' OK.", ne.SceneAssetName);

            m_IsChangeSceneComplete = true;
        }

        private void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            var ne = (LoadSceneFailureEventArgs)e;
            if (ne.UserData != this) return;

            Log.Error("Load scene '{0}' failure, error message '{1}'.", ne.SceneAssetName, ne.ErrorMessage);
        }

        private void OnLoadSceneUpdate(object sender, GameEventArgs e)
        {
            var ne = (LoadSceneUpdateEventArgs)e;
            if (ne.UserData != this) return;

            Log.Info("Load scene '{0}' update, progress '{1}'.", ne.SceneAssetName, ne.Progress.ToString("P2"));
        }

        private void OnLoadSceneDependencyAsset(object sender, GameEventArgs e)
        {
            var ne = (LoadSceneDependencyAssetEventArgs)e;
            if (ne.UserData != this) return;

            Log.Info("Load scene '{0}' dependency asset '{1}', count '{2}/{3}'.", ne.SceneAssetName,
                ne.DependencyAssetName, ne.LoadedCount.ToString(), ne.TotalCount.ToString());
        }
    }
}