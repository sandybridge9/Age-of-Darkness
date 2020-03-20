using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    public Barracks()
    {
        Health = 100f;
        Cost = 10f;
        BuildingType = BuildingTypes.Barracks;
    }

    public void SpawnTroop()
    {

    }
}
