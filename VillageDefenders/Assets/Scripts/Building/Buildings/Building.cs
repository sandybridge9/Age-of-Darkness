using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Internal;

public class Building : MonoBehaviour, IBuilding
{
    #region Fields


    #endregion

    #region Properties

    public float Health;
    public ResourceBundle Cost;
    public BuildingTypes BuildingType;


    [HideInInspector]
    public bool IsPlaced { get; set; } = false;
    [HideInInspector]
    public bool IsSelected { get; set; } = false;

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

    void Update()
    {
        if (IsSelected)
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

    public void Select()
    {
        IsSelected = true;
    }

    public void DeSelect()
    {
        IsSelected = false;
    }

    //Destroys this gameObject
    public void Destroy()
    {
        Object.Destroy(this.gameObject);
    }

    //Same as Destroy() but first deletes Building from manager lists
    public void Delete()
    {
        SettingsManager.Instance.BuildingManager.DeleteBuildingFromList(this);
        SettingsManager.Instance.SelectionManager.RemoveGameObjectFromSelection(this.gameObject);
        Destroy();
    }

    //Derived classes should override this if they are not using BoxCollider
    protected virtual void GetCollider()
    {
        Collider = GetComponent<BoxCollider>();
        if (Collider == null)
        {
            Collider = GetComponentInChildren<Collider>();
        }
    }

    #endregion
}
