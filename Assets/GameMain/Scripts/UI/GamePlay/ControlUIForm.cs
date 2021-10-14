using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Runtime.Base.GameEntry;

public class ControlUIForm : UIFormLogic
{
    [SerializeField] private Button openPackageButton; //打开背包按钮

    private void Awake()
    {
        if (!GameEntry.Base) OnInit(this);
    }

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        openPackageButton.onClick.AddListener(OpenPackageUIForm);
    }

    #region PrivateMethod

    private void OpenPackageUIForm()
    {
        if (GameEntry.UI)
        {
            //GameEntry.UI.OpenUIForm()
        }
    }

    #endregion
}