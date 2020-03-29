using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTower : Building
{
    public BasicTower()
    {
        Health = 100f;
        Cost = 15f;
        BuildingType = BuildingTypes.BasicTower;
    }
}
