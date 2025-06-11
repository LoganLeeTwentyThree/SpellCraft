using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = FindFirstObjectByType<T>();
        }
        else
        {
            Destroy(this);
        }
    }

    public static T GetInstance()
    {
        return instance;
    }

}
