using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    //Current resources
    public double Stone;
    public double Wood;
    public double Gold;
    public double Food;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Build(Building building)
    {
        if (building.Cost < Gold)
        {
            Debug.Log("Enough gold to purchase the building");
            Gold -= building.Cost;
            return true;
        }
        else
        {
            Debug.Log("Not enough gold");
            return false;
        }
    }
}
