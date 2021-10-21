using System;
using System.Collections.Generic;
using System.Linq;
using Characteristic.Runtime.Scripts.Manager;
using Characteristic.Runtime.Scripts.ScriptableObject;
using GameFramework.Resource;
using GameMain.Scripts.Base.Struct;
using GameMain.Scripts.Component.Mono;
using GameMain.Scripts.GameLogic.Base;
using GameMain.Scripts.Runtime.Utility;
using Inventory.Runtime.Scripts.Manager;
using Inventory.Runtime.Scripts.ScriptableObject;
using Inventory.Scripts.ScriptableObject;
using MissionSystem.Runtime.Scripts.Manager;
using MissionSystem.Runtime.Scripts.ScriptableObject;
using Narrative.Runtime.Scripts.Graph;
using Narrative.Runtime.Scripts.MonoBehaviour;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Runtime.Base.GameEntry;

namespace GameMain.Scripts.GameLogic
{
    public class ToiletTextGameLogic : GameLogicBase
    {
        public override Progress Progress
        {
            get
            {
                if (_progress == null)
                {
                    _progress = new Progress();
                    _progress.name = "ToiletText";
                    _progress.sceneId = 2;
                }

                return _progress;
            }
            set { _progress = value; }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("游戏逻辑进入");


            LoadNarrativeData(); //加载叙事数据
            LoadNarrativeGraphLocation(); //加载叙事图位置
            LoadMissionData(); //加载任务数据
            LoadItemData(); //加载物品数据
            LoadCharacteristicData(); //加载属性数据

            NarrativeSaveHandler(); //叙事保存处理
            MissionSaveHandler(); //任务保存处理
            InventorySaveHandler(); //物品保存处理
            CharacteristicSaveHandler(); //属性保存处理
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //Debug.Log("游戏逻辑更新");
        }

        public override void OnLeave()
        {
            base.OnLeave();

            Debug.Log("游戏逻辑退出");
        }


        #region PrivateMethod

        #region CharacteristicModule

        public void LoadCharacteristicData()
        {
            var collectionData = Progress.GetData<CharacteristicCollectionData>("CharacteristicCollectionData");
            if (collectionData != null)
            {
                foreach (var characteristicData in collectionData.characteristicDatas)
                {
                    var unit = CharacteristicManager.Collection.FindUnitByName(characteristicData.characteristicName);
                    unit.intvalue = characteristicData.intvalue;
                    unit.floatvalue = characteristicData.floatvalue;
                    unit.boolvalue = characteristicData.boolvalue;
                    unit.stringvalue = characteristicData.stringvalue;
                }
            }
        }

        public void CharacteristicSaveHandler()
        {
            CharacteristicManager.Instance.onValueUpdate += delegate(CharacteristicUnit unit)
            {
                SaveCharacteristicData();
            };
            CharacteristicManager.Instance.onValueAdd += delegate(CharacteristicUnit unit)
            {
                SaveCharacteristicData();
            };
            CharacteristicManager.Instance.onValueLess += delegate(CharacteristicUnit unit)
            {
                SaveCharacteristicData();
            };
        }

        /// <summary>
        /// 保存属性数据
        /// </summary>
        public void SaveCharacteristicData()
        {
            var collectionData = new CharacteristicCollectionData();
            foreach (var characteristicUnit in CharacteristicManager.Collection.CharacteristicUnits)
            {
                var data = new CharacteristicData();
                data.characteristicName = characteristicUnit.name;
                data.intvalue = characteristicUnit.intvalue;
                data.floatvalue = characteristicUnit.floatvalue;
                data.boolvalue = characteristicUnit.boolvalue;
                data.stringvalue = characteristicUnit.stringvalue;
                collectionData.characteristicDatas.Add(data);
            }

            Progress.SetData("CharacteristicCollectionData", collectionData);
        }


        [Serializable]
        public class CharacteristicData
        {
            public string characteristicName;
            public int intvalue;
            public float floatvalue;
            public string stringvalue;
            public bool boolvalue;
        }

        [Serializable]
        public class CharacteristicCollectionData
        {
            public List<CharacteristicData> characteristicDatas = new List<CharacteristicData>();
        }

        #endregion

        #region InventoryModule

        public void InventorySaveHandler()
        {
            InventoryManager.Instance.Package.onItemAdd += delegate(ItemStack stack) { SaveItemData(); };
            InventoryManager.Instance.Package.onItemReduce += delegate(ItemStack stack) { SaveItemData(); };
        }

        public void SaveItemData()
        {
            ItemData data = new ItemData();
            foreach (var itemStack in InventoryManager.Instance.Package.Items)
            {
                data.itemsName.Add(itemStack.Item.name);
                data.itemsAmount.Add(itemStack.Amount);
            }

            Progress.SetData("InventoryData", data);
        }

        public void LoadItemData()
        {
            var data = Progress.GetData<ItemData>("InventoryData");
            InventoryManager.Instance.Package.Items.Clear();

            if (data == null) return;

            for (int i = 0; i < data.itemsName.Count; i++)
            {
                var itemName = data.itemsName[i];
                var amount = data.itemsAmount[i];


                var manager = AssetLibraryManager.Instance;
                if (manager)
                {
                    var item = manager.GetAsset<InventoryItem>(itemName);
                    if (item)
                        InventoryManager.Instance.Package.Items.Add(ItemStack.Create(item, amount));
                }

                // GameEntry.Resource.LoadAsset(AssetUtility.GetInventoryItemAsset(itemName), new LoadAssetCallbacks(
                //     (assetName, asset, duration, userData) =>
                //     {
                //         InventoryManager.Instance.Package.Items.Add(ItemStack.Create((InventoryItem)asset, amount));
                //     },
                //     (assetName, status, errorMessage, userData) => { }));
            }
        }

