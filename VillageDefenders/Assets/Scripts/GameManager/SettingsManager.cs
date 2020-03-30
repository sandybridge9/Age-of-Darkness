using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    #region Singleton

    public static SettingsManager Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Properties

    //Editor properties
    public float BuildingHeightCheckerSensitivity = 0.25f;
    public LayerMask GroundLayerMask;
    public LayerMask BuildingLayerMask;
    public LayerMask UnitLayerMask;
    public List<Building> PlaceableBuildings;

    public Material MaterialCanBuild;
    public Material MaterialCantBuild;
    public double StartingGold = 500f;
    public double StartingWood = 100f;
    public double StartingStone = 100f;
    public double StartingIron = 100f;

    //Properties hidden in editor
    [HideInInspector]
    public BuildingManager BuildingManager;
    [HideInInspector]
    public SelectionManager SelectionManager;
    [HideInInspector]
    public ResourceManager ResourceManager;
    
    #endregion

    void Start()
    {
        BuildingManager = GetComponent<BuildingManager>();
        SelectionManager = GetComponent<SelectionManager>();
        ResourceManager = GetComponent<ResourceManager>();
    }

    void Update()
    {
        
    }
}
