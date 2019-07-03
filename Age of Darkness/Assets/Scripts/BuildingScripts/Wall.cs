using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Building
{
    public Wall()
    {
        health = 100f;
        cost = 10f;
    }

    public Wall(float health, float cost)
    {
        this.health = health;
        this.cost = cost;
    }
}
