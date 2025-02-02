﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGatehouse : Building
{
    #region CONSTRUCTORS

    public StoneGatehouse()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 35, 0, 0);
        BuildingType = global::BuildingType.StoneGatehouse;
    }

    public StoneGatehouse(float health, ResourceBundle cost)
    {
        Health = health;
        Cost = cost;
        BuildingType = global::BuildingType.StoneGatehouse;
    }

    #endregion
}
