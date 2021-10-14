using System;
using Inventory.Runtime.Scripts.ScriptableObject;
using Inventory.Scripts.ScriptableObject;
using UnityEngine;

namespace Inventory.Runtime.Scripts.Manager
{
    [DisallowMultipleComponent]
    public class InventoryManager : MonoBehaviour
    {
        #region Field

        [SerializeField] private InventoryPackage currentPackage; //当前背包

        #endregion

        #region Property

        public InventoryPackage Package => currentPackage;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            if (Instance == this)
                DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Singleton

        private static InventoryManager _instance;

        public static InventoryManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<InventoryManager>();
                    if (!_instance)
                        _instance = new GameObject(nameof(InventoryManager)).AddComponent<InventoryManager>();
                }

                return _instance;
            }
        }

        #endregion

        #region Event

        public Action<ItemStack> ONItemAdd
        {
            get => Package.onItemAdd;
            set => Package.onItemAdd = value;
        }

        public Action<ItemStack> ONItemReduce
        {
            get => Package.onItemReduce;
            set => Package.onItemReduce = value;
        } //当物品减少

        public Action<ItemStack> ONItemUse
        {
            get => Package.onItemUse;
            set => Package.onItemUse = value;
        } //当物品使用

        #endregion

        #region Method

        public void AddItem(InventoryItem item)
        {
            if (!item) return;
            Package.AddItem(item);
        }

        public void ReduceItem(InventoryItem item)
        {
            if (!item) return;
            Package.ReduceItem(item);
        }

        public void UseItem(InventoryItem item)
        {
            if (!item) return;
            Package.UseItem(item);
        }

        #endregion
    }
}