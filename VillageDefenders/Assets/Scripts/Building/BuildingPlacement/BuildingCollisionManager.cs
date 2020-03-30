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
    public List<Collider> CurrentColliders = new List<Collider>();
    public List<BuildingTypes> ExceptionList = new List<BuildingTypes>();

    #endregion

    #region Overriden Methods

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Building")
        {
            var building = other.transform.GetComponent<Building>();
            if (!ExceptionList.Contains(building.BuildingType))
            {
                CurrentColliders.Add(other);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Building")
        {
            CurrentColliders.Remove(other);
        }
    }

    public bool IsColliding()
    {
        return CurrentColliders.Any();
    }
    
    public void ResetCollision()
    {
        CurrentColliders = new List<Collider>();
    }

    public void AddException(BuildingTypes type)
    {
        ExceptionList.Add(type);
    }

    #endregion
}
