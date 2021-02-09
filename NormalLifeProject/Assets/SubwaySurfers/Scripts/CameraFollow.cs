using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetTrans;
    private Vector3 offset;
    [SerializeField]
    private float followSpeed;
    private float y;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 followPos = targetTrans.position + offset;
        RaycastHit hit;
        if(Physics.Raycast(targetTrans.position,Vector3.down,out hit ,3f))
        {
            y = Mathf.Lerp(y, hit.point.y, Time.deltaTime * followSpeed);
        }
        else
        {
            y = Mathf.Lerp(y, targetTrans.position.y, Time.deltaTime * followSpeed);
        }

        followPos.y = y + offset.y; 
        transform.position = followPos;
    }
}
