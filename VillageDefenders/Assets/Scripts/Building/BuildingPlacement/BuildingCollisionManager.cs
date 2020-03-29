using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that is used for collider detection when placing buildings or other structures
/// </summary>
public class BuildingCollisionManager : MonoBehaviour
{
    #region Properties

    [HideInInspector]
    //List used to keep track of all other colliders this building collides with
    private List<Collider> Colliders = new List<Collider>();
    
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

    public bool IsColliding()
    {
        return Colliders.Any();
    }

    //public bool IsOverlapping()
    //{
    //    var collider = transform.GetComponent<Collider>();
    //    List<Collider> colliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, transform.rotation, SettingsManager.Instance.BuildingLayerMask).ToList();
    //    Debug.Log("Count: " + colliders.Count);
    //    foreach (var c in colliders)
    //    {
    //        Debug.Log(c.transform.position);
    //    }
    //    return colliders.Any();
    //}

    public void ResetCollision()
    {
        Colliders = new List<Collider>();
    }

    #endregion
}
