using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : Building
{
    #region Constructors

    public StoneWall()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 1, 0);
        BuildingType = BuildingTypes.StoneWall;
    }

    public StoneWall(float health, ResourceBundle cost)
    {
        Health = health;
        Cost = cost;
        BuildingType = BuildingTypes.StoneWall;
    }

    #endregion
}
