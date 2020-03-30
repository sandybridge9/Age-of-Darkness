using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardTower : Building
{
    #region Properties

    public float Damage;
    public float FireRate;

    #endregion

    #region Constructors

    public WizardTower()
    {
        Health = 100f;
        Cost = new ResourceBundle(500, 50, 50, 50);
        Damage = 25f;
        FireRate = 3f;
        BuildingType = BuildingTypes.WizardTower;
    }

    public WizardTower(float health, ResourceBundle cost, float damage, float fireRate)
    {
        Health = health;
        Cost = cost;
        Damage = damage;
        FireRate = fireRate;
        BuildingType = BuildingTypes.WizardTower;
    }

    #endregion
}
