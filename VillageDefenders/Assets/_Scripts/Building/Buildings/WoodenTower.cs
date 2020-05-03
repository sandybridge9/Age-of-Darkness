using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenTower : Building
{
    public WoodenTower()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 50, 0, 0, 0);
        BuildingType = BuildingType.WoodenTower;
    }

    public WoodenTower(float health, ResourceBundle cost, BuildingType type)
    {
        Health = health;
        Cost = cost;
        BuildingType = type;
    }
}
