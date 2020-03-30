using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    //Current resources
    private ResourceBundle currentResources;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentResources(new ResourceBundle(SettingsManager.Instance.StartingGold,
            SettingsManager.Instance.StartingWood,
            SettingsManager.Instance.StartingStone,
            SettingsManager.Instance.StartingIron));
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Gold: " + currentResources.Gold +" Wood: " +currentResources.Wood + " Iron: " + currentResources.Iron +" Stone: " +currentResources.Stone);
    }

    public void SetCurrentResources(ResourceBundle resources)
    {
        currentResources = resources;
    }

    public bool SubtractBuildingCostFromCurrentResources(ResourceBundle cost)
    {
        return currentResources.SubtractResources(cost);
    }

    public void ReturnPercentageOfBuildingCost(ResourceBundle cost, int percentage)
    {
        currentResources.ReturnResources(cost, percentage);
    }
}
