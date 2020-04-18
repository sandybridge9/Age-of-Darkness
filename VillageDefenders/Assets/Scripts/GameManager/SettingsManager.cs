using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region Singleton

    public static SettingsManager Instance { get; set; }

    void Awake()
    {
        Instance = this;
        SetupOnAwake();
    }

    #endregion

    #region Properties

    //Editor properties
    public Canvas MainUICanvas;
    public float BuildingHeightCheckerSensitivity = 0.25f;
    public LayerMask GroundLayerMask;
    public LayerMask ResourceLayerMask;
    public LayerMask BuildingLayerMask;
    public LayerMask UnitLayerMask;
    public List<Building> PlaceableBuildings;
    public List<Unit> TownhallTrainableUnits;
    public List<Unit> BarracksTrainableUnits;

    public Material MaterialCanBuild;
    public Material MaterialCantBuild;
    public double StartingGold = 500f;
    public double StartingWood = 100f;
    public double StartingStone = 100f;
    public double StartingIron = 100f;
    public double StartingFood = 150f;

    //Properties hidden in editor
    [HideInInspector]
    public BuildingManager BuildingManager;
    [HideInInspector]
    public SelectionManager SelectionManager;
    [HideInInspector]
    public ResourceManager ResourceManager;
    [HideInInspector]
    public UnitManager UnitManager;
    [HideInInspector] 
    public UIManager UIManager;
    
    #endregion

    private void SetupOnAwake()
    {
        BuildingManager = GetComponent<BuildingManager>();
        SelectionManager = GetComponent<SelectionManager>();
        ResourceManager = GetComponent<ResourceManager>();
        UnitManager = GetComponent<UnitManager>();
        UIManager = GetComponent<UIManager>();
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}
