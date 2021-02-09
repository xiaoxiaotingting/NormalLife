using System;
using System.Collections;
using System.Collections.Generic;
using SG;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectPoolTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ResourceManager.Instance.InitPool("CubePool",10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Button]
    public void GenerateObj()
    {
        GameObject tObj = ResourceManager.Instance.GetObjectFromPool("CubePool");
        tObj.transform.position = Vector3.zero;
    }
    [Button]
    public void Generate5Obj()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject tObj = ResourceManager.Instance.GetObjectFromPool("CubePool");
            tObj.transform.position = Vector3.zero;
            StartCoroutine(ExecuteAfterTime(2.5f, () =>
            {            
                ResourceManager.Instance.ReturnObjectToPool(tObj);
                //Add somwthing here
            }));
        }
    }

    public void ReturnObj(GameObject tObj)
    {
        ResourceManager.Instance.ReturnObjectToPool(tObj);
    }
    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        // if (isCoroutineExecuting)
        //     yield break;
        //isCoroutineExecuting = true;
        yield return new WaitForSeconds(time);
        task();
        //isCoroutineExecuting = false;
    }
}
