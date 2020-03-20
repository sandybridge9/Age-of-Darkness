using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Townhall : Building
{
    public Townhall()
    {
        Health = 100f;
        Cost = 10f;
        BuildingType = BuildingTypes.Townhall;
    }

    public void SpawnBuilder()
    {

    }

    public void SpawnWorker()
    {

    }
}
