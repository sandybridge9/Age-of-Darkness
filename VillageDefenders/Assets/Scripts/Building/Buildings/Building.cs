using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Internal;

public class Building : MonoBehaviour, IBuilding
{
    #region Fields

    private bool selected;

    #endregion

    #region Properties

    public float Health;
    public float Cost;
    public BuildingTypes BuildingType;

    [HideInInspector]
    public bool IsPlaced { get; set; } = false;

    #endregion

    #region Constructors

    public Building()
    {
        Health = 100f;
        Cost = 0f;
    }
    public Building(BuildingTypes buildingType) : this()
    {
        BuildingType = buildingType;
    }
    public Building(float health, float cost, BuildingTypes buildingType)
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

    void Update()
    {
        if (selected)
        {
            if (Input.GetKey(KeyCode.Delete))
            {
                Destroy();
            }
        }
    }

    #endregion
}
