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
    public LayerMask GroundLayerMask;
    public LayerMask BuildingLayerMask;
    public LayerMask UnitLayerMask;
    public List<Building> PlaceableBuildings;

    //Properties hidden in editor
    [HideInInspector]
    public BuildingManager BuildingManager;
    [HideInInspector]
    public SelectionManager SelectionManager;



    #endregion

    void Start()
    {
        BuildingManager = GetComponent<BuildingManager>();
        SelectionManager = GetComponent<SelectionManager>();
    }

    void Update()
    {
        
    }
}
