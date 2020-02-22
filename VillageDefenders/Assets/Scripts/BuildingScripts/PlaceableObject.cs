using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [HideInInspector]
    public List<Collider> colliders = new List<Collider>(); //List used to keep track of all other colliders this building collides with

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == "Building")
        {
            colliders.Add(c);
        }
    }
    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Building")
        {
            colliders.Remove(c);
        }
    }
}
