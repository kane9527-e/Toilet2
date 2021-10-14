using Inventory.Runtime.Scripts.ScriptableObject;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace Inventory.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Inventory/Item")]
    public class InventoryItem : UnityEngine.ScriptableObject
    {
        [SerializeField] private string itemName; //物品名称
        [SerializeField] private InventoryItemType itemType; //物品类型
        [SerializeField] private GameObject itemPrefab; //物品预制物
        [SerializeField] private Texture2D itemIcon; //物品图标
        [TextArea] [SerializeField] private string itemInfo; //物品信息
        [SerializeField] private int maxAmount = -1; //最大数量
        [SerializeField] public UnityEvent onUseEvent;
        [SerializeField] private UnityEngine.ScriptableObject userDataObject;

        #region PublicMethod

        /// <summary>
        ///     使用物品
        /// </summary>
        public virtual void Use()
        {
            // ReSharper disable once Unity.NoNullPropagation
            //ItemUsableManager.Instance?.OpenItemUsableConfigs(this);
            onUseEvent?.Invoke();
        }


        // public List<ItemUsableConfig.UsableOption> GetAllUsableOptions()
        // {
        //     List<ItemUsableConfig.UsableOption> usableOptions = new List<ItemUsableConfig.UsableOption>();
        //     
        //     foreach (var config in usableConfigs)
        //         usableOptions.AddRange(config.UsableOptions);
        //     return usableOptions;
        // }

        #endregion

        // ReSharper disable once InconsistentNaming
        //[SerializeField] private List<ItemUsableConfig> usableConfigs = new List<ItemUsableConfig>();

        #region ReadOnlyProperty

        public string ItemName => itemName;
        public string ItemInfo => itemInfo;
        public Texture2D ItemIcon => itemIcon;
        public int MAXAmount => maxAmount;
        public InventoryItemType ItemType => itemType;

        public UnityEngine.ScriptableObject UserDataObject => userDataObject;
        //public List<ItemUsableConfig> UsableConfigs => usableConfigs;

        #endregion

#if UNITY_EDITOR
        public string EditorSetItemName
        {
            set => itemName = value;
        }

        public string EditorSetItemInfo
        {
            set => itemInfo = value;
        }
#endif
    }
}