using System.Collections.Generic;
using Inventory.Runtime.Scripts.ScriptableObject;
using UnityEngine;

namespace GameMain.Scripts.UI.GamePlay.InventoryUI
{
    public class ItemUsableOptionsPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private GameObject usableOptionPrefab;
        private readonly List<ItemUsableOptionButton> _optionButtons = new List<ItemUsableOptionButton>();

        public InventoryUIForm Parent { get; private set; }

        public void Init(InventoryUIForm parentUIForm)
        {
            if (!Parent || Parent != parentUIForm)
                Parent = parentUIForm;
        }

        public void OpenUsableConfig(ItemUsableConfig config)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            var resultOptions =
                new List<ItemUsableConfig.UsableOption>(config.UsableOptions);
            resultOptions.RemoveAll(option => option.ConditionConfig.Result() == false);
            CreateOrUpdateOptions(config.UsableOptions);
        }

        private void CreateOrUpdateOptions(List<ItemUsableConfig.UsableOption> usableOptions)
        {
            for (var i = 0; i < usableOptions.Count; i++)
            {
                if (_optionButtons.Count < i + 1)
                    CreateUsableOptionButton();
                var optionButton = _optionButtons[i];

                if (optionButton)
                    optionButton.UpdateOption(this, usableOptions[i]);

                // optionButton.UsableOption?.Action?.RemoveListener(ClosePanel);
                // optionButton.UsableOption?.Action?.AddListener(ClosePanel);
            }
        }

        private void CreateUsableOptionButton()
        {
            if (!root || !usableOptionPrefab)
                return; //if there is not have UIPrefab or rootTransform just return exit function
            var buttonObj = Instantiate(usableOptionPrefab, root);
            // ReSharper disable once NotAccessedVariable
            if (!buttonObj.TryGetComponent(out ItemUsableOptionButton button))
                button = buttonObj.AddComponent<ItemUsableOptionButton>();
            if (!_optionButtons.Contains(button))
                _optionButtons.Add(button);
        }

        public void ClosePanel()
        {
            if (Parent)
                Parent.UnUseCurrentItem();
        }
    }
}