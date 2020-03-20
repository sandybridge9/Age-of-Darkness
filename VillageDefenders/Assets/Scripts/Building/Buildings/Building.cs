using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IBuilding
{
    #region Properties

    public float Health;
    public float Cost;
    public BuildingTypes BuildingType;

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

    public void Destroy()
    {
        Object.Destroy(this.gameObject);
    }

    #endregion
}
