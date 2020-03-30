using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenTower : Building
{
    public WoodenTower()
    {
        Health = 100f;
        Cost = 15f;
        BuildingType = BuildingTypes.WoodenTower;
    }
}
