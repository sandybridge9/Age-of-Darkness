using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class ResourceManager : MonoBehaviour
{
    #region FIELDS

    private bool isTownhallBuilt = false;
    private bool isInitialSetupDone = false;

    #endregion

    #region PROPERTIES

    //Current resources
    [HideInInspector]
    public ResourceBundle CurrentResources;
    [HideInInspector]
    public ResourceBundle MaximumCapacity;

    #endregion

    #region METHODS

    // Start is called before the first frame update
    private void Awake()
    {
        SetStartingMaximumCapacity();
    }

    public void SetStartingResources(ResourceBundle resources)
    {
        //CurrentResources = resources;
        CurrentResources.AddResources(resources);
    }

    public bool SubtractBuildingCostFromCurrentResources(ResourceBundle cost)
    {
        return CurrentResources.SubtractResources(cost);
    }

    public bool SubtractUnitCostFromCurrentResources(ResourceBundle cost)
    {
        return CurrentResources.SubtractResources(cost);
    }

    public void ReturnPercentageOfBuildingCost(ResourceBundle cost, int percentage)
    {
        CurrentResources.ReturnResources(MaximumCapacity, cost, percentage);
    }

    public void SetStartingMaximumCapacity()
    {
        MaximumCapacity = new ResourceBundle();
        CurrentResources = new ResourceBundle();
    }

    public void AddMaximumCapacity(ResourceBundle capacity)
    {
        MaximumCapacity.AddResources(capacity);
    }

    public void AddToCurrentResources(ResourceBundle resourcesToAdd)
    {
        CurrentResources.AddResources(resourcesToAdd);
    }

    public void BuildTownhall(Townhall townhall)
    {
        if (!isTownhallBuilt)
        {
            isTownhallBuilt = true;
            AddMaximumCapacity(townhall.ResourceCapacity);
            if (!isInitialSetupDone)
            {
                SetStartingResources(new ResourceBundle(SettingsManager.Instance.StartingGold,
                    SettingsManager.Instance.StartingWood,
                    SettingsManager.Instance.StartingStone,
                    SettingsManager.Instance.StartingIron, 
                    SettingsManager.Instance.StartingFood));
                isInitialSetupDone = true;
            }
        }
    }

    public void BuildWarehouse(Warehouse warehouse)
    {
        AddMaximumCapacity(warehouse.ResourceCapacity);
    }

    public double GetCurrentResourceCount(string resource)
    {
        double currentResourceCount = 0;
        switch (resource)
        {
            case "Food":
                currentResourceCount = CurrentResources.Food;
                break;
            case "Gold":
                currentResourceCount = CurrentResources.Gold;
                break;
            case "Stone":
                currentResourceCount = CurrentResources.Stone;
                break;
            case "Iron":
                currentResourceCount = CurrentResources.Iron;
                break;
            case "Wood":
                currentResourceCount = CurrentResources.Wood;
                break;
            default:
                Debug.Log("Resource Type was not found. ");
                break;
        }
        return currentResourceCount;
    }

    public double GetMaximumResourceCount(string resource)
    {
        double maximumResourceCount = 0;
        switch (resource)
        {
            case "Food":
                maximumResourceCount = MaximumCapacity.Food;
                break;
            case "Gold":
                maximumResourceCount = MaximumCapacity.Gold;
                break;
            case "Stone":
                maximumResourceCount = MaximumCapacity.Stone;
                break;
            case "Iron":
                maximumResourceCount = MaximumCapacity.Iron;
                break;
            case "Wood":
                maximumResourceCount = MaximumCapacity.Wood;
                break;
            default:
                Debug.Log("Resource Type was not found. ");
                break;
        }
        return maximumResourceCount;
    }

    #endregion
}
