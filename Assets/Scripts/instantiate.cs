using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instantiate : MonoBehaviour
{

    public GameObject Agent;
    public int numCars = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i < numCars; i++)
        {
            var newcar = Instantiate(Agent);
            newcar.name = "Car " + i;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
