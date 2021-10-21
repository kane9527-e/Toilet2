using System;
using System.Collections.Generic;
using GameMain.Scripts.Runtime.Message;
using GameMain.Scripts.Runtime.ScrptableObject;
using GameMain.Scripts.UI.GamePlay.InventoryUI;
using Inventory.Runtime.Scripts.Manager;
using Inventory.Runtime.Scripts.ScriptableObject;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Runtime.Base.GameEntry;

public class InventoryUIForm : UIFormLogic
{
    [Header("物品网格UI设置")] [SerializeField] private RectTransform itemGridRoot; //物品根目标
    [SerializeField] private GameObject itemGridUIPrefab; //物品格子UI
    [Header("物品信息设置")] [SerializeField] private Image itemImage; //物品图标
    [SerializeField] private Text itemInfoText; //物品信息文本
    [Header("其他设置")] [SerializeField] private RectTransform rootPanel;
    [SerializeField] private RectTransform itemPanel;
    [SerializeField] private Button closeButton; //关闭按钮
    [SerializeField] private ItemUsableOptionsPanel usableOptionsPanel;

    [Header("按钮操作设置")] [SerializeField] private Button useButton; //使用按钮
    [SerializeField] private Button discardButton; //丢弃按钮
    [SerializeField] private Button disSelectButton; //取消选择按钮
    public List<ItemUIGrid> ItemUIGrids { get; } = new List<ItemUIGrid>();

    public ItemUIGrid CurrentSelectGrid { get; private set; }

    public static bool IsInited { get; private set; }
    public bool IsUsing { get; private set; }

    private void Awake()
    {
        //if (!GameEntry.Base) OnInit(this);
        OnInit(this);
    }

    // private void OnEnable()
    // {
    //     if (!GameEntry.Base)
    //     {
    //         OnOpen(this);
    //     }
    // }

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        InitEvent();
        CloseUIForm();
        ForceUpdateAllUIGrid();
        IsInited = true;
        if (usableOptionsPanel)
            usableOptionsPanel.Init(this);
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        if (rootPanel)
            rootPanel.gameObject.SetActive(true);
        
        ForceUpdateAllUIGrid();
        
        UnUseCurrentItem();
        DisCurrentSelectGrid();
        
