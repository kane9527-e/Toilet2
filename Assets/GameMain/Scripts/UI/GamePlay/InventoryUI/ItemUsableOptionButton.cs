using Inventory.Runtime.Scripts.ScriptableObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMain.Scripts.UI.GamePlay.InventoryUI
{
    public class ItemUsableOptionButton : MonoBehaviour, IPointerClickHandler
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ItemUsableOptionsPanel Panel { get; private set; }
        public ItemUsableConfig.UsableOption UsableOption { get; private set; }
        [SerializeField] private Text optionText;

        #region PublicMethod

        public void UpdateOption(ItemUsableOptionsPanel panel, ItemUsableConfig.UsableOption option)
        {
            if (Panel != panel)
                Panel = panel;
            this.UsableOption = option;
            if (optionText)
                optionText.text = UsableOption.OptionName;
        }

        #endregion

        #region PrivateMethod

        private void ExecuteUseAction()
        {
            UsableOption?.Action?.Invoke();
            
        }

        #endregion

        public void OnPointerClick(PointerEventData eventData)
        {
            ExecuteUseAction();
            if(Panel)
                Panel.ClosePanel();
        }
    }
}