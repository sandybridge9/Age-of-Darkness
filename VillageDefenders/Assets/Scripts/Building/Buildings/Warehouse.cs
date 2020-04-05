using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : Building
{
    public ResourceBundle ResourceCapacity;

    public Warehouse()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 0, 0, 0);
        ResourceCapacity = new ResourceBundle(2500, 1500, 1500, 1500, 1500);
        BuildingType = BuildingTypes.Warehouse;
    }

    public Warehouse(float health, ResourceBundle cost, ResourceBundle resourceCapacity, BuildingTypes type)
    {
        Health = health;
        Cost = cost;
        ResourceCapacity = resourceCapacity;
        BuildingType = type;
    }
}
