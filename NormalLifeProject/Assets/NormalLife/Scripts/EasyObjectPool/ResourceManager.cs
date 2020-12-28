using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    [DisallowMultipleComponent]
    //[AddComponentMenu("")]
    public class ResourceManager : MonoBehaviour
    {
        //obj pool
        private Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();

        private static ResourceManager mInstance = null;

        public static bool IsDestroy = false;

        public static ResourceManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject go = new GameObject("ResourceManager", typeof(ResourceManager));
                    go.transform.localPosition = new Vector3(4000, 0, 0);
                    var canvasGroup = go.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = 0;
                    //canvasGroup.interactable = false;
                    //canvasGroup.blocksRaycasts = false;
                    // Kanglai: if we have `GO.hideFlags |= HideFlags.DontSave;`, we will encounter Destroy problem when exit playing
                    // However we should keep using this in Play mode only!
                    mInstance = go.GetComponent<ResourceManager>();

                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(mInstance.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("[ResourceManager] You'd better ignore ResourceManager in Editor mode");
                    }
                }
                return mInstance;
            }
        }
        public void InitPool(string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                GameObject pb = Resources.Load<GameObject>(poolName);
                if (pb == null)
                {
                    Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                    return;
                }
                poolDict[poolName] = new Pool(poolName, pb, gameObject, size, type);
            }
        }

        public void DelPool(string poolName)
        {
            if (!poolDict.ContainsKey(poolName))
            {
                return;
            }
            Pool pool = poolDict[poolName];
            pool.ReleaseAllObjects();
            poolDict.Remove(poolName);
        }
        /// <summary>
        /// 通过prefab初始化
        /// 扩展by qingqing-zhao
        /// </summary>
        /// <param name="poolPrefab"></param>
        /// <param name="size"></param>
        /// <param name="type"></param>
        public void InitPool(GameObject poolPrefab, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (poolPrefab == null) return;
            var poolName = poolPrefab.name;
            if (!poolDict.ContainsKey(poolName))
            {
                poolDict[poolName] = new Pool(poolName, poolPrefab, gameObject, size, type);
            }
        }
        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public GameObject GetObjectFromPool(string poolName, bool autoActive = true, int autoCreate = 1)
        {
            GameObject result = null;

            if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(poolName))
            {
                Pool pool = poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if DEBUG
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if DEBUG
            else
            {
                Debug.LogWarning("[ResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }
        /// <summary>
        /// Returns an available object from the pool 
        /// can use gameobject
        /// </summary>
        /// <param name="poolObj"></param>
        /// <param name="autoActive"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public GameObject GetObjectFromPool(GameObject poolObj, bool autoActive = true, int autoCreate = 1)
        {
            GameObject result = null;
            var poolName = poolObj.name;
            if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                InitPool(poolObj, autoCreate, PoolInflationType.INCREMENT);
            }
            if (poolDict.ContainsKey(poolName))
            {
                Pool pool = poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if DEBUG
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if DEBUG
            else
            {
                Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="go"></param>
        public void ReturnObjectToPool(GameObject go)
        {
            if(go == null)
            {
                return;
            }
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null || poolDict == null)
            {
                GameObject.Destroy(go);
#if DEBUG
                if(po == null)
                {
                    Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
                }
                if(poolDict == null)
                {
                    Debug.LogWarning("obj pool is null,please check." + go.name);
                }
#endif
            }
            else
            {
                Pool pool = null;
                if (poolDict.TryGetValue(po.poolName, out pool))
                {
                    pool.ReturnObjectToPool(po);
                }

                else
                {
                    GameObject.Destroy(go);
#if DEBUG
                    Debug.LogWarning("No pool available with name: " + po.poolName);
#endif
                }
            }
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="t"></param>
        public void ReturnTransformToPool(Transform t)
        {
            if (t == null)
            {
#if DEBUG
                Debug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
                return;
            }
            ReturnObjectToPool(t.gameObject);
        }

        public void ReleaseAllPool()
        {
            foreach (var pool in poolDict.Values)
            {
                pool.ReleaseAllObjects();
            }
            poolDict.Clear();
            poolDict = null;
            Destroy(mInstance.gameObject);
            mInstance = null;
            IsDestroy = true;
        }
        List<string> delList = new List<string>();
        //删除超过 fInterval时间不用的缓存 
        public void ReleasePoolSmart(float fInterval)
        {
            delList.Clear();
            if(poolDict != null)
            {
                foreach (var key in poolDict.Keys)
                {
                    Pool pool = poolDict[key];
                    pool.ReleaseObjectSmart(fInterval);
                    if (pool.bDel)
                    {
                        delList.Add(key);
                    }
                }
            }

            foreach (var key in delList)
            {
                DelPool(key);
            }
        }
    }
}