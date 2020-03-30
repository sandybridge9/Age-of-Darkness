using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Internal;

public class Building : MonoBehaviour, IBuilding
{
    #region Fields

    private bool selected;

    #endregion

    #region Properties

    public float Health;
    public ResourceBundle Cost;
    public BuildingTypes BuildingType;


    [HideInInspector]
    public bool IsPlaced { get; set; } = false;
    [HideInInspector]
    public Collider Collider;

    #endregion

    #region Constructors

    public Building()
    {
        Health = 100f;
        Cost = new ResourceBundle(10,10,10,10);
    }

    public Building(BuildingTypes buildingType) : this()
    {
        BuildingType = buildingType;
    }

    public Building(float health, ResourceBundle cost, BuildingTypes buildingType)
    {
        Health = health;
        Cost = cost;
        BuildingType = buildingType;
    }

    #endregion

    #region Methods

    public void Select()
    {
        selected = true;
    }

    public void DeSelect()
    {
        selected = false;
    }

    public void Destroy()
    {
        Object.Destroy(this.gameObject);
    }

    private void Delete()
    {
        SettingsManager.Instance.BuildingManager.DeleteBuildingFromList(this);
        Destroy();
    }

    void Update()
    {
        if (selected)
        {
            if (Input.GetKey(KeyCode.Delete))
            {
                Delete();
            }
        }
    }

    void Start()
    {
        GetCollider();
        StartupActions();
    }

    //Method for derived classes to override if actions need to be made on Start()
    protected virtual void StartupActions()
    {

    }

    //Derived classes should override this if they are not using BoxCollider
    protected virtual void GetCollider()
    {
        Collider = GetComponent<BoxCollider>();
        if (Collider == null)
        {
            Collider = GetComponentInChildren<Collider>();
        }
        Debug.Log(Collider);
    }

    #endregion
}
