using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region FIELDS

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
    
    private TextMeshProUGUI objective1Progress;
    private TextMeshProUGUI objective2Progress;
    private int objective1 = 10;
    private int progress1 = 0;
    private int objective2 = 1;
    private int progress2 = 0;

    private GameObject buildingMenu;
    private GameObject unitMenu;
    private GameObject settingsMenu;
    private GameObject tutorialMenu;
    private GameObject objectivesCompletedMenu;

    #endregion

    #region PROPERTIES

    public AudioMixer mixer;

    #endregion

    #region UNITY METHODS

    private void Start()
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

        objective1Progress = GameObject.Find("Objective1Progress").GetComponent<TextMeshProUGUI>();
        objective2Progress = GameObject.Find("Objective2Progress").GetComponent<TextMeshProUGUI>();

        buildingMenu = transform.Find("BuildingMenu").gameObject;
        unitMenu = transform.Find("UnitMenu").gameObject;
        settingsMenu = transform.Find("SettingsMenu").gameObject;
        tutorialMenu = transform.Find("TutorialMenu").gameObject;
        objectivesCompletedMenu = transform.Find("ObjectivesCompletedMenu").gameObject;

        settingsMenu.SetActive(false);
        unitMenu.SetActive(false);
        buildingMenu.SetActive(false);
        tutorialMenu.SetActive(true);
        objectivesCompletedMenu.SetActive(false);
    }

    private void Update()
    {
        UpdateResourceText();
        if (Input.GetKeyDown(KeyCode.T))
        {
            tutorialMenu.SetActive(!tutorialMenu.activeSelf);
        }
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

    #endregion

    #region BOTTOM RIGHT MENU CLICKS

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    public void OnSettingsButtonClick()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
        buildingMenu.SetActive(false);
        unitMenu.SetActive(false);
    }

    public void OnBuildingButtonClick()
    {
        buildingMenu.SetActive(!settingsMenu.activeSelf);
        settingsMenu.SetActive(false);
        unitMenu.SetActive(false);
    }

    public void OnUnitButtonClick()
    {
        unitMenu.SetActive(!settingsMenu.activeSelf);
        buildingMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void OnTutorialMenuCloseButtonClick()
    {
        tutorialMenu.SetActive(false);
    }

    #endregion

    #region BUILDING MENU BUTTON CLICKS

    public void OnTownhallButtonClick()
    {
        Townhall b = (Townhall)SettingsManager.Instance.PlaceableBuildings.Find(x => x.name == "Townhall");
        SettingsManager.Instance.BuildingManager.SetItem(b);
    }
    public void OnWarehouseButtonClick()
    {
        Warehouse b = (Warehouse)SettingsManager.Instance.PlaceableBuildings.Find(x => x.name == "Warehouse");
        SettingsManager.Instance.BuildingManager.SetItem(b);
    }
    public void OnBarracksButtonClick()
    {
        Barracks b = (Barracks)SettingsManager.Instance.PlaceableBuildings.Find(x => x.name == "Barracks");
        SettingsManager.Instance.BuildingManager.SetItem(b);
    }
    public void OnStoneGatehouseButtonClick()
    {
        StoneGatehouse b = (StoneGatehouse)SettingsManager.Instance.PlaceableBuildings.Find(x => x.name == "Stone Gatehouse");
        SettingsManager.Instance.BuildingManager.SetItem(b);
    }
    public void OnStoneTowerButtonClick()
    {
        StoneTower b = (StoneTower)SettingsManager.Instance.PlaceableBuildings.Find(x => x.name == "Stone Tower");
        SettingsManager.Instance.BuildingManager.SetItem(b);
    }
    public void OnStoneWallButtonClick()
    {
        StoneWall b = (StoneWall)SettingsManager.Instance.PlaceableBuildings.Find(x => x.name == "Stone Wall");
        SettingsManager.Instance.BuildingManager.SetItem(b);
    }
    public void OnWoodenWallButtonClick()
    {
        WoodenWall b = (WoodenWall)SettingsManager.Instance.PlaceableBuildings.Find(x => x.name == "Wood Wall");
        SettingsManager.Instance.BuildingManager.SetItem(b);
    }
    public void OnWoodenTowerButtonClick()
    {
        WoodenTower b = (WoodenTower)SettingsManager.Instance.PlaceableBuildings.Find(x => x.name == "Wooden Tower");
        SettingsManager.Instance.BuildingManager.SetItem(b);
    }
    public void OnFarmBuildingButtonClick()
    {
        FarmBuilding b = (FarmBuilding)SettingsManager.Instance.PlaceableBuildings.Find(x => x.name == "Farm");
        SettingsManager.Instance.BuildingManager.SetItem(b);
    }

    #endregion

    #region UNIT MENU BUTTON CLICKS

    public void OnWorkerButtonClick()
    {
        Worker w = (Worker)SettingsManager.Instance.TownhallTrainableUnits.Find(x => x.name == "Worker");
        SettingsManager.Instance.UnitManager.SetUnit(w);
    }

    public void OnArmedPeasantButtonClick()
    {
        ArmedPeasant ap = (ArmedPeasant)SettingsManager.Instance.BarracksTrainableUnits.Find(x => x.name == "Armed Peasant");
        SettingsManager.Instance.UnitManager.SetUnit(ap);
    }

    public void OnWarriorButtonClick()
    {
        Warrior w = (Warrior)SettingsManager.Instance.BarracksTrainableUnits.Find(x => x.name == "Warrior");
        SettingsManager.Instance.UnitManager.SetUnit(w);
    }

    #endregion

    #region SETTINGS MENU BUTTON CLICKS

    public void Set720pResolution()
    {
        Screen.SetResolution(1280, 720, true);
    }

    public void Set900pResolution()
    {
        Screen.SetResolution(1600, 900, true);
    }

    public void Set1080pResolution()
    {
        Screen.SetResolution(1920, 1080, true);
    }

    public void Set1440pResolution()
    {
        Screen.SetResolution(2560, 1440, true);
    }

    public void SetVolume(float volume)
    {
        mixer.SetFloat("Volume", volume);
    }

    #endregion

    #region OBJECTIVE

    public void RenewObjectiveProgress(string objective, int progress)
    {
        Debug.Log("progressing");
        switch (objective)
        {
            case "SkeletonBoss":
                progress2 += progress;
                string newText2 = "{0}/{1}";
                newText2 = String.Format(newText2, progress2, objective2);
                objective2Progress.text = newText2;
                break;
            case "ForestBandit":
                progress1 += progress;
                string newText1 = "{0}/{1}";
                newText1 = String.Format(newText1, progress1, objective1);
                objective1Progress.text = newText1;
                break;
        }

        if (progress1 >= objective1 && progress2 >= objective2)
        {
            ShowObjectivesCompleted();
        }
    }

    private void ShowObjectivesCompleted()
    {
        settingsMenu.SetActive(false);
        unitMenu.SetActive(false);
        buildingMenu.SetActive(false);
        tutorialMenu.SetActive(false);
        objectivesCompletedMenu.SetActive(true);
    }

    public void OnObjectivesCompletedDoneButtonClick()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    #endregion
}
