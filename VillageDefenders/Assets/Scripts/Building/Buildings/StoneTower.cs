using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTower : Building
{
    public StoneTower()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 25, 0, 0);
        BuildingType = BuildingTypes.StoneTower;
    }

    public StoneTower(float health, ResourceBundle cost, BuildingTypes type)
    {
        Health = health;
        Cost = cost;
        BuildingType = type;
    }
}
