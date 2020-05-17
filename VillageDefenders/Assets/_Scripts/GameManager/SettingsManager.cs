using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region SINGLETON

    public static SettingsManager Instance { get; set; }

    #endregion

    #region PROPERTIES

    //Editor properties
    [Header("Layer masks for raycasting")]
    public LayerMask GroundLayerMask;
    public LayerMask ResourceLayerMask;
    public LayerMask BuildingLayerMask;
    public LayerMask UnitLayerMask;

    [Header("Buildings")]
    public List<Building> PlaceableBuildings;
    public float BuildingHeightCheckerSensitivity = 0.25f;
    public Material MaterialCanBuild;
    public Material MaterialCantBuild;

    [Header("Units")]
    public List<Unit> TownhallTrainableUnits;
    public List<Unit> BarracksTrainableUnits;

    [Header("Starting resources")]
    public double StartingGold = 500f;
    public double StartingWood = 100f;
    public double StartingStone = 100f;
    public double StartingIron = 100f;
    public double StartingFood = 150f;

    [Header("Worker resource maximum gathering amounts per action")]
    public int MaximumGoldGatheringAmount = 3;
    public int MaximumWoodGatheringAmount = 10;
    public int MaximumStoneGatheringAmount = 7;
    public int MaximumIronGatheringAmount = 4;
    public int MaximumFoodGatheringAmount = 7;

    //Managers
    [HideInInspector]
    public BuildingManager BuildingManager;
    [HideInInspector]
    public SelectionManager SelectionManager;
    [HideInInspector]
    public ResourceManager ResourceManager;
    [HideInInspector]
    public UnitManager UnitManager;

    public UIManager UIManager;

    #endregion

    #region UNITY METHODS

    void Awake()
    {
        Instance = this;
        SetupOnAwake();
    }

    #endregion

    #region METHODS

    private void SetupOnAwake()
    {
        BuildingManager = GetComponent<BuildingManager>();
        SelectionManager = GetComponent<SelectionManager>();
        ResourceManager = GetComponent<ResourceManager>();
        UnitManager = GetComponent<UnitManager>();
    }

    #endregion
}
