using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    //Current resources
    private ResourceBundle currentResources;
    private ResourceBundle maximumCapacity;
    [HideInInspector]
    private bool isTownhallBuilt = false;
    private bool isInitialSetupDone = false;

    // Start is called before the first frame update
    void Start()
    {
        SetStartingMaximumCapacity();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Gold: " + currentResources.Gold +" Wood: " +currentResources.Wood + " Iron: " + currentResources.Iron +" Stone: " +currentResources.Stone);
        //Debug.Log("Gold: " + maximumCapacity.Gold + " Wood: " + maximumCapacity.Wood + " Iron: " + maximumCapacity.Iron + " Stone: " + maximumCapacity.Stone);
    }

    public void SetStartingResources(ResourceBundle resources)
    {
        currentResources = resources;
    }

    public bool SubtractBuildingCostFromCurrentResources(ResourceBundle cost)
    {
        return currentResources.SubtractResources(cost);
    }

    public void ReturnPercentageOfBuildingCost(ResourceBundle cost, int percentage)
    {
        currentResources.ReturnResources(maximumCapacity, cost, percentage);
    }

    public void SetStartingMaximumCapacity()
    {
        maximumCapacity = new ResourceBundle();
        currentResources = new ResourceBundle();
    }

    public void AddMaximumCapacity(ResourceBundle capacity)
    {
        maximumCapacity.AddResources(capacity);
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
}
