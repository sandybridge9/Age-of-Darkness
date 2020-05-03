using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    private Transform unitSpawnPoint;

    public Barracks()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 70, 25, 0, 0);
        BuildingType = global::BuildingType.Barracks;
    }

    public Barracks(float health, ResourceBundle cost, BuildingType type)
    {
        Health = health;
        Cost = cost;
        BuildingType = type;
    }
}
