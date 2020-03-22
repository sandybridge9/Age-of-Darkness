using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    private Transform unitSpawnPoint;

    public Barracks()
    {
        Health = 100f;
        Cost = 10f;
        BuildingType = BuildingTypes.Barracks;
    }

    private void Start()
    {
        unitSpawnPoint = transform.Find("UnitSpawnPoint");
    }

    public void SpawnTroop()
    {

    }
}
