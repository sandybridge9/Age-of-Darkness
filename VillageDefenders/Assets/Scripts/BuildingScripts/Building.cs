using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float health;
    public float cost;

    public Building()
    {
        health = 100f;
        cost = 0f;
    }
    public Building(float health, float cost)
    {
        this.health = health;
        this.cost = cost;
    }
}
