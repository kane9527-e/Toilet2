using UnityEngine;

namespace Inventory.Runtime.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "NewInventoryItemType", menuName = "Inventory/Type")]
    public class InventoryItemType : UnityEngine.ScriptableObject
    {
        [SerializeField] private string typeName;
        [SerializeField] private bool canDiscard;
        [SerializeField] private bool canUse;
        public bool CanDiscard => canDiscard;
        public bool CanUse => canUse;
    }
}