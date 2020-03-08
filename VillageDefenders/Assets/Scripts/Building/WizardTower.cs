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
        Cost = 10f;
        Damage = 25f;
        FireRate = 3f;
    }

    public WizardTower(float health, float cost, float damage, float fireRate)
    {
        this.Health = health;
        this.Cost = cost;
        this.Damage = damage;
        this.FireRate = fireRate;
    }

    #endregion
}
