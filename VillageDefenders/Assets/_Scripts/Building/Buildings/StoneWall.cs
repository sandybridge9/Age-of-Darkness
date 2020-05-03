using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : Building
{
    #region Constructors

    public StoneWall()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 2, 0, 0);
        BuildingType = BuildingType.StoneWall;
    }

    public StoneWall(float health, ResourceBundle cost)
    {
        Health = health;
        Cost = cost;
        BuildingType = BuildingType.StoneWall;
    }

    #endregion
}
