using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    private Transform unitSpawnPoint;
    public List<Unit> BarracksTrainableUnits;

    public Barracks()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 50, 0, 0, 0);
        BuildingType = global::BuildingType.Barracks;
    }

    public Barracks(float health, ResourceBundle cost, BuildingType type)
    {
        Health = health;
        Cost = cost;
        BuildingType = type;
    }

    protected override void StartupActions()
    {
        BarracksTrainableUnits = SettingsManager.Instance.BarracksTrainableUnits;
    }

    //void OnGUI()
    //{
    //    for (int i = 0; i < BarracksTrainableUnits.Count; i++)
    //    {
    //        if (GUI.Button(new Rect(Screen.width / 80, Screen.height / 85 - Screen.height / 12 * i, 100, 30),
    //            BarracksTrainableUnits[i].name))
    //        {
    //            TrainUnit(BarracksTrainableUnits[i]);
    //        }
    //    }
    //}

    private void TrainUnit(Unit unit)
    {

    }
}
