using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    //private LayerMask buildingLayerMask;
    //private LayerMask unitLayerMask;
    //public Transform SelectionArea;
    private LayerMask combinedMask;
    private List<GameObject> currentSelections;
    private GameObject currentSelection;

    private bool needsClearing = false;

    void Start()
    {
        //buildingLayerMask = SettingsManager.Instance.BuildingLayerMask;
        //unitLayerMask = SettingsManager.Instance.UnitLayerMask;
        combinedMask = (1 << LayerMask.NameToLayer("Building")) | (1 << LayerMask.NameToLayer("Unit"));
        //combinedMask = (1 << SettingsManager.Instance.BuildingLayerMask.value) | (1 << SettingsManager.Instance.UnitLayerMask.value);
        currentSelections = new List<GameObject>();
    }

    void Update()
    {
        ShootRay();
        Debug.Log(currentSelections.Count);
    }

    private void ShootRay()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!SettingsManager.Instance.BuildingManager.HasSelectedBuilding())
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 1000f, combinedMask))
                {
                    //If shift key is held then add selected gameobjects to list
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        SelectGameObject(hitInfo.transform.gameObject, true);
                    }
                    else
                    {
                        SelectGameObject(hitInfo.transform.gameObject, false);
                    }
                }
                //If clicked elsewhere - clear selection
                else
                {
                    needsClearing = true;
                }
            }
            if(needsClearing)
            {
                ClearSelection();
            }
        }
    }

    //Method that is called upon object selection
    private void SelectGameObject(GameObject selectedObject, bool addToList)
    {
        if (selectedObject.GetComponent<Building>())
        {
            var building = selectedObject.GetComponent<Building>();
            building.Select();
            //Check if currentSelections list has any units selected, and if yes, then clear it
            //Because units and buildings can't be selected at the same time
            if (currentSelections.Any(s=>s.GetComponent<Unit>() != null))//Where(s => s.GetComponent<Unit>() != null).Any())
            {
                ClearSelection();
            }
        }
        else if (selectedObject.GetComponent<Unit>())
        {
            var unit = selectedObject.GetComponent<Unit>();
            unit.Select();
            //Check if currentSelections list has any buildings selected, and if yes, then clear it
            //Because units and buildings can't be selected at the same time
            if (currentSelections.Any(s => s.GetComponent<Building>() != null))//Where(s => s.GetComponent<Unit>() != null).Any())
            {
                ClearSelection();
            }
        }
        if (addToList && !currentSelections.Contains(selectedObject))
        {
            currentSelections.Add(selectedObject);
        }
        else
        {
            ClearSelection();
            currentSelection = selectedObject;
        }
    }

    public void ClearSelection()
    {
        Debug.Log("Clearing selections");
        foreach (var selection in currentSelections)
        {
            var b = selection.GetComponent<Building>();
            if (b != null)
            {
                b.DeSelect();
            }
            else
            {
                var u = selection.GetComponent<Unit>();
                if (u != null)
                {
                    u.DeSelect();
                }
            }
        }
        currentSelections = new List<GameObject>();
        if (currentSelection != null)
        {
            var b = currentSelection.GetComponent<Building>();
            if (b != null)
            {
                b.DeSelect();
            }
            else
            {
                var u = currentSelection.GetComponent<Unit>();
                if (u != null)
                {
                    u.DeSelect();
                }
            }
            currentSelection = null;
        }
        needsClearing = false;
    }

    public void RemoveGameObjectFromSelection(GameObject _gameObject)
    {
        currentSelection = null;
        currentSelections.Remove(_gameObject);
    }
}
