using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;
using UnityStandardAssets;
using UnityStandardAssets.Characters.ThirdPerson;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Worker : Unit
{
    #region FIELDS

    private Transform currentResourceSelection;
    private Transform unloadingSite;
    private string currentResourceSelectionTypeName = "";
    private float gatheringDelay = 60f;
    private float normalMovementSpeed;

    private ResourceBundle currentlyHeldResources;
    private ResourceBundle maximumResourceCapacity;

    //Worker maximum gathering amounts
    private int GoldAmount = 3;
    private int WoodAmount = 10;
    private int StoneAmount = 7;
    private int IronAmount = 4;
    private int FoodAmount = 7;
    private bool isAnimationSet = false;

    #endregion

    #region PROPERTIES

    public GameObject pickaxe;
    public GameObject plow;
    public GameObject axe;

    #endregion

    #region CONSTRUCTORS

    public Worker()
    {
        Health = 50f;
        Cost = new ResourceBundle(0, 5, 0, 5, 30);
        CurrentUnitState = UnitState.Idle;
        currentlyHeldResources = new ResourceBundle(0, 0, 0, 0, 0);
        maximumResourceCapacity = new ResourceBundle(50, 50, 50, 50, 50);
    }

    #endregion

    #region OVERRIDEN METHODS

    protected override void UnitSpecificStartup()
    {
        base.UnitSpecificStartup();

        GoldAmount = SettingsManager.Instance.MaximumGoldGatheringAmount;
        WoodAmount = SettingsManager.Instance.MaximumWoodGatheringAmount;
        StoneAmount = SettingsManager.Instance.MaximumStoneGatheringAmount;
        IronAmount = SettingsManager.Instance.MaximumIronGatheringAmount;
        FoodAmount = SettingsManager.Instance.MaximumFoodGatheringAmount;
        normalMovementSpeed = agent.speed;

        pickaxe.SetActive(false);
        plow.SetActive(false);
        axe.SetActive(false);
    }

    #endregion

    #region ORDER MANAGEMENT

    protected override void SelectedUnitSpecificOrdersOnUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GiveOrder();
        }
    }

    protected override void UnitSpecificOrdersOnUpdate()
    {
        switch (CurrentUnitState)
        {
            case UnitState.Moving:
                Move();
                break;
            case UnitState.MovingToResource:
                MoveToResource();
                break;
            case UnitState.Gathering:
                GatherResource();
                break;
            case UnitState.MovingToUnload:
                MoveToUnloadingSite();
                break;
            case UnitState.Unloading:
                Unload();
                break;
            case UnitState.Idle:
                character.Move(Vector3.zero, false, false);
                break;
            case UnitState.Rotating:
                RotateToFaceResource();
                break;
        }
    }

    //Method that is responsible for processing mouse click worker commands (gather, unload, move).
    private void GiveOrder()
    {
        ResetGatheringAnimations();
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 1000f, SettingsManager.Instance.ResourceLayerMask))
        {
            Debug.Log("Got orders to move to resource. ");
            currentResourceSelection = hitInfo.transform;
            MoveToResourceOrder();
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
                    MoveToUnloadingSiteOrder();
                    break;
                case Townhall th:
                    unloadingSite = hitBuilding.transform.Find("UnloadingPoint");
                    MoveToUnloadingSiteOrder();
                    break;
                default:
                    MoveOrder();
                    break;
            }
        }
        else
        {
            Debug.Log("Got orders to move to a location. ");
            MoveOrder();
            currentResourceSelection = null;
        }
    }

    #endregion

    #region RESOURCE GATHERING

    private void MoveToResourceOrder()
    {
        if (currentResourceSelection != null)
        {
            GetResourceTypeName();
            agent.ResetPath();
            CurrentUnitState = UnitState.MovingToResource;
            agent.SetDestination(currentResourceSelection.position);
        }
        else
        {
            CurrentUnitState = UnitState.Idle;
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
            Debug.Log("Checking if arrived at resource");
            CheckIfArrivedAtResource();
            if (CurrentUnitState == UnitState.MovingToResource)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }
        else
        {
            CurrentUnitState = UnitState.Idle;
        }
    }

    //Check if worker has arrived at harvesting location (location doesn't have to be exact, because one cannot stand on top of resources)
    private void CheckIfArrivedAtResource()
    {
        if ((currentResourceSelection.position - transform.position).sqrMagnitude < 3f * 3f)//Vector3.Distance(currentResourceSelection.position, transform.position) < 1.5f)
        {
            Debug.Log("I have arrived, and I should now rotate to face the resources");
            agent.ResetPath();
            CurrentUnitState = UnitState.Rotating;
            character.Move(Vector3.zero, false, false);
        }
    }


    //Method that is called when worker's state is set to Gathering
    private void GatherResource()
    {
        character.Move(Vector3.zero, false, false);
        if (gatheringDelay >= 60f)
        {
            if (currentlyHeldResources.HasReachedMaximumCapacity(maximumResourceCapacity))
            {
                ResetGatheringAnimations();
                //When worker is full, set his speed to 0.5, because he is carrying resources
                agent.speed = normalMovementSpeed / 2;
                GetNearestUnloadingSite();
                if (unloadingSite != null)
                {
                    MoveToUnloadingSiteOrder();
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
                //TODO Play animation when gathering, 3 different animations are needed.
                Random rand = new Random();
                switch (currentResourceSelectionTypeName)
                {
                    case "Stone":
                        PlayAnimation("Mining");
                        currentlyHeldResources.AddResources(new ResourceBundle(0, 0, rand.Next(1, StoneAmount), rand.Next(1, IronAmount), 0));
                        break;
                    case "Wood":
                        PlayAnimation("Chopping");
                        currentlyHeldResources.AddResources(new ResourceBundle(0, rand.Next(1, WoodAmount), 0, 0, 0));
                        break;
                    case "Food":
                        PlayAnimation("Farming");
                        currentlyHeldResources.AddResources(new ResourceBundle(0, 0, 0, 0, rand.Next(1, FoodAmount)));
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

    private void ResetGatheringAnimations()
    {
        isAnimationSet = false;
        animator.SetBool("Chopping", false);
        animator.SetBool("Mining", false);
        animator.SetBool("Farming", false);
        pickaxe.SetActive(false);
        plow.SetActive(false);
        axe.SetActive(false);
    }

    private void PlayAnimation(string animationName)
    {
        if (!isAnimationSet)
        {
            animator.SetBool(animationName, true);
            switch (animationName)
            {
                case "Chopping":
                    axe.SetActive(true);
                    break;
                case "Mining":
                    pickaxe.SetActive(true);
                    break;
                case "Plow":
                    plow.SetActive(true);
                    break;
            }
        }
    }

    private void RotateToFaceResource()
    {
        if (currentResourceSelectionTypeName != "Food")
        {
            var direction = (currentResourceSelection.position - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(direction);
            if (Quaternion.Angle(lookRotation, transform.rotation) > 40f)
            {
                Debug.Log("Rotating");
                character.Move(direction, false, false);
            }
            else
            {
                CurrentUnitState = UnitState.Gathering;
                character.Move(Vector3.zero, false, false);
            }
        }
        else
        {
            CurrentUnitState = UnitState.Gathering;
            character.Move(Vector3.zero, false, false);
        }
    }

    #endregion

    #region UNLOADING

    private void MoveToUnloadingSiteOrder()
    {
        if (unloadingSite != null)
        {
            agent.ResetPath();
            CurrentUnitState = UnitState.MovingToUnload;
            agent.SetDestination(unloadingSite.transform.position);
        }
    }

    private void MoveToUnloadingSite()
    {
        if (unloadingSite != null)
        {
            CheckIfArrivedAtUnloadingSite();
            if (CurrentUnitState == UnitState.MovingToUnload)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }
    }

    //Method that is called when worker's state is set to MovingToUnload
    private void CheckIfArrivedAtUnloadingSite()
    {
        if (Vector3.Distance(transform.position, unloadingSite.transform.position) < 1.5f)
        {
            agent.ResetPath();
            character.Move(Vector3.zero, false, false);
            CurrentUnitState = UnitState.Unloading;
        }
    }

    private void Unload()
    {
        Debug.Log("Unload");
        SettingsManager.Instance.ResourceManager.AddToCurrentResources(currentlyHeldResources);
        currentlyHeldResources = new ResourceBundle();
        //Set speed back to normal
        agent.speed = normalMovementSpeed;

        //Get back to the resource after unloading (only when gathering and unloading automatically)
        MoveToResourceOrder();
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
            var distanceToTownhall = (transform.position - thUnloadingPoint.position).sqrMagnitude;
            var distanceToWarehouse = (transform.position - whUnloadingPoint.position).sqrMagnitude;
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

    #endregion
}
