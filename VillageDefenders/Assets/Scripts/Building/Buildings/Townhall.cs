using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Townhall : Building
{
    public ResourceBundle ResourceCapacity;

    public Townhall()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 0, 0, 0);
        ResourceCapacity = new ResourceBundle(1000, 750, 750 ,750, 750);
        BuildingType = BuildingType.Townhall;
    }

    public Townhall(float health, ResourceBundle cost, ResourceBundle resourceCapacity, BuildingType type)
    {
        Health = health;
        Cost = cost;
        ResourceCapacity = resourceCapacity;
        BuildingType = type;
    }

    public void SpawnBuilder()
    {

    }

    public void SpawnWorker()
    {

    }
}
