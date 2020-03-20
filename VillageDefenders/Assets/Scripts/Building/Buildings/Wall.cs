using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Building
{
    #region Constructors

    public Wall()
    {
        Health = 100f;
        Cost = 10f;
        BuildingType = BuildingTypes.WoodenWall;
    }

    public Wall(float health, float cost)
    {
        this.Health = health;
        this.Cost = cost;
        BuildingType = BuildingTypes.WoodenWall;
    }

    #endregion

}
