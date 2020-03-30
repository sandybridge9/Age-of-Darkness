using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenTower : Building
{
    public WoodenTower()
    {
        Health = 100f;
        Cost = new ResourceBundle(5, 15, 5, 5);
        BuildingType = BuildingTypes.WoodenTower;
    }

    public WoodenTower(float health, ResourceBundle cost, BuildingTypes type)
    {
        Health = health;
        Cost = cost;
        BuildingType = type;
    }
}
