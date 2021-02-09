using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T mInstance = null;

    public static T Instance()
    {
        if (mInstance == null)
        {
            mInstance = FindObjectOfType<T>();

            if (FindObjectsOfType<T>().Length > 1)
            {
                Debug.LogError("More than 1!");
                return mInstance;
            }

            if (mInstance == null)
            {
                string instanceName = typeof(T).Name;
                Debug.Log ("Instance Name: " + instanceName); 
                GameObject instanceGO = GameObject.Find(instanceName);

                if (instanceGO == null)
                    instanceGO = new GameObject(instanceName);
                        
                mInstance = instanceGO.AddComponent<T>();
                DontDestroyOnLoad(instanceGO);  //保证实例不会被释放
                Debug.Log ("Add New Singleton " + mInstance.name + " in Game!");
            }
            else
            {
                Debug.Log("Already exist: " + mInstance.name);
            }
        }

        return mInstance;
    }


    protected virtual void OnDestroy()
    {
        mInstance = null;
    }
}