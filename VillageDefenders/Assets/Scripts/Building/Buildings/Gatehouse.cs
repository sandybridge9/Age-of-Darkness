using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatehouse : Building
{
    public Gatehouse()
    {
        Health = 100f;
        Cost = 10f;
        BuildingType = BuildingTypes.StoneGatehouse;
    }
}
