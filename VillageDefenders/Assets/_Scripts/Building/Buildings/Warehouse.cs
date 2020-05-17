using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : Building
{
    #region PROPERTIES

    public ResourceBundle ResourceCapacity;

    #endregion

    #region CONSTRUCTORS

    public Warehouse()
    {
        Health = 100f;
        Cost = new ResourceBundle(5, 45, 20, 10, 20);
        ResourceCapacity = new ResourceBundle(2500, 1500, 1500, 1500, 1500);
        BuildingType = BuildingType.Warehouse;
    }

    public Warehouse(float health, ResourceBundle cost, ResourceBundle resourceCapacity, BuildingType type)
    {
        Health = health;
        Cost = cost;
        ResourceCapacity = resourceCapacity;
        BuildingType = type;
    }

    #endregion
}
