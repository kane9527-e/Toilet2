using UnityEngine;

namespace GameMain.Scripts.Runtime.Base
{
    public abstract class MonoSingleton <T>: MonoBehaviour where T:MonoBehaviour
    {
        private static T _instance = null;
 
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Locker = new object();
 
        // ReSharper disable once StaticMemberInGenericType
        private static bool _appQuitting;
 
        public static T Instance
        {
            get
            {
                if (_appQuitting)
                {
                    _instance = null;
                    return _instance;
                }
 
                lock (Locker)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (FindObjectsOfType<T>().Length > 1)
                        {
                            Debug.LogError("不应该存在多个单例！");
                            return _instance;
                        }
 
                        if (_instance == null)
                        {
                            var singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton)" + typeof(T);
                            singleton.hideFlags = HideFlags.None;
                            DontDestroyOnLoad(singleton);
                        }
                        else
                            DontDestroyOnLoad(_instance.gameObject);
                    }
                    _instance.hideFlags = HideFlags.None;
                    return _instance;
                }
            }
        }
 
        private void Awake()
        {
            _appQuitting = false;
        }
 
        private void OnDestroy()
        {
            _appQuitting = true;
        }
    }
}