using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHelper : MonoSingleton<SceneHelper>
{
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ChangeRoadSample()
    {
        var road1 = GameObject.Find("RoadRoot");
        var road2 = GameObject.Find("MapContain");
        road2 = road2.transform.Find("RoadRoot").gameObject;

        road1.SetActive(!road1.activeSelf);
        road2.SetActive(!road2.activeSelf);
        
    }
    
}
