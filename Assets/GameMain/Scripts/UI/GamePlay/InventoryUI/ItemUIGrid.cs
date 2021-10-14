using Inventory.Runtime.Scripts.ScriptableObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUIGrid : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemAmountText;
    private InventoryUIForm _parent;
    public ItemStack Stack { get; private set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        _parent.SelectGrid(this);
    }

    public void InitGrid(InventoryUIForm inventoryUIForm, ItemStack stack)
    {
        _parent = inventoryUIForm;
        Stack = stack;
        UpdateItemGridUI();
    }

    public void UpdateItemGridUI()
    {
        if (itemImage && Stack.Item.ItemIcon)
            itemImage.sprite = Sprite.Create(Stack.Item.ItemIcon,
                new Rect(0, 0, Stack.Item.ItemIcon.width, Stack.Item.ItemIcon.height), Vector2.zero);
        if (itemNameText && Stack.Item.ItemName != null)
            itemNameText.text = Stack.Item.ItemName;
        if (itemAmountText)
            itemAmountText.text = string.Format("x{0}", Stack.Amount);
    }
}