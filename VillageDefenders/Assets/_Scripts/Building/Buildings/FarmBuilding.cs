using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmBuilding : Building
{
    public FarmBuilding()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 20, 0, 0, 0);
        BuildingType = global::BuildingType.StoneGatehouse;
    }

    public FarmBuilding(float health, ResourceBundle cost)
    {
        Health = health;
        Cost = cost;
        BuildingType = global::BuildingType.StoneGatehouse;
    }
}
