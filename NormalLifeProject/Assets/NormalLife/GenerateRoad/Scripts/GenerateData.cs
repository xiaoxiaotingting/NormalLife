using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GenerateData",menuName = "Generate")]
public class GenerateData : ScriptableObject
{
    public List<bunchData> GeneList;
    
}
[System.Serializable]
//用于生成同一状态的块件，减少重复添加List
public class bunchData
{
    //道路类型
    public ChunkStatus Status;
    //该类道路数量
    public int bunchNum=1;
}