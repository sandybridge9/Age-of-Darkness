using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : Building
{
    public Warehouse()
    {
        Health = 100f;
        Cost = 10f;
        BuildingType = BuildingTypes.Warehouse;
    }
}
