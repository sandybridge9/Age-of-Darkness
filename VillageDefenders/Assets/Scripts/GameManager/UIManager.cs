using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //private Text resourceText;
    //private ResourceManager resourceManager;
    private ResourceBundle currentResources;
    private ResourceBundle maximumResources;

    private TextMeshProUGUI gold;
    private TextMeshProUGUI food;
    private TextMeshProUGUI wood;
    private TextMeshProUGUI stone;
    private TextMeshProUGUI iron;
    private TextMeshProUGUI goldMax;
    private TextMeshProUGUI foodMax;
    private TextMeshProUGUI woodMax;
    private TextMeshProUGUI stoneMax;
    private TextMeshProUGUI ironMax;

    // Start is called before the first frame update
    void Start()
    {
        gold = GameObject.Find("GoldText").GetComponent<TextMeshProUGUI>();
        food = GameObject.Find("FoodText").GetComponent<TextMeshProUGUI>();
        wood = GameObject.Find("WoodText").GetComponent<TextMeshProUGUI>();
        stone = GameObject.Find("StoneText").GetComponent<TextMeshProUGUI>();
        iron = GameObject.Find("IronText").GetComponent<TextMeshProUGUI>();
        goldMax = GameObject.Find("GoldMaxAmountText").GetComponent<TextMeshProUGUI>();
        foodMax = GameObject.Find("FoodMaxAmountText").GetComponent<TextMeshProUGUI>();
        woodMax = GameObject.Find("WoodMaxAmountText").GetComponent<TextMeshProUGUI>();
        stoneMax = GameObject.Find("StoneMaxAmountText").GetComponent<TextMeshProUGUI>();
        ironMax = GameObject.Find("IronMaxAmountText").GetComponent<TextMeshProUGUI>();
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
        string currentResourceText = "{0}";
        string maximumResourceText = "/{0}";
        gold.text = string.Format(currentResourceText, currentResources.Gold);
        food.text = string.Format(currentResourceText, currentResources.Food);
        wood.text = string.Format(currentResourceText, currentResources.Wood);
        stone.text = string.Format(currentResourceText, currentResources.Stone);
        iron.text = string.Format(currentResourceText, currentResources.Iron);

        goldMax.text = string.Format(maximumResourceText, maximumResources.Gold);
        foodMax.text = string.Format(maximumResourceText, maximumResources.Food);
        woodMax.text = string.Format(maximumResourceText, maximumResources.Wood);
        stoneMax.text = string.Format(maximumResourceText, maximumResources.Stone);
        ironMax.text = string.Format(maximumResourceText, maximumResources.Iron);
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
