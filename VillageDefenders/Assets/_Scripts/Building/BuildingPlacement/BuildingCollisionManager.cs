using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that is used for collider detection when placing buildings or other structures
/// </summary>
public class BuildingCollisionManager : MonoBehaviour
{
    #region PROPERTIES

    [HideInInspector]
    //List used to keep track of all other colliders this building collides with
    public List<Collider> CurrentColliders = new List<Collider>();
    //List used to keep track of building exceptions when collision checking
    public List<BuildingType> ExceptionList = new List<BuildingType>();

    #endregion

    #region UNITY METHODS

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Building")
        {
            var building = other.transform.GetComponent<Building>();
            if (!ExceptionList.Contains(building.BuildingType))
            {
                CurrentColliders.Add(other);
            }
        }
        else if (other.tag == "Resource" || other.tag == "Unit")
        {
            CurrentColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Building")
        {
            CurrentColliders.Remove(other);
        }
        else if (other.tag == "Resource" || other.tag == "Unit")
        {
            CurrentColliders.Remove(other);
        }
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Checks if this Building is currently colliding with another Building, Resource or Unit
    /// </summary>
    /// <returns></returns>
    public bool IsColliding()
    {
        return CurrentColliders.Any();
    }
    
    /// <summary>
    /// Resets CurrentColliders list
    /// </summary>
    public void ResetCollision()
    {
        CurrentColliders = new List<Collider>();
    }

    /// <summary>
    /// Method used to exclude certain buildings from collision checking
    /// </summary>
    /// <param name="type">Building type to exclude from collision checking</param>
    public void AddException(BuildingType type)
    {
        ExceptionList.Add(type);
    }

    #endregion
}
