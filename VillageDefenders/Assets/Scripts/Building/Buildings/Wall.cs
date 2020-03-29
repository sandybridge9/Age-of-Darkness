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
    }

    public Wall(float health, float cost, BuildingTypes type)
    {
        this.Health = health;
        this.Cost = cost;
        this.BuildingType = type;
    }

    #endregion

}
