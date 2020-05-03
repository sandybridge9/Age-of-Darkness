using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenWall : Building
{
    #region Constructors

    public WoodenWall()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 2, 0, 0, 0);
        BuildingType = BuildingType.WoodenWall;
    }

    public WoodenWall(float health, ResourceBundle cost)
    {
        Health = health;
        Cost = cost;
        BuildingType = BuildingType.WoodenWall;
    }

    #endregion

}
