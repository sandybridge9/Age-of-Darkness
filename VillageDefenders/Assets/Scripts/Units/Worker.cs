using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class Worker : Unit
{
    private Transform currentResourceSelection;
    private Transform unloadingSite;
    private string currentResourceSelectionTypeName = "";
    private readonly float maxDistance = 3f;
    private float gatheringDelay = 60f;

    private ResourceBundle currentlyHeldResources;
    private ResourceBundle maximumResourceCapacity;

    public Worker()
    {
        Health = 50f;
        Cost = new ResourceBundle(0, 0, 0, 0, 20);
        CurrentUnitState = UnitState.Idle;
        currentlyHeldResources = new ResourceBundle(0, 0, 0, 0, 0);
        maximumResourceCapacity = new ResourceBundle(50, 50, 50, 50, 50);
    }

    protected override void SelectedUnitSpecificOrders()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GiveOrder();
        }
    }

    protected override void UnitSpecificOrders()
    {
        switch (CurrentUnitState)
        {
            case UnitState.Moving:
                CheckIfArrivedAtDestination();
                break;
            case UnitState.MovingToResource:
                CheckIfArrivedAtResource();
                break;
            case UnitState.Gathering:
                GatherResource();
                break;
            case UnitState.MovingToUnload:
                CheckIfArrivedAtUnloadingSite();
                break;
            case UnitState.Unloading:
                Unload();
                break;
            default:
                break;
        }
    }

    //Method that is responsible for processing mouse click worker commands (gather, unload, move).
    private void GiveOrder()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 1000f, SettingsManager.Instance.ResourceLayerMask))
        {
            Debug.Log("Got orders to move to resource. ");
            currentResourceSelection = hitInfo.transform;
            GetResourceTypeName();
            MoveToResource();
        }
        else if (Physics.Raycast(ray, out hitInfo, 1000f, SettingsManager.Instance.BuildingLayerMask))
        {
            currentResourceSelection = null;
            var transform = hitInfo.transform;
            Building hitBuilding = transform.GetComponent<Building>();
            switch (hitBuilding)
            {
                case Warehouse w:
                    unloadingSite = hitBuilding.transform.Find("UnloadingPoint");
                    MoveToUnloadingSite();
                    break;
                case Townhall th:
                    unloadingSite = hitBuilding.transform.Find("UnloadingPoint");
                    MoveToUnloadingSite();
                    break;
                default:
                    Move();
                    break;
            }
        }
        else
        {
            Debug.Log("Got orders to move to a location. ");
            Move();
            currentResourceSelection = null;
        }
    }

    //Get resource type name once after resource selection, to avoid calling type checker every Update
    private void GetResourceTypeName()
    {
        var resource = currentResourceSelection.GetComponent<Resource>();
        switch (resource)
        {
            case Stone s:
                currentResourceSelectionTypeName = "Stone";
                break;
            case Wood w:
                currentResourceSelectionTypeName = "Wood";
                break;
            case Food f:
                currentResourceSelectionTypeName = "Food";
                break;
            default:
                Debug.Log("The type doesn't exist");
                break;
        }
        Debug.Log(currentResourceSelectionTypeName);
    }

    private void MoveToResource()
    {
        if (currentResourceSelection != null)
        {
            agent.ResetPath();
            CurrentUnitState = UnitState.MovingToResource;
            agent.SetDestination(currentResourceSelection.position);
        }
        else
        {
            CurrentUnitState = UnitState.Idle;
        }
    }

    private void MoveToUnloadingSite()
    {
        agent.ResetPath();
        CurrentUnitState = UnitState.MovingToUnload;
        agent.SetDestination(unloadingSite.transform.position);
    }

    //Check if worker has arrived at harvesting location (location doesn't have to be exact, because one cannot stand on top of resources)
    private void CheckIfArrivedAtResource()
    {
        if (Vector3.Distance(currentResourceSelection.position, transform.position) < maxDistance)
        {
            Debug.Log("I have arrived, and I can now gather resources");
            agent.ResetPath();
            CurrentUnitState = UnitState.Gathering;
        }
    }

    //Check if worker has arrived at harvesting location (location doesn't have to be exact, because one cannot stand on top of resources)
    private void CheckIfArrivedAtDestination()
    {
        if (Vector3.Distance(agent.destination, transform.position) < 1)
        {
            Debug.Log("I have arrived at my destination and I am now idle.");
            agent.ResetPath();
            CurrentUnitState = UnitState.Idle;
        }
    }

    //Method that is called when worker's state is set to Gathering
    private void GatherResource()
    {
        if (gatheringDelay >= 60f)
        {
            if (currentlyHeldResources.HasReachedMaximumCapacity(maximumResourceCapacity))
            {
                GetNearestUnloadingSite();
                if (unloadingSite != null)
                {
                    MoveToUnloadingSite();
                    Debug.Log("Found an unloading site. Going to unload.");
                }
                else
                {
                    CurrentUnitState = UnitState.Idle;
                    Debug.Log("Unloading site was not found. Idling.");
                }
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

    //Method that is called when worker's state is set to MovingToUnload
    private void CheckIfArrivedAtUnloadingSite()
    {
        if (unloadingSite != null && Vector3.Distance(transform.position, unloadingSite.transform.position) < maxDistance)
        {
            CurrentUnitState = UnitState.Unloading;
        }
    }

    //Method that is called when worker's state is set to Unloading
    private void Unload()
    {
        SettingsManager.Instance.ResourceManager.AddToCurrentResources(currentlyHeldResources);
        currentlyHeldResources = new ResourceBundle();
        //Get back to the resource after unloading (only when gathering and unloading automatically)
        MoveToResource();
        Debug.Log("Unloaded. Have to get back to gathering resources.");
    }

    //Tries to get both townhall and warehouse, and if both are found, then sets nearest one as an unloadingBuilding,
    //if only one is found, then sets it as an unloading site
    private void GetNearestUnloadingSite()
    {
        var warehouse = SettingsManager.Instance.BuildingManager.GetWarehouse();
        var townhall = SettingsManager.Instance.BuildingManager.GetTownhall();
        if (warehouse != null && townhall != null)
        {
            var thUnloadingPoint = townhall.transform.Find("UnloadingPoint");
            var whUnloadingPoint = warehouse.transform.Find("UnloadingPoint");
            var distanceToTownhall = Vector3.Distance(transform.position, thUnloadingPoint.position);
            var distanceToWarehouse = Vector3.Distance(transform.position, whUnloadingPoint.position);
            if (distanceToTownhall < distanceToWarehouse)
            {
                unloadingSite = thUnloadingPoint;
            }
            else
            {
                unloadingSite = whUnloadingPoint;
            }
        }
        else if (warehouse != null)
        {
            unloadingSite = warehouse.transform.Find("UnloadingPoint"); ;
        }
        else if (townhall != null)
        {
            unloadingSite = townhall.transform.Find("UnloadingPoint");
        }
        else
        {
            unloadingSite = null;
            Debug.Log("No unloading sites have been found. ");
        }
    }
}
