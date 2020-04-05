using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = System.Random;

public class Worker : Unit
{
    private Transform currentResourceSelection;
    private string currentResourceSelectionTypeName = "";
    private readonly float maxGatheringDistance = 3f;
    private float gatheringDelay = 60f;
    private bool shouldUnload = false;

    private ResourceBundle currentlyHeldResources;
    private ResourceBundle maximumResourceCapacity;

    public Worker()
    {
        Health = 50f;
        Cost = new ResourceBundle(0, 0, 0, 0, 20);
        currentlyHeldResources = new ResourceBundle(0, 0, 0, 0, 0);
        maximumResourceCapacity = new ResourceBundle(50, 50, 50, 50, 50);
    }

    protected override void UnitSpecificOrders()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MoveWorker();
        }

        if (currentResourceSelection != null)
        {
            if (HasArrived())
            {
                GatherResource();
            }
            else
            {
                Debug.Log("Not yet arrived.");
            }
        }

        //TODO STOP gathering of resources and don't let gathering of new resources.
        if (shouldUnload)
        {
            Unload();
        }

        Debug.Log(currentlyHeldResources.ToString());
    }

    private void MoveWorker()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        //If raycast hits a resource, 
        if (Physics.Raycast(ray, out hitInfo, 1000f, SettingsManager.Instance.ResourceLayerMask))
        {
            Debug.Log("Moving to resource. Im on it chief.");
            currentResourceSelection = hitInfo.transform;
            GetResourceTypeName();
            Move();
        }
        else
        {
            Debug.Log("Just moving around.");
            Move();
            currentResourceSelection = null;
        }
    }

    private void GetResourceTypeName()
    {
        var resource = currentResourceSelection.GetComponent<Resource>();
        Debug.Log(resource);
        if (resource is Stone)
        {
            currentResourceSelectionTypeName = "Stone";
        }
        else if (resource is Wood)
        {
            currentResourceSelectionTypeName = "Wood";

        }
        else if(resource is Food)
        {
            currentResourceSelectionTypeName = "Food";
        }
        else
        {
            Debug.Log("The type doesn't exist");
            return;
        }
        Debug.Log(currentResourceSelectionTypeName);
    }

    //Check if worker has arrived at harvesting location (location doesn't have to be exact, because one cannot stand on top of resources)
    private bool HasArrived()
    {
        if (Vector3.Distance(currentResourceSelection.position, transform.position) < maxGatheringDistance)
        {
            Debug.Log("I have arrived, I can now gather resources");
            agent.ResetPath();
            return true;
        }
        return false;
        //return Vector3.Distance(lastGatheredResource.position, transform.position) < 3;
    }

    private void GatherResource()
    {
        if (gatheringDelay >= 60f)
        {
            if (currentlyHeldResources.HasReachedMaximumCapacity(maximumResourceCapacity))
            {
                shouldUnload = true;
            }
            else
            {
                Random rand = new Random();
                switch (currentResourceSelectionTypeName)
                {
                    case "Stone":
                        currentlyHeldResources.AddResources(new ResourceBundle(0, 0, rand.Next(1, 10), rand.Next(1, 5), 0));
                        break;
                    case "Wood":
                        currentlyHeldResources.AddResources(new ResourceBundle(0, rand.Next(5, 20), 0, 0, 0));
                        break;
                    case "Food":
                        currentlyHeldResources.AddResources(new ResourceBundle(0, 0, 0, 0, rand.Next(5, 15)));
                        break;
                    default:
                        break;
                }
                Debug.Log("Gathered some" +currentResourceSelectionTypeName);
                gatheringDelay = 0;
            }
        }
        gatheringDelay++;
    }

    //TODO Move to warehouse and dump all currently held resources
    private void Unload()
    {
        if (shouldUnload)
        {
            //Vector3 unloadingPoint = GetUnloadingPoint();
            Debug.Log("Should unload now.");
            //TODO Make actual unloading at the warehouse or townhall
            shouldUnload = false;
            currentlyHeldResources = new ResourceBundle();
        }
    }

    //private Vector3 GetUnloadingPoint()
    //{
    //    return new Vector3(0,0,0);
    //}
}
