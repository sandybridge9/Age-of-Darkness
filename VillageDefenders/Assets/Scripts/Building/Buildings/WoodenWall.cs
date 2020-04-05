using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenWall : Building
{
    #region Constructors

    public WoodenWall()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 1, 0, 0, 0);
        BuildingType = BuildingTypes.WoodenWall;
    }

    public WoodenWall(float health, ResourceBundle cost)
    {
        Health = health;
        Cost = cost;
        BuildingType = BuildingTypes.WoodenWall;
    }

    #endregion

}
