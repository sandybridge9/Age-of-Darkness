using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : Building
{
    public Warehouse()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 0, 0);
        BuildingType = BuildingTypes.Warehouse;
    }

    public Warehouse(float health, ResourceBundle cost, BuildingTypes type)
    {
        Health = health;
        Cost = cost;
        BuildingType = type;
    }
}
