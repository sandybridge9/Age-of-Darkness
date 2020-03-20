using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager instance = null;
    public static SettingsManager Instance
    {
        get
        {
            //if (instance == null)
            //{
            //    instance = new SettingsManager();
            //}
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    public static BuildingManager BuildingManager;

    public LayerMask GroundLayerMask;
    public List<Building> PlaceableBuildings;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        BuildingManager = GetComponent<BuildingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
