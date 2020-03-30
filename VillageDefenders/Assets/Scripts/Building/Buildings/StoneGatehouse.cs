using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGatehouse : Building
{
    public StoneGatehouse()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 15, 0);
        BuildingType = BuildingTypes.StoneGatehouse;
    }

    public StoneGatehouse(float health, ResourceBundle cost)
    {
        Health = health;
        Cost = cost;
        BuildingType = BuildingTypes.StoneGatehouse;
    }
}
