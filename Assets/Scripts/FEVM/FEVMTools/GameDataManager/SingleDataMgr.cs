public class SingleDataMgr<T> where T :  new()
{
    protected static T m_Instance;
    static object mLock = new object();
    public static T Instance
    {
        get
        {
            lock (mLock)
            {
                if (m_Instance == null)
                {
                    m_Instance = new T();
                }
            }
            return m_Instance;
        }
    }
}