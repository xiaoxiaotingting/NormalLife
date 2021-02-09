using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadChunk : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Transform model;
    // Start is called before the first frame update
    void Awake()
    {
        startPoint = transform.Find("start");
        endPoint = transform.Find("end");
        model = transform.Find("model");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
