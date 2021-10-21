//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using GameFramework.DataTable;
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
    public class ProcedurePreload : ProcedureBase
    {
        private readonly Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();
        public override bool UseNativeDialog { get; }


        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            // GameEntry.Event.Subscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            // GameEntry.Event.Subscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            // GameEntry.Event.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            // GameEntry.Event.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            m_LoadedFlag.Clear();
            PreloadResources();
        }


        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            // GameEntry.Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            // GameEntry.Event.Unsubscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            // GameEntry.Event.Unsubscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            // GameEntry.Event.Unsubscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            foreach (var loadedFlag in m_LoadedFlag)
                if (!loadedFlag.Value)
                    return;


            procedureOwner.SetData<VarInt32>("NextSceneId", (int)SceneId.MenuScene); //切换主菜单


            ChangeState<ProcedureChangeScene>(procedureOwner);
        }

        private void PreloadResources()
        {
            
            // Preload configs
            //LoadConfig("DefaultConfig");
            // Preload data tables
            foreach (var dataTableName in Constant.DataTable.DataTableNames) LoadDataTable(dataTableName);

            // Preload dictionaries
            //LoadDictionary("Default");

            // Preload fonts
            // LoadFont("MainFont");
            
            GameEntry.Progress.Init();//初始化进度
            
        }

        // private void LoadConfig(string configName)
        // {
        //     string configAssetName = AssetUtility.GetConfigAsset(configName, false);
        //     m_LoadedFlag.Add(configAssetName, false);
        //     GameEntry.Config.ReadData(configAssetName, this);
        // }

        private void LoadDataTable(string dataTableName)
        {
            var dataTableAssetName = AssetUtility.GetDataTableAsset(dataTableName, false);
            m_LoadedFlag.Add(dataTableAssetName, false);
            GameEntry.DataTable.LoadDataTable(dataTableName, dataTableAssetName, this);
        }

        private DRScene GETSceneRowById(IDataTable<DRScene> sceneTables, int id)
        {
            foreach (var drScene in sceneTables)
                if (drScene.Id.Equals(id))
                    return drScene;

            return null;
        }

        // private void LoadDictionary(string dictionaryName)
        // {
        //     string dictionaryAssetName = AssetUtility.GetDictionaryAsset(dictionaryName, false);
        //     m_LoadedFlag.Add(dictionaryAssetName, false);
        //     GameEntry.Localization.ReadData(dictionaryAssetName, this);
        // }
        //
        // private void LoadFont(string fontName)
        // {
        //     m_LoadedFlag.Add(GameFramework.Utility.Text.Format("Font.{0}", fontName), false);
        //     GameEntry.Resource.LoadAsset(AssetUtility.GetFontAsset(fontName), Constant.AssetPriority.FontAsset,
        //         new LoadAssetCallbacks(
        //             (assetName, asset, duration, userData) =>
        //             {
        //                 m_LoadedFlag[GameFramework.Utility.Text.Format("Font.{0}", fontName)] = true;
        //                 UGuiForm.SetMainFont((Font) asset);
        //                 Log.Info("Load font '{0}' OK.", fontName);
        //             },
        //             (assetName, status, errorMessage, userData) =>
        //             {
        //                 Log.Error("Can not load font '{0}' from '{1}' with error message '{2}'.", fontName, assetName,
        //                     errorMessage);
        //             }));
        // }

        // private void OnLoadConfigSuccess(object sender, GameEventArgs e)
        // {
        //     LoadConfigSuccessEventArgs ne = (LoadConfigSuccessEventArgs) e;
        //     if (ne.UserData != this)
        //     {
        //         return;
        //     }
        //
        //     m_LoadedFlag[ne.ConfigAssetName] = true;
        //     Log.Info("Load config '{0}' OK.", ne.ConfigAssetName);
        // }
        //
        // private void OnLoadConfigFailure(object sender, GameEventArgs e)
        // {
        //     LoadConfigFailureEventArgs ne = (LoadConfigFailureEventArgs) e;
        //     if (ne.UserData != this)
        //     {
        //         return;
        //     }
        //
        //     Log.Error("Can not load config '{0}' from '{1}' with error message '{2}'.", ne.ConfigAssetName,
        //         ne.ConfigAssetName, ne.ErrorMessage);
        // }

        private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
        {
            var ne = (LoadDataTableSuccessEventArgs)e;
            if (ne.UserData != this) return;

            m_LoadedFlag[ne.DataTableAssetName] = true;
            Log.Info("Load data table '{0}' OK.", ne.DataTableAssetName);
        }

        private void OnLoadDataTableFailure(object sender, GameEventArgs e)
        {
            var ne = (LoadDataTableFailureEventArgs)e;
            if (ne.UserData != this) return;

            Log.Error("Can not load data table '{0}' from '{1}' with error message '{2}'.", ne.DataTableAssetName,
                ne.DataTableAssetName, ne.ErrorMessage);
        }

        // private void OnLoadDictionarySuccess(object sender, GameEventArgs e)
        // {
        //     LoadDictionarySuccessEventArgs ne = (LoadDictionarySuccessEventArgs) e;
        //     if (ne.UserData != this)
        //     {
        //         return;
        //     }
        //
        //     m_LoadedFlag[ne.DictionaryAssetName] = true;
        //     Log.Info("Load dictionary '{0}' OK.", ne.DictionaryAssetName);
        // }
        //
        // private void OnLoadDictionaryFailure(object sender, GameEventArgs e)
        // {
        //     LoadDictionaryFailureEventArgs ne = (LoadDictionaryFailureEventArgs) e;
        //     if (ne.UserData != this)
        //     {
        //         return;
        //     }
        //
        //     Log.Error("Can not load dictionary '{0}' from '{1}' with error message '{2}'.", ne.DictionaryAssetName,
        //         ne.DictionaryAssetName, ne.ErrorMessage);
        // }
    }
}