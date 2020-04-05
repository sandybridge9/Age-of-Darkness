using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    private Transform unitSpawnPoint;

    public Barracks()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 50, 0, 0, 0);
        BuildingType = BuildingTypes.Barracks;
    }

    public Barracks(float health, ResourceBundle cost, BuildingTypes type)
    {
        Health = health;
        Cost = cost;
        BuildingType = type;
    }

    //Use virtual StartupActions()
    //private void Start()
    //{
    //    unitSpawnPoint = transform.Find("UnitSpawnPoint");
    //}

    public void SpawnTroop()
    {

    }
}
