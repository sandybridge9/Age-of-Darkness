using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Townhall : Building
{
    public Townhall()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 0, 0);
        BuildingType = BuildingTypes.Townhall;
    }

    public Townhall(float health, ResourceBundle cost, BuildingTypes type)
    {
        Health = health;
        Cost = cost;
        BuildingType = type;
    }

    public void SpawnBuilder()
    {

    }

    public void SpawnWorker()
    {

    }
}
