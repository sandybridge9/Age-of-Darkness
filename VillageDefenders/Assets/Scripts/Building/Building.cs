using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IBuilding
{
    #region Properties

    public float Health;
    public float Cost;

    #endregion

    #region Constructors

    public Building()
    {
        Health = 100f;
        Cost = 0f;
    }
    public Building(float health, float cost)
    {
        Health = health;
        Cost = cost;
    }

    #endregion
}
