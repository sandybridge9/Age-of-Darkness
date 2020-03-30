using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTower : Building
{
    public StoneTower()
    {
        Health = 100f;
        Cost = 25f;
        BuildingType = BuildingTypes.StoneTower;
    }
}