        // if (usableOptionsPanel)
        //     usableOptionsPanel.gameObject.SetActive(false);
    }

    protected override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);
        if (!GameEntry.Base)
            if (rootPanel)
                rootPanel.gameObject.SetActive(false);
        
        UnUseCurrentItem();
        DisCurrentSelectGrid();
    }

    protected override void OnRecycle()
    {
        base.OnRecycle();
        if (InventoryManager.Instance)
        {
            InventoryManager.Instance.ONItemAdd -= OnItemAddedHandler;
            InventoryManager.Instance.ONItemReduce -= OnItemReducedHandler;
            InventoryManager.Instance.ONItemUse -= OnItemUsedHandler;
        }
    }

    private void InitEvent()
    {
        if (InventoryManager.Instance)
        {
            InventoryManager.Instance.ONItemAdd += OnItemAddedHandler;
            InventoryManager.Instance.ONItemReduce += OnItemReducedHandler;
            InventoryManager.Instance.ONItemUse += OnItemUsedHandler;
        }

        if (closeButton)
            closeButton.onClick.AddListener(CloseUIForm);
        if (disSelectButton)
            disSelectButton.onClick.AddListener(DisCurrentSelectGrid);
        if (useButton)
            useButton.onClick.AddListener(UnOrUseCurrentItem);
        if (discardButton)
            discardButton.onClick.AddListener(DiscardCurrentItem);
    }


    #region EventHandler

    private void OnItemUsedHandler(ItemStack stack)
    {
        var grid = FindGrid(stack);
        if (!grid) return;
        grid.UpdateItemGridUI();
    }

    private void OnItemReducedHandler(ItemStack stack)
    {
        var grid = FindGrid(stack);
        if (!grid) return;
        if (!InventoryManager.Instance.Package.Items.Contains(stack) || stack.Amount <= 0)
        {
            ItemUIGrids.Remove(grid);
            Destroy(grid.gameObject);
            if (CurrentSelectGrid != null && grid == CurrentSelectGrid)
                CurrentSelectGrid = null;
        }

        if (grid)
            grid.UpdateItemGridUI();
        UpdateItemPanelStatus(); //更新Panel状态
    }

    private void OnItemAddedHandler(ItemStack stack)
    {
        var grid = FindGrid(stack);
        if (!grid)
            grid = CreateNewGrid(stack);
        else
            grid.UpdateItemGridUI();

        if (stack.Item.UserDataObject && stack.Item.UserDataObject is ItemMessageObject)
        {
            var msgObj = (ItemMessageObject)stack.Item.UserDataObject;
            msgObj.RandomPushAddMsg();
        }
        else if (!stack.Item.UserDataObject)
        {
            MessageSystem.PushMessage(string.Format("你获得了\n{0}", stack.Item.ItemName));
        }
    }

    public void CloseUIForm()
    {
        OnClose(true, this);
    }

    public void OpenUIForm()
    {
        OnOpen(this);
    }

    #endregion

    #region PrivateMethod

    private ItemUIGrid FindGrid(ItemStack stack)
    {
        return ItemUIGrids.Find(g => g.Stack == stack);
    }

    private ItemUIGrid CreateNewGrid(ItemStack stack)
    {
        var gridUI = Instantiate(itemGridUIPrefab, itemGridRoot).GetComponent<ItemUIGrid>();
        gridUI.InitGrid(this, stack);
        if (!ItemUIGrids.Contains(gridUI))
            ItemUIGrids.Add(gridUI);
        return gridUI;
    }

    /// <summary>
    ///     更新物品面板状态
    /// </summary>
    private void UpdateItemPanelStatus()
    {
        if (CurrentSelectGrid)
        {
            var item = CurrentSelectGrid.Stack.Item;
            useButton.gameObject.SetActive(item.ItemType.CanUse);
            try
            {
                var text = (Text)useButton.targetGraphic;
                text.text = IsUsing ? "取消使用" : "使用物品";
            }
            // ReSharper disable once EmptyGeneralCatchClause
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                //ignore
            }

            discardButton.gameObject.SetActive(item.ItemType.CanDiscard);
            if (item.ItemIcon)
                itemImage.sprite = Sprite.Create(item.ItemIcon, new Rect(0, 0, item.ItemIcon.width, item.ItemIcon.height),
                    Vector2.zero);
            itemInfoText.text = item.ItemInfo;
        }
 

        discardButton.gameObject.SetActive(!IsUsing);
        disSelectButton.gameObject.SetActive(!IsUsing); //取消选择按钮
        
        if (itemPanel)
            itemPanel.gameObject.SetActive(CurrentSelectGrid != null);
        if (usableOptionsPanel && itemPanel && !itemPanel.gameObject.activeSelf)
            usableOptionsPanel.gameObject.SetActive(false);
    }

    private void ForceUpdateAllUIGrid()
    {
        for (var i = ItemUIGrids.Count - 1; i >= 0; i--)
        {
            var girdUI = ItemUIGrids[i];
            ItemUIGrids.Remove(girdUI);
            Destroy(girdUI.gameObject);
        }

        var itemstack = InventoryManager.Instance.Package.Items;
        foreach (var stack in itemstack) CreateNewGrid(stack);
    }

    #endregion

    #region PublicMethod

    public void SelectGrid(ItemUIGrid grid)
    {
        CurrentSelectGrid = grid;
        UpdateItemPanelStatus();
    }

    public void DisCurrentSelectGrid()
    {
        CurrentSelectGrid = null;
        IsUsing = false;
        UpdateItemPanelStatus();
    }

    /// <summary>
    ///     使用当前物品
    /// </summary>
    public void UnOrUseCurrentItem()
    {
        if (!CurrentSelectGrid) return;

        IsUsing = !IsUsing;


        if (IsUsing)
            UseCurrentItem();
        else
            UnUseCurrentItem();
    }

    /// <summary>
    ///     使用当前物品
    /// </summary>
    public void UseCurrentItem()
    {
        IsUsing = true;
        var item = CurrentSelectGrid.Stack.Item;
        if (!item.ItemType.CanUse) return;
        var eventCount = item.onUseEvent.GetPersistentEventCount();
        for (var i = 0; i < eventCount; i++)
        {
            var eventTarget = item.onUseEvent.GetPersistentTarget(i);
            if (eventTarget.GetType() == typeof(ItemUsableConfig))
            {
                var usableConfig = (ItemUsableConfig)eventTarget;
                if (usableOptionsPanel)
                {
                    usableOptionsPanel.gameObject.SetActive(true);
                    usableOptionsPanel.OpenUsableConfig(usableConfig);
                }

                break;
            }
        }

        if (item.UserDataObject && item.UserDataObject is ItemMessageObject)
        {
            var msgObj = (ItemMessageObject)item.UserDataObject;
            msgObj.RandomPushUseMsg();
        }

        InventoryManager.Instance.UseItem(item);
        UpdateItemPanelStatus();
    }

    /// <summary>
    ///     取消使用当前物品
    /// </summary>
    public void UnUseCurrentItem()
    {
        IsUsing = false;
        // ReSharper disable once Unity.NoNullPropagation
        if (usableOptionsPanel && usableOptionsPanel.gameObject.activeSelf)
            usableOptionsPanel.gameObject?.SetActive(false);
        
        UpdateItemPanelStatus();
    }

    /// <summary>
    ///     丢弃当前物品
    /// </summary>
    public void DiscardCurrentItem()
    {
        if (!CurrentSelectGrid) return;
        var item = CurrentSelectGrid.Stack.Item;
        if (!item.ItemType.CanDiscard) return;
        if (item.UserDataObject && item.UserDataObject is ItemMessageObject)
        {
            var msgObj = (ItemMessageObject)item.UserDataObject;
            msgObj.RandomPushReduceMsg();
        }
        else if (!item.UserDataObject)
        {
            MessageSystem.PushMessage(string.Format("你将\n{0}\n丢弃了", item.ItemName));
        }

        InventoryManager.Instance.ReduceItem(item);
    }

    #endregion
}