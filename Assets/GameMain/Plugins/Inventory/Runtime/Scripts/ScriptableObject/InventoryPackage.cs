using System;
using System.Collections.Generic;
using Inventory.Scripts.ScriptableObject;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Inventory.Runtime.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "NewInventoryPackage", menuName = "Inventory/Package")]
    public class InventoryPackage : UnityEngine.ScriptableObject
    {
        [SerializeField] private List<ItemStack> items;
        public List<ItemStack> Items => items;

        #region Event

        public Action<ItemStack> onItemAdd; //当物品被添加
        public Action<ItemStack> onItemReduce; //当物品减少
        public Action<ItemStack> onItemUse; //当物品使用
        
        #endregion

        #region Method

        // public bool HasItem(string itemName, int amount = 1)
        // {
        //     if (items == null || items.Count <= 0) return false;
        //     return items.Exists(i => i.Item.ItemName == itemName && i.Amount == amount);
        // }

        /// <summary>
        ///     检测是否有物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool HasItem(InventoryItem item, int amount = 1)
        {
            if (items == null || items.Count <= 0) return false;
            return items.Exists(i => i.Item.Equals(item) && i.Amount == amount);
        }

        /// <summary>
        ///     检测是否有物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool HasItem(InventoryItem item)
        {
            if (items == null || items.Count <= 0) return false;
            return items.Exists(i => i.Item.Equals(item));
        }
        /// <summary>
        ///     减少物品
        /// </summary>
        public void ReduceItem(InventoryItem item)
        {
            if (items == null || items.Count <= 0) return;
            var findResult = items.Find(i => i.Item == item);
            if (findResult != null) //如果找到了物品
            {
                if (findResult.Amount > 1)
                    findResult.ReduceCount();
                else items.Remove(findResult);
                onItemReduce?.Invoke(findResult);
            }
        }
        
        /// <summary>
        ///     增加物品
        /// </summary>
        public void AddItem(InventoryItem item)
        {
            if (items == null || items.Count <= 0) items = new List<ItemStack>();
            var findResult = items.Find(i => i.Item == item);
            if (findResult == null) //如果找到了物品
            {
                findResult = ItemStack.Create(item);
                items.Add(findResult);
            }
            else
            {
                findResult.AddCount();
            }

            onItemAdd?.Invoke(findResult);
        }

        /// <summary>
        ///     使用物品
        /// </summary>
        /// <param name="item"></param>
        public void UseItem(InventoryItem item)
        {
            var findResult = items.Find(i => i.Item == item);
            if (findResult == null) return;
            item.Use();
            onItemUse?.Invoke(findResult);
        }

        #endregion
    }

    [Serializable]
    public class ItemStack
    {
        [SerializeField] private InventoryItem item;
        [SerializeField] private int amount;

        private ItemStack(InventoryItem item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }

        public InventoryItem Item => item;
        public int Amount => amount;

        #region Method

        public void ReduceCount()
        {
            if (amount > 0)
                amount--;
        }

        public void AddCount()
        {
            if (item.MAXAmount <= 0 || item.MAXAmount > 0 && amount < item.MAXAmount)
                amount++;
        }

        public static ItemStack Create([NotNull] InventoryItem item,int amount=1)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var result = new ItemStack(item, amount);
            return result;
        }

        #endregion
    }
}