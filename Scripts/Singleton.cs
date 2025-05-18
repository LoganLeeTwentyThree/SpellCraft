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

    protected void OnEnable()
    {
        
        Populate();
    }

    public static T GetInstance()
    {
        return instance;
    }

    public abstract void Populate();
}
