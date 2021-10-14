namespace GameMain.Scripts.Runtime.Base
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Locker = new object();

        public static T Instance
        {
            get
            {
                lock (Locker)
                {
                    if (_instance == null)
                        _instance = new T();
                    return _instance;
                }
            }
        }
    }
}