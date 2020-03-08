using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
	#region Fields

	private BuildingPlacement buildingPlacement;

	#endregion

	#region Properties

	public List<Building> PlaceableBuildings;

	#endregion

	#region Overriden Methods

	void Start()
    {
        buildingPlacement = GetComponent<BuildingPlacement>();
    }

    void Update()
    {

    }

    void OnGUI()
    {
        for (int i = 0; i < PlaceableBuildings.Count; i++)
        {
            if (GUI.Button(new Rect(Screen.width / 20, Screen.height / 15 + Screen.height / 12 * i, 100, 30), PlaceableBuildings[i].name))
            {
                buildingPlacement.SetItem(PlaceableBuildings[i]);
            }
        }
    }

	#endregion
}
