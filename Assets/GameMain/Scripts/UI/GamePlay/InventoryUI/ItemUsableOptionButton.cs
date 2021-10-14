using Inventory.Runtime.Scripts.ScriptableObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMain.Scripts.UI.GamePlay.InventoryUI
{
    public class ItemUsableOptionButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Text optionText;

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ItemUsableOptionsPanel Panel { get; private set; }
        public ItemUsableConfig.UsableOption UsableOption { get; private set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            ExecuteUseAction();
            if (Panel)
                Panel.ClosePanel();
        }

        #region PublicMethod

        public void UpdateOption(ItemUsableOptionsPanel panel, ItemUsableConfig.UsableOption option)
        {
            if (Panel != panel)
                Panel = panel;
            UsableOption = option;
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
    }
}