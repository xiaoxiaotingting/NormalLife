using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChunkStatus
{
    STRI,//直的
    LEFT,//左拐的
    RIGHT,//右拐的
    BACKSTRI,//往回，Z轴负方向
    NONE//无
}
public class GenerateManager : MonoBehaviour
{
    public GenerateData generateData;
    public static GenerateData staticGenerateData;
    public GameObject straightChunk, backStraight, leftChunk, rightChunk;
    //转弯时生产点的位置偏转
    public Vector3 turnOffset=new Vector3(1.25f , 0 ,0);
    //道路数量计数
    public int chunkCount;
    
    //当前的道路类型
    [SerializeField]
    private ChunkStatus cStatus=ChunkStatus.NONE;
    // //将要生成的道路类型
    // [SerializeField]
    // private ChunkStatus geneStatus=ChunkStatus.NONE;
    //根节点
    private GameObject chunkRoot;
    //初始点
    private static Vector3 originPoint;
    //当前要生成道路的位置
    private static Vector3 curGenePoint;
    //上一个道路的生成点
    private Vector3 lastGenePoint;
    //上一个生成的道路块
    private GameObject lastGeneObj;
    // Start is called before the first frame update
    void Start()
    {
        originPoint = Vector3.zero;
        curGenePoint = originPoint;
        GenerateRoadByData(generateData);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateNextChunk();
        }        
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.R))
        {
            DestroyImmediate(chunkRoot);
            //重置点
            RestPoint();
            
            //重新生成
            GenerateRoadByData(generateData);
        }
    }
    
    private  void GenerateRoadByData(GenerateData data)
    {
        cStatus = ChunkStatus.NONE;
        //预生成一个直道
        GenerateNextChunk();
        if (data != null)
        {
            foreach (var chunkBunch in data.GeneList)
            {
                if (chunkBunch.bunchNum <= 1)
                {
                    GenerateNextChunk(chunkBunch.Status);
                    chunkCount++;
                }
                else
                {
                    for (int i = 0; i < chunkBunch.bunchNum; i++)
                    {
                        GenerateNextChunk(chunkBunch.Status);
                        chunkCount++;
                    }
                }
                
            }
        }
        else
        {
            Debug.LogError("No Generate Data!");
        }
    }
    private  void GenerateNextChunk(ChunkStatus geneStatus = ChunkStatus.STRI)
    {
        if(geneStatus == ChunkStatus.NONE) return;
        if (chunkRoot == null)
        {
            chunkRoot = new GameObject();
            chunkRoot.name = "RoadRoot";
        }
        //预处理
        GameObject geneObj=null;
        //左转右转处理生成点
        if (geneStatus == ChunkStatus.LEFT || geneStatus == ChunkStatus.RIGHT)
            DealWithGenePoint(geneStatus == ChunkStatus.RIGHT);
        switch (cStatus)
        {
            case ChunkStatus.NONE:
                //如果第一个为空，先生成直道
                geneObj=Instantiate(straightChunk, curGenePoint, Quaternion.identity);
                geneObj.transform.position=Vector3.zero;

                cStatus = ChunkStatus.STRI;
                break;
            case ChunkStatus.STRI:
                switch (geneStatus)
                {
                    case ChunkStatus.STRI:
                        geneObj=Instantiate(straightChunk, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.STRI;
                        break;
                    case ChunkStatus.BACKSTRI:
                        return;
                    case ChunkStatus.LEFT:
                        geneObj=Instantiate(leftChunk, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.LEFT;
                        break;
                    case ChunkStatus.RIGHT:
                        geneObj=Instantiate(rightChunk, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.RIGHT;
                        break;
                }
                //geneObj=Instantiate(straightChunk, curGenePoint, Quaternion.identity);
                break;          
            case ChunkStatus.BACKSTRI:
                switch (geneStatus)
                {
                    case ChunkStatus.STRI:
                        geneObj=Instantiate(backStraight, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.BACKSTRI;
                        break;
                    case ChunkStatus.BACKSTRI:
                        return;
                        break;
                    case ChunkStatus.LEFT:
                        geneObj=Instantiate(rightChunk, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.RIGHT;
                        break;
                    case ChunkStatus.RIGHT:
                        geneObj=Instantiate(leftChunk, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.LEFT;
                        break;
                }
                //geneObj=Instantiate(straightChunk, curGenePoint, Quaternion.identity);
                break;
            case ChunkStatus.LEFT:
                switch (geneStatus)
                {
                    case ChunkStatus.STRI:
                        geneObj=Instantiate(leftChunk, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.LEFT;
                        break;
                    case ChunkStatus.BACKSTRI:
                        return;
                        break;
                    case ChunkStatus.LEFT:
                        geneObj=Instantiate(backStraight, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.BACKSTRI;
                        break;
                    case ChunkStatus.RIGHT:
                        geneObj=Instantiate(straightChunk, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.STRI;
                        break;
                }
                break;
            case ChunkStatus.RIGHT:
                switch (geneStatus)
                {
                    case ChunkStatus.STRI:
                        geneObj=Instantiate(rightChunk, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.RIGHT;
                        break;
                    case ChunkStatus.BACKSTRI:
                        return;
                        break;
                    case ChunkStatus.LEFT:
                        geneObj=Instantiate(straightChunk, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.STRI;
                        break;
                    case ChunkStatus.RIGHT:
                        geneObj=Instantiate(backStraight, curGenePoint, Quaternion.identity);
                        cStatus = ChunkStatus.BACKSTRI;
                        break;
                }
                break;
                
        }
        //SetParent
        geneObj.transform.parent = chunkRoot.transform;
        //记录上个生产点
        lastGenePoint = curGenePoint;
        //获取当前块件的脚本信息
        var curRoadChunk=geneObj.GetComponent<RoadChunk>();
        if (curRoadChunk != null)
        {
            curGenePoint = curRoadChunk.endPoint.position;
            SetTurnState(geneObj, geneStatus);
        }
        else
        {
            curGenePoint = Vector3.zero;
            print("Can't Get Next GenePos.");
        }

        lastGeneObj=geneObj;
    }

    private void SetTurnState(GameObject geneObj, ChunkStatus geneStatus)
    {
        var turnTrigger = geneObj.GetComponentInChildren<TurnStateTrigger>();
        if (turnTrigger != null)
        {
            switch (geneStatus)
            {
                case ChunkStatus.LEFT:
                    turnTrigger.turnStateSetting = -1;
                    break;
                case ChunkStatus.RIGHT:
                    turnTrigger.turnStateSetting = 1;
                    break;
                default:
                    turnTrigger.gameObject.SetActive(false);
                    break;;
            }
        }
    }

    //左转右转时处理衔接问题
    private void DealWithGenePoint(bool isRight)
    {
        //以当前的移动方向为标准进行调整，
        switch (cStatus)
        {
            case ChunkStatus.STRI :
                //向左移动 x轴减小 向右移动 x轴增大
                curGenePoint.x += isRight?(turnOffset.x):(-turnOffset.x);
                //z轴统一移动
                curGenePoint.z += isRight?(turnOffset.y):(turnOffset.y);
                break;
            case ChunkStatus.LEFT :
                print("adjust");
                //向左移动 z轴减小 向右移动 z轴增大
                curGenePoint.z += isRight?(turnOffset.x):(-turnOffset.x);
                //x轴统一移动
                curGenePoint.x += isRight?(-turnOffset.y):(-turnOffset.y);
                break;
            case ChunkStatus.RIGHT :
                //向左移动 z轴增大 向右移动 z轴减小
                curGenePoint.z += isRight?(-turnOffset.x):(+turnOffset.x);
                //x轴统一移动
                curGenePoint.x += isRight?(turnOffset.y):(turnOffset.y);
                break;
            case ChunkStatus.BACKSTRI :
                //向左移动 x轴增大 向右移动 x轴减小
                curGenePoint.x += isRight?(-turnOffset.x):(turnOffset.x);
                //z轴统一移动
                curGenePoint.z += isRight?(-turnOffset.y):(-turnOffset.y);
                break;
        }
    }

    private void RestPoint()
    {
        originPoint = Vector3.zero;
        lastGenePoint = originPoint;
        curGenePoint = lastGenePoint;
    }
    
}
