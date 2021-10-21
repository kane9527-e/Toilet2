using System;
using System.Collections;
using GameMain.Scripts.Runtime.Message;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Runtime.Base.GameEntry;

namespace GameMain.Scripts.UI.GamePlay.MessageUIForm
{
    public class MessageUIForm : UIFormLogic
    {
        [SerializeField] private Text msgText;

        //[SerializeField] private RectTransform messagePanel;
        [SerializeField] private CanvasGroup messagePanelCanvasGroup;

        private Coroutine _fadeCanvasGroupCor;

        private void Awake()
        {
            OnInit(this);
        }

        private void OnDestroy()
        {
            OnRecycle();
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            HidePanel();
            MessageSystem.OnMessagePushed += MessagePushedHandler;
            MessageSystem.OnMessagePoped += MessagePopedHandler;
        }


        protected override void OnRecycle()
        {
            base.OnRecycle();
            MessageSystem.OnMessagePushed -= MessagePushedHandler;
            MessageSystem.OnMessagePoped -= MessagePopedHandler;
        }

        public virtual void MessagePushedHandler(object package)
        {
            if (package == null) return;
            if (!messagePanelCanvasGroup.gameObject.activeSelf)
            {
                ShowPanel();
                NextMessage();
            }
        }

        private void MessagePopedHandler(object package)
        {
            if (msgText)
                msgText.text = (string)package;
        }

        public void ShowPanel()
        {
            //ONPanelHide();
            messagePanelCanvasGroup.gameObject.SetActive(true);
            if (_fadeCanvasGroupCor != null)
                StopCoroutine(_fadeCanvasGroupCor);
            _fadeCanvasGroupCor = StartCoroutine(FadeCanvasGroup(messagePanelCanvasGroup, 1f, 5f, ONPanelShow));
        }

        public void NextMessage()
        {
            if (MessageSystem.MsgQueue.Count <= 0)
            {
                HidePanel();
                return;
            }

            MessageSystem.NextMessage();
        }

        public void HidePanel()
        {
            //ONPanelShow();
            if (_fadeCanvasGroupCor != null)
                StopCoroutine(_fadeCanvasGroupCor);
            _fadeCanvasGroupCor = StartCoroutine(FadeCanvasGroup(messagePanelCanvasGroup, 0f, 15f, ONPanelHide));
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup group, float targetValue, float speedMultiplier = 1f,
            Action callBack = null)
        {
            if (!group) yield break;
            targetValue = targetValue > 1 ? 1 : targetValue < 0 ? 0 : targetValue; //Clamp 0 to 1
            var waitTime = new WaitForEndOfFrame();
            while (Math.Abs(group.alpha - targetValue) > Time.deltaTime)
            {
                group.alpha = Mathf.Lerp(group.alpha, targetValue, Time.deltaTime * speedMultiplier);
                yield return waitTime;
            }

            group.alpha = targetValue;
            callBack?.Invoke();
        }

        #region PrivateMethod

        private void ONPanelShow()
        {
            messagePanelCanvasGroup.blocksRaycasts = true;
            messagePanelCanvasGroup.alpha = 1;
            messagePanelCanvasGroup.gameObject.SetActive(true);
        }

        private void ONPanelHide()
        {
            messagePanelCanvasGroup.blocksRaycasts = false;
            messagePanelCanvasGroup.alpha = 0;
            messagePanelCanvasGroup.gameObject.SetActive(false);
        }

        #endregion
    }
}