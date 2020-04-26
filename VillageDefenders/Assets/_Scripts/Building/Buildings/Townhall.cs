using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Townhall : Building
{
    public ResourceBundle ResourceCapacity;
    [HideInInspector]
    public List<Unit> TownhallTrainableUnits;

    public Townhall()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 0, 0, 0);
        ResourceCapacity = new ResourceBundle(1000, 750, 750 ,750, 750);
        BuildingType = BuildingType.Townhall;
    }

    public Townhall(float health, ResourceBundle cost, ResourceBundle resourceCapacity, BuildingType type)
    {
        Health = health;
        Cost = cost;
        ResourceCapacity = resourceCapacity;
        BuildingType = type;
    }

    protected override void StartupActions()
    {
        TownhallTrainableUnits = SettingsManager.Instance.TownhallTrainableUnits;
    }

    protected override void OnSelectActions()
    {
        base.OnSelectActions();
        for (int i = 0; i < TownhallTrainableUnits.Count; i++)
        {
            if (GUI.Button(new Rect(Screen.width / 80, Screen.height / 85 - Screen.height / 12 * i, 100, 30),
                TownhallTrainableUnits[i].name))
            {
                TrainUnit(TownhallTrainableUnits[i]);
            }
        }
    }

    private void TrainUnit(Unit unit)
    {

    }

    public void SpawnBuilder()
    {

    }

    public void SpawnWorker()
    {

    }
}
