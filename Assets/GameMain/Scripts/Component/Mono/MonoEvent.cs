using UnityEngine;
using UnityEngine.Events;

public class MonoEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent onAwake;
    [SerializeField] private UnityEvent onStart;
    [SerializeField] private UnityEvent onEnable;
    [SerializeField] private UnityEvent onUpdate;
    [SerializeField] private UnityEvent onDisable;
    [SerializeField] private UnityEvent onDestory;
    private void Awake() => onAwake?.Invoke();
    private void Start() => onStart?.Invoke();
    private void Update() => onUpdate?.Invoke();
    private void OnEnable() => onEnable?.Invoke();
    private void OnDisable() => onDisable?.Invoke();
    private void OnDestroy() => onDestory?.Invoke();
}