using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace UnityEngine.UI
{
    public class Tab : Toggle
    {
        [SerializeField] private Button tabBarButton;
        [SerializeField] private UnityEvent onShow;
        [SerializeField] private UnityEvent onHide;
        [SerializeField] private float fadeSpeed = 10f;

        private CanvasGroup m_panelCanvasGroup;

        private Coroutine _fadeAlphaCoroutine;

        public CanvasGroup panelCanvasGroup
        {
            get
            {
                if (!m_panelCanvasGroup && !graphic.gameObject.TryGetComponent<CanvasGroup>(out m_panelCanvasGroup))
                    m_panelCanvasGroup = graphic.gameObject.AddComponent<CanvasGroup>();
                return m_panelCanvasGroup;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            // ReSharper disable once Unity.NoNullPropagation
            tabBarButton?.onClick.AddListener(SetTabOn);
            onValueChanged.AddListener(OnValueCHangedHandler);
            panelCanvasGroup.gameObject.SetActive(true);
            OnValueCHangedHandler(isOn);
        }

        private void SetTabOn() => isOn = true;

        private void OnValueCHangedHandler(bool value)
        {
            // if (graphic)
            //     graphic.gameObject.SetActive(value);

            //panelCanvasGroup.alpha = ;

            StartFade(Convert.ToInt32(value), fadeSpeed);
            panelCanvasGroup.blocksRaycasts = value;


            if (value)
            {
                var showColor = Color.white;
                if (targetGraphic)
                    targetGraphic.color = showColor;
                if (tabBarButton && targetGraphic)
                    tabBarButton.targetGraphic.color = showColor;

                onShow?.Invoke();
            }

            else
            {
                var hideColor = new Color(1, 1, 1, 0.5f);
                if (targetGraphic)
                    targetGraphic.color = hideColor;
                if (tabBarButton && tabBarButton.targetGraphic)
                    tabBarButton.targetGraphic.color = hideColor;


                onHide?.Invoke();
            }
        }

        private void StartFade(float target, float speed)
        {
            if (_fadeAlphaCoroutine != null)
                StopCoroutine(_fadeAlphaCoroutine);
            _fadeAlphaCoroutine = StartCoroutine(CanvasGroupFade(panelCanvasGroup, target, speed));
        }

        IEnumerator CanvasGroupFade(CanvasGroup canvasGroup, float targetAlpha, float speed = 1f)
        {
            if (!canvasGroup) yield break;
            while (Math.Abs(canvasGroup.alpha - targetAlpha) > 0.1f)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.smoothDeltaTime * speed);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }
    }
}