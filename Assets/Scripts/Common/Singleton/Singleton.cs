class Singleton<T> where T : class, new()
{
    private static T _instance;
    private static readonly object syslock = new object();

    public static T Instance
    {
        get
        {
            if (null == _instance)
            {
                lock (syslock)
                {
                    if (null == _instance)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}