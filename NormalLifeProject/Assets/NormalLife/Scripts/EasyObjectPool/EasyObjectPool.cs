/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
// modified version by Kanglai Qian
using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class PoolObject : MonoBehaviour
    {
        public string poolName;
        //defines whether the object is waiting in pool or is in use
        public bool isPooled;
        public float fLastUsedTime = 0;
    }

    public enum PoolInflationType
    {
        /// When a dynamic pool inflates, add one to the pool.
        INCREMENT,
        /// When a dynamic pool inflates, double the size of the pool
        DOUBLE
    }

    class Pool
    {
        //private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();
        private List<PoolObject> availableObjList = new List<PoolObject>();
        //the root obj for unused obj
        private GameObject rootObj;
        private PoolInflationType inflationType;
        private string poolName;
        private int objectsInUse = 0;
        public bool bDel = false;

        public Pool(string poolName, GameObject poolObjectPrefab, GameObject rootPoolObj, int initialCount, PoolInflationType type)
        {
            if (poolObjectPrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ObjPoolManager] null pool object prefab !");
#endif
                return;
            }
            this.poolName = poolName;
            this.inflationType = type;
            this.rootObj = new GameObject(poolName + "Pool");
            this.rootObj.transform.SetParent(rootPoolObj.transform, false);

            // In case the origin one is Destroyed, we should keep at least one
            // 不持有原始GameObject的引用，据官方说，原始Asset会有一个内部Remap结构引用，当加载的资源太多时，这个结构可能会达到10MB
            GameObject go = GameObject.Instantiate(poolObjectPrefab);
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
                po = go.AddComponent<PoolObject>();
            }
            po.poolName = poolName;
            AddObjectToPool(po);

            //populate the pool
            populatePool(Mathf.Max(initialCount, 1));
        }

        //o(1)
        private void AddObjectToPool(PoolObject po)
        {
            if(po.gameObject == null)
            {
#if UNITY_EDITOR
                Debug.LogError(string.Format("return a null gameobject : {0} ", poolName));
#endif
                return;
            }
            if(rootObj == null)
            {
                return;
            }
            //add to pool
            //po.gameObject.SetActive(false);
            po.gameObject.name = poolName;
            availableObjList.Add(po);
            po.isPooled = true;
            po.fLastUsedTime = Time.time;
            //add to a root obj
            po.gameObject.transform.SetParent(rootObj.transform, false);
        }

        private void populatePool(int initialCount)
        {
            for (int index = 0; index < initialCount; index++)
            {
                PoolObject po = GameObject.Instantiate(availableObjList[0]);
                AddObjectToPool(po);
            }
        }

        //o(1)
        public GameObject NextAvailableObject(bool autoActive)
        {
            PoolObject po = null;
            if (availableObjList.Count > 1)
            {
                po = availableObjList[availableObjList.Count - 1];
                availableObjList.RemoveAt(availableObjList.Count - 1);
            }
            else
            {
                int increaseSize = 0;
                //increment size var, this is for info purpose only
                if (inflationType == PoolInflationType.INCREMENT)
                {
                    increaseSize = 1;
                }
                else if (inflationType == PoolInflationType.DOUBLE)
                {
                    increaseSize = availableObjList.Count + Mathf.Max(objectsInUse, 0);
                }
#if UNITY_EDITOR
                Debug.Log(string.Format("Growing pool {0}: {1} populated", poolName, increaseSize));
#endif
                if (increaseSize > 0)
                {
                    populatePool(increaseSize);
                    po = availableObjList[availableObjList.Count - 1];
                    availableObjList.RemoveAt(availableObjList.Count - 1);
                }
            }

            GameObject result = null;
            if (po != null)
            {
                objectsInUse++;
                po.isPooled = false;
                po.fLastUsedTime = float.MaxValue;
                result = po.gameObject;
                //if (autoActive)
                //{
                //    result.SetActive(true);
                //}
            }

            return result;
        }

        //o(1)
        public void ReturnObjectToPool(PoolObject po)
        {
            if (poolName.Equals(po.poolName))
            {
                /* we could have used availableObjStack.Contains(po) to check if this object is in pool.
                 * While that would have been more robust, it would have made this method O(n) 
                 */
                if (po.isPooled)
                {
#if UNITY_EDITOR
                    Debug.LogError(po.gameObject.name + " is already in pool. Why are you trying to return it again? Check usage.");
#endif
                }
                else
                {
                    objectsInUse--;
                    AddObjectToPool(po);
                }
            }
            else
            {
                Debug.LogError(string.Format("Trying to add object to incorrect pool {0} {1}", po.poolName, poolName));
            }
        }


        public void ReleaseAllObjects()
        {
            for (int i = 0; i < availableObjList.Count; i++)
            {
                PoolObject po = availableObjList[i];
                if(po && po.gameObject)
                {
                    Object.Destroy(po.gameObject);
                }
            }
            if(rootObj != null)
            {
                Object.Destroy(rootObj);
            }
            availableObjList.Clear();
            objectsInUse = 0;
        }


        public void ReleaseObjectSmart(float fInterval)
        {
            for (int i = availableObjList.Count - 1; i >= 1; i--)
            {
                PoolObject po = availableObjList[i];
                if (po != null && Time.time - po.fLastUsedTime >= fInterval && po.gameObject != null && po.isPooled)
                {
                    Object.Destroy(po.gameObject);
                    availableObjList.RemoveAt(i);
                }
            }
            if(availableObjList.Count == 1 && objectsInUse == 0)
            {
                bDel = true;
            }
        }
    }
}