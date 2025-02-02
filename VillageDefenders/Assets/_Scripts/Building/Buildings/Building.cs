﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Internal;

public class Building : MonoBehaviour, IBuilding
{
    #region PROPERTIES

    public float Health;
    public ResourceBundle Cost;
    public BuildingType BuildingType;

    public bool IsPlaced = false;
    public bool IsSelected = false;
    public bool IsEnemy = false;

    [HideInInspector]
    public Collider Collider;

    #endregion

    #region CONSTRUCTORS

    public Building()
    {
        Health = 100f;
        Cost = new ResourceBundle(10,10,10,10, 0);
    }

    public Building(BuildingType buildingType) : this()
    {
        BuildingType = buildingType;
    }

    public Building(float health, ResourceBundle cost, BuildingType buildingType)
    {
        Health = health;
        Cost = cost;
        BuildingType = buildingType;
    }

    #endregion

    #region METHODS

    private void Update()
    {
        if (IsSelected)
        {
            if (Input.GetKey(KeyCode.Delete))
            {
                Delete();
            }
        }
    }

    private void Start()
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
        OnSelectActions();
    }

    protected virtual void OnSelectActions()
    {

    }

    public void DeSelect()
    {
        IsSelected = false;
        OnDeSelectActions();
    }

    protected virtual void OnDeSelectActions()
    {

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
        SettingsManager.Instance.SelectionManager.RemoveBuildingFromSelection(this);
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
