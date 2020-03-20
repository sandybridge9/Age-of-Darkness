using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
	#region Fields

    private BuildingCollisionManager currentBuildingCollisionManager;      //Used to detect if there is already a building in place
    private Building currentBuilding;                       //Current building that needs to be placed
    private HeightChecking currentBuildingHeightChecking;   //Responsible for checking height of currently selected building
    private float tileSize = 0.5f;                          // Grid snapping step size
    private float rotationDelay = 60f;                      // Used to delay rotation
    private float cancelDelay = 60f;                        // Used to delay canceling
    private float placementDelay = 120f;
    private LayerMask groundLayerMask;

    #endregion

    #region Properties

    private List<Building> placeableBuildings;

	#endregion

	#region Overriden Methods

	void Start()
    {
        placeableBuildings = SettingsManager.Instance.PlaceableBuildings;
    }

    void Update()
    {
        if (currentBuilding != null)
        {
            MoveCurrentObjectToMouse();
            RotateBuilding();
            BuildingPlacement();
        }
        CancelSelection();
    }

    void OnGUI()
    {
        for (int i = 0; i < placeableBuildings.Count; i++)
        {
            if (GUI.Button(new Rect(Screen.width / 20, Screen.height / 15 + Screen.height / 12 * i, 100, 30), placeableBuildings[i].name))
            {
                SetItem(placeableBuildings[i]);
            }
        }
    }

    #endregion

    #region HelperMethods

    //Checks if current building position contains any other buildings
    bool IsPositionEmpty()
    {
        if (currentBuildingCollisionManager.Colliders.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Sets currently selected building
    public void SetItem(Building b)
    {
        currentBuilding = Instantiate<Building>(b);
        currentBuildingCollisionManager = currentBuilding.GetComponent<BuildingCollisionManager>();
        if (currentBuilding.GetComponent<HeightChecking>() != null)
        {
            currentBuilding.transform.GetComponent<HeightChecking>().enabled = true;
            currentBuildingHeightChecking = currentBuilding.GetComponent<HeightChecking>();
        }
    }

    private void MoveCurrentObjectToMouse()
    {
        groundLayerMask = SettingsManager.Instance.GroundLayerMask;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 10000f, groundLayerMask))
        {
            //Snapping
            currentBuilding.transform.position = new Vector3(
                Mathf.Floor(hitInfo.point.x / tileSize) * tileSize,
                currentBuilding.transform.position.y, //Position Y is set in HeightChecking script
                Mathf.Floor(hitInfo.point.z / tileSize) * tileSize);
        }
    }

    private void RotateBuilding()
    {
        if (Input.GetKey(KeyCode.R))
        {
            if (rotationDelay >= 60)
            {
                currentBuilding.transform.Rotate(Vector3.up, -90);
                rotationDelay = 0;
            }
        }
        else if (Input.GetKey(KeyCode.T))
        {
            if (rotationDelay >= 60)
            {
                currentBuilding.transform.Rotate(Vector3.up, 90);
                rotationDelay = 0;
            }
        }
        rotationDelay++;
    }

    private void BuildingPlacement()
    {
        if (currentBuilding.BuildingType == BuildingTypes.WoodenWall)
        {
            PlaceSpammableBuilding();
        }
        else if (currentBuilding.BuildingType == BuildingTypes.Townhall)
        {
            PlaceUniqueBuildingPlace();
        }
        else
        {
            PlaceBuilding();
        }
        placementDelay++;
    }

    //TODO: Wall placement
    private void PlaceSpammableBuilding()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (IsPositionEmpty())
            {
                if (currentBuildingHeightChecking.CanPlace)
                {
                    if (placementDelay >= 45f)
                    {
                        var newCopy = GameObject.Instantiate(currentBuilding, new Vector3(
                                currentBuilding.transform.position.x,
                                currentBuildingHeightChecking.OptimalHeight,
                                currentBuilding.transform.position.z),
                            currentBuilding.transform.rotation);
                        newCopy.GetComponent<HeightChecking>().enabled = false;
                        placementDelay = 0;
                    }
                }
            }
        }
    }

    private void PlaceBuilding()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (IsPositionEmpty())
            {
                if (currentBuildingHeightChecking.CanPlace)
                {
                    if (placementDelay >= 120f)
                    {
                        var newCopy = GameObject.Instantiate(currentBuilding, new Vector3(
                                currentBuilding.transform.position.x,
                                currentBuildingHeightChecking.OptimalHeight,
                                currentBuilding.transform.position.z),
                                currentBuilding.transform.rotation);
                                newCopy.GetComponent<HeightChecking>().enabled = false;
                                placementDelay = 0;
                    }
                }
            }
        }
    }

    //Placement for unique building(Buildings that you can't have several of)
    private void PlaceUniqueBuildingPlace()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (IsPositionEmpty())
            {
                if (currentBuildingHeightChecking.CanPlace)
                {
                    Debug.Log(currentBuilding.transform.position);
                    currentBuilding.transform.GetComponent<HeightChecking>().enabled = false;
                    currentBuildingHeightChecking = null;
                    currentBuildingCollisionManager = null;
                    currentBuilding = null;
                }
            }
        }
    }

    private void CancelSelection()
    {
        if (Input.GetKey(KeyCode.C))
        {
            if (cancelDelay >= 60)
            {
                if (currentBuilding != null)
                {
                    currentBuildingHeightChecking = null;
                    currentBuildingCollisionManager = null;
                    currentBuilding.Destroy();
                    currentBuilding = null;
                    cancelDelay = 0;
                }
            }
        }
        cancelDelay++;
    }

    #endregion
}
