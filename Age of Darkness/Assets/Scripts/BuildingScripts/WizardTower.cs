using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardTower : Building
{
    public float damage;
    public float fireRate;

    public WizardTower()
    {
        health = 100f;
        cost = 10f;
        damage = 25f;
        fireRate = 3f;
    }

    public WizardTower(float health, float cost, float damage, float fireRate)
    {
        this.health = health;
        this.cost = cost;
        this.damage = damage;
        this.fireRate = fireRate;
    }
}
