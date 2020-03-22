using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private LayerMask buildingLayerMask;
    private GameObject CurrentSelection;

    void Start()
    {
        buildingLayerMask = SettingsManager.Instance.BuildingLayerMask;
    }

    void Update()
    {
        ShootRay();
    }

    private void ShootRay()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!SettingsManager.Instance.BuildingManager.HasSelectedBuilding())
            {
                Debug.Log("Has no selected buildings");
                DeSelectCurrentGameObject();
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 1000f, buildingLayerMask))
                {
                    if (hitInfo.transform.GetComponent<Building>())
                    {
                        Debug.Log("Selected a building");
                        SelectGameObject(hitInfo.transform.gameObject);
                    }
                    //TODO: else if check if hitInfo hit an unit
                }
            }
            else
            {
                Debug.Log("Deselected");
                DeSelectCurrentGameObject();
            }
        }
    }

    public void SelectGameObject(GameObject gameObject)
    {
        CurrentSelection = gameObject;
        if (gameObject.GetComponent<Building>())
        {
            var building = gameObject.GetComponent<Building>();
            if (building.IsPlaced)
            {
                building.Select();
            }
        }
        //TODO: Impelement unit selection
    }

    public void DeSelectCurrentGameObject()
    {
        if (CurrentSelection != null && CurrentSelection.GetComponent<Building>())
        {
            var building = CurrentSelection.GetComponent<Building>();
            if (building.IsPlaced)
            {
                building.DeSelect();
            }
        }
        //TODO: Implement unit deselection
    }
}
