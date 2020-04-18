using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Text resourceText;
    //private ResourceManager resourceManager;
    private ResourceBundle currentResources;
    private ResourceBundle maximumResources;

    // Start is called before the first frame update
    void Start()
    {
        resourceText = SettingsManager.Instance.MainUICanvas.transform.Find("ResourceText").GetComponent<Text>();
        //resourceManager = SettingsManager.Instance.ResourceManager;
        currentResources = SettingsManager.Instance.ResourceManager.CurrentResources;
        maximumResources = SettingsManager.Instance.ResourceManager.MaximumCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateResourceText();
    }

    private void UpdateResourceText()
    {
        string updatedText = "\nGold: {0}/{1} \nFood: {2}/{3} \nStone: {4}/{5} \nIron: {6}/{7} \nWood: {8}/{9}";
        updatedText = String.Format(updatedText,
            currentResources.Food, maximumResources.Food,
            currentResources.Gold, maximumResources.Gold,
            currentResources.Stone, maximumResources.Stone,
            currentResources.Iron, maximumResources.Iron,
            currentResources.Wood, maximumResources.Wood);
        Debug.Log(updatedText);
        resourceText.text = updatedText;
    }

    private void ShowBarrackUnitTraining()
    {
        //TODO when barracks are selected, show available troops to train
    }

    private void ShowTownhallUnitTraining()
    {
        //TODO when townhall are selected, show available troops to train
    }

    private void ShowBuildingList()
    {
        //TODO When no building or unit is selected, show building list
    }
}
