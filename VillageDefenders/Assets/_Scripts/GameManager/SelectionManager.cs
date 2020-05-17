using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    #region FIELDS

    private Building currentlySelectedBuilding;
    private List<Unit> currentlySelectedUnits;

    private Vector3 currentMousePoint;
    private Vector3? startPosition;
    private Vector3? endPosition;
    private bool isSelecting;

    #endregion

    #region PROPERTIES

    public GUIStyle MouseDragSkin;
    public float DistanceBetweenUnits = 0.5f;

    #endregion

    #region UNITY METHODS

    private void Start()
    {
        currentlySelectedUnits = new List<Unit>();
    }

    private void Update()
    {
        MakeSelection();
    }

    private void OnGUI()
    {
        if (isSelecting)
        {
            float boxWidth = Camera.main.WorldToScreenPoint(startPosition.Value).x -
                             Camera.main.WorldToScreenPoint(currentMousePoint).x;
            float boxHeight = Camera.main.WorldToScreenPoint(startPosition.Value).y -
                              Camera.main.WorldToScreenPoint(currentMousePoint).y;

            float boxLeft = Input.mousePosition.x;
            float boxTop = (Screen.height - Input.mousePosition.y) - boxHeight;
            GUI.backgroundColor = new Color(177f, 29f, 33f, 0.5f);
            Rect rect = new Rect(boxLeft, boxTop, boxWidth, boxHeight);
            GUI.Box(rect, "", MouseDragSkin);
        }
    }

    #endregion

    #region Methods

    private void MakeSelection()
    {
        //Cannot make selections while building is being placed
        if (!SettingsManager.Instance.BuildingManager.HasSelectedBuilding())
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = new Vector3();
                endPosition = new Vector3();
                currentMousePoint = new Vector3();
                ClearSelections();
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.transform.GetComponent<Building>())
                    {
                        currentlySelectedBuilding = hitInfo.transform.GetComponent<Building>();
                        if (currentlySelectedBuilding != null)
                        {
                            currentlySelectedBuilding.Select();
                        }
                        else
                        {
                            ClearSelections();
                        }
                    }
                    startPosition = hitInfo.point;
                }
            }

            if (Input.GetMouseButton(0) && currentlySelectedBuilding == null)
            {
                isSelecting = true;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    currentMousePoint = hitInfo.point;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (startPosition != null && currentlySelectedBuilding == null)
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        endPosition = hitInfo.point;
                        BoxSelectAnArea();
                    }
                }
                isSelecting = false;
            }
        }
        else
        {
            isSelecting = false;
            ClearSelections();
            startPosition = new Vector3();
            endPosition = new Vector3();
            currentMousePoint = new Vector3();
        }
    }

    private void BoxSelectAnArea()
    {
        var middle = new Vector3()
        {
            x = (startPosition.Value.x + endPosition.Value.x) / 2,
            y = (startPosition.Value.y + endPosition.Value.y) / 2,
            z = (startPosition.Value.z + endPosition.Value.z) / 2
        };
        var halfExtents = new Vector3()
        {
            x = Math.Abs(Math.Abs(startPosition.Value.x) - Math.Abs(endPosition.Value.x)) / 2,
            y = (Math.Abs(Math.Abs(startPosition.Value.y) - Math.Abs(endPosition.Value.y)) / 2) + 200,
            z = Math.Abs(Math.Abs(startPosition.Value.z) - Math.Abs(endPosition.Value.z)) / 2
        };
        List<Collider> colliders = Physics.OverlapBox(middle, halfExtents).ToList();
        foreach (var c in colliders)
        {
            Unit u = c.GetComponent<Unit>();
            if (u != null && !currentlySelectedUnits.Contains(u))
            {
                u.Select();
                currentlySelectedUnits.Add(u);
            }
        }
    }

    private void ClearSelections()
    {
        if (currentlySelectedBuilding != null)
        {
            currentlySelectedBuilding.DeSelect();
            currentlySelectedBuilding = null;
        }

        foreach (var u in currentlySelectedUnits)
        {
            u.DeSelect();
        }
        currentlySelectedUnits.Clear();
    }

    public void RemoveUnitFromSelection(Unit unit)
    {
        currentlySelectedUnits.Remove(unit);
    }

    public void RemoveBuildingFromSelection(Building building)
    {
        if (currentlySelectedBuilding != null && currentlySelectedBuilding == building)
        {
            currentlySelectedBuilding.DeSelect();
            currentlySelectedBuilding = null;
        }
    }

    public Vector3 GetPositionForUnit(Unit unit, Vector3 destination)
    {
        if (currentlySelectedUnits.Contains(unit) && unit.IsSelected)
        {
            if (currentlySelectedUnits.Count > 1)
            {
                int currentUnitIndex = currentlySelectedUnits.FindIndex(u => u == unit);
                float angle = currentUnitIndex * (360f / currentlySelectedUnits.Count);
                Vector3 direction = ApplyRotationToVector(new Vector3(1, 0, 1), angle);
                Vector3 position = destination + direction * DistanceBetweenUnits;
                return position;
            }
            else
            {
                return destination;
            }
        }
        return destination;
    }

    private Vector3 ApplyRotationToVector(Vector3 vector, float angle)
    {
        Debug.Log(Quaternion.Euler(0, 0, angle) * vector);
        return Quaternion.Euler(0, 0, angle) * vector;
    }

    #endregion
}
