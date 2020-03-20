using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fastest : MonoBehaviour
{

    public string fastestName = "";
    public float fastestTime = 0;
    public float avgSpeed = 0;
    public Text timeTxt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeTxt.text = fastestName + ": " + fastestTime + "  Average Speed: " + avgSpeed;
    }
}
