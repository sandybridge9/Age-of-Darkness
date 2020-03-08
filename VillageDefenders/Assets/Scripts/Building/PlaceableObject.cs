using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that is used for collider detection when placing buildings or other structures
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    #region Properties

    [HideInInspector]
    //List used to keep track of all other colliders this building collides with
    public List<Collider> Colliders = new List<Collider>();
    
    #endregion

    #region Overriden Methods

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == "Building")
        {
            Colliders.Add(c);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Building")
        {
            Colliders.Remove(c);
        }
    }

    #endregion
}