        [Serializable]
        public class ItemData
        {
            public List<string> itemsName = new List<string>();
            public List<int> itemsAmount = new List<int>();
        }

        #endregion

        #region NarrativeModule

        public void NarrativeSaveHandler()
        {
            NarrativeManager.Instance.DataBase.OndatasChanged += delegate(string json)
            {
                Progress.SetData("NarrativeData", json);
            };

            NarrativeManager.Instance.OnNarrativeNodeChangeEvent += delegate(DisplayNode node)
            {
                var graph = NarrativeManager.Instance.Graph;
                var locationData = new NarrativeGraphLocationData
                {
                    GraphName = graph.name.Replace("(Clone)", ""),
                    NodeIndex = graph.GetIndexByNode(graph.CurrentNode),
                };
                Progress.SetData("NarrativeLocation", locationData);
            };
        }

        /// <summary>
        /// 加载叙事节点数据
        /// </summary>
        public void LoadNarrativeData()
        {
            var jsonData = Progress.GetData<string>("NarrativeData");
            if (!string.IsNullOrWhiteSpace(jsonData))
                NarrativeManager.Instance.DataBase.InitData(jsonData);
        }

        public void LoadNarrativeGraphLocation()
        {
            var locationDataKey = "NarrativeLocation";
            NarrativeManager.Instance.Graph = null;
            var manager = AssetLibraryManager.Instance;
            if (Progress.HasData(locationDataKey)) //如果已经有叙事进度保存
            {
                var locationData = Progress.GetData<NarrativeGraphLocationData>(locationDataKey);
                if (locationData != null)
                {
                    if (manager)
                    {
                        var graph = manager.GetAsset<NarrativeGraph>(locationData.GraphName);
                        if (graph)
                            NarrativeManager.Instance.PlayNarrative(graph, locationData.NodeIndex);
                    }

                    // Runtime.Base.GameEntry.Resource.LoadAsset(AssetUtility.GetNarrativeGraphAsset(locationData.GraphName),
                    //     new LoadAssetCallbacks(
                    //         (assetName, asset, duration, userData) =>
                    //         {
                    //             NarrativeManager.Instance.StartNarrative(asset as NarrativeGraph);
                    //             NarrativeManager.Instance.Graph.SwitchNode(locationData.NodeGuid);
                    //         },
                    //         (assetName, status, errorMessage, userData) => { }));
                }
            }
            else//SetDefaultNode
            {
                if (manager)
                {
                    var graph = manager.GetAsset<NarrativeGraph>("开始剧情");
                    if (graph)
                        NarrativeManager.Instance.PlayNarrative(graph);
                }
            }
            
            //NarrativeManager.Instance.Graph.CurrentNode;
        }

        [Serializable]
        public class NarrativeGraphLocationData
        {
            public string GraphName;
            public int NodeIndex;
        }

        #endregion

        #region MissionModule

        public void MissionSaveHandler()
        {
            MissionManager.Instance.OnMissionAdded += delegate(MissionConfig config) { SaveMissionData(); };
            MissionManager.Instance.OnMissionCompleted += delegate(MissionConfig config) { SaveMissionData(); };
            MissionManager.Instance.OnMissionRemoved += delegate(MissionConfig config) { SaveMissionData(); };
        }


        public void LoadMissionData()
        {
            if (Progress == null) return;

            var data = Progress.GetData<MissionData>("MissionData");
            if (data == null) return;
            var currentNames = data.currentMissionsName;
            var completeNames = data.completeMissionsName;
            if (currentNames != null)
            {
                foreach (var name in currentNames)
                {
                    var manager = AssetLibraryManager.Instance;
                    if (manager)
                    {
                        var missionConfig = manager.GetAsset<MissionConfig>(name);
                        MissionManager.Instance.InitCurrentMission(missionConfig);
                    }

                    // GameEntry.Resource.LoadAsset(AssetUtility.GetMissionConfigAsset(name), new LoadAssetCallbacks(
                    //     (assetName, asset, duration, userData) =>
                    //     {
                    //         MissionManager.Instance.InitCurrentMission(asset as MissionConfig);
                    //     },
                    //     (assetName, status, errorMessage, userData) => { }));
                }
            }

            if (completeNames != null)
            {
                foreach (var name in completeNames)
                {
                    var manager = AssetLibraryManager.Instance;
                    if (manager)
                    {
                        var missionConfig = manager.GetAsset<MissionConfig>(name);
                        MissionManager.Instance.InitCompleteMission(missionConfig);
                    }

                    // GameEntry.Resource.LoadAsset(AssetUtility.GetMissionConfigAsset(name), new LoadAssetCallbacks(
                    //     (assetName, asset, duration, userData) =>
                    //     {
                    //         MissionManager.Instance.InitCompleteMission(asset as MissionConfig);
                    //     },
                    //     (assetName, status, errorMessage, userData) => { }));
                }
            }
        }

        public void SaveMissionData()
        {
            if (Progress == null) return;

            MissionData data = new MissionData();
            foreach (var config in MissionManager.Instance.MissionLine.currentMissions)
                data.currentMissionsName.Add(config.name);
            foreach (var config in MissionManager.Instance.MissionLine.completeMissions)
                data.completeMissionsName.Add(config.name);

            Progress.SetData("MissionData", data);
        }

        [Serializable]
        public class MissionData
        {
            public List<string> currentMissionsName = new List<string>();
            public List<string> completeMissionsName = new List<string>();
        }

        #endregion

        #endregion
    }
}