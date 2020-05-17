using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTower : Building
{
    #region CONSTRUCTORS

    public StoneTower()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 40, 0, 0);
        BuildingType = BuildingType.StoneTower;
    }

    public StoneTower(float health, ResourceBundle cost, BuildingType type)
    {
        Health = health;
        Cost = cost;
        BuildingType = type;
    }

    #endregion
}
