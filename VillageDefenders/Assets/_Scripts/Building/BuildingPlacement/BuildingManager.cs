using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class BuildingManager : MonoBehaviour
{
    #region Fields

    //Current building selection fields
    private BuildingCollisionManager currentBuildingCollisionManager; //Used to detect if there is already a building in place
    private Building currentBuilding; //Current building that needs to be placed -> for instantiating
    private Building currentBuildingSelection; //Current building for checking placement position, changing color etc.
    private HeightChecking currentBuildingHeightChecking; //Responsible for checking height of currently selected building
    private List<Renderer> currentlySelectedBuildingRenderers;

    //Private settings used in internal logic
    private List<Building> allBuildings; 
    private List<Building> allWalls;
    private const float tileSize = 0.25f; // Grid snapping step size
    private float rotationDelay = 10f; // Used to delay rotation
    private float cancelDelay = 10f; // Used to delay canceling
    private float placementDelay = 20f;
    private LayerMask groundLayerMask;
    private Material materialCanBuild;
    private Material materialCantBuild;

    //Fields used in wall placement
    private Vector3? startingLocation;
    private Vector3? endLocation;
    private List<Building> tempWalls;
    private bool isBuildingWalls;

    //2 separate step sizes in case wall isn't square shaped    
    private float wallStepSizeX;
    private float wallStepSizeZ;
    private BoxCollider wallBoxCollider;

    #endregion

    #region Properties

    private List<Building> placeableBuildings;

    #endregion

    #region Overriden Methods

    void Start()
    {
        placeableBuildings = SettingsManager.Instance.PlaceableBuildings;
        materialCanBuild = SettingsManager.Instance.MaterialCanBuild;
        materialCantBuild = SettingsManager.Instance.MaterialCantBuild;
        currentlySelectedBuildingRenderers = new List<Renderer>();
        allBuildings = new List<Building>();
        allWalls = new List<Building>();
        tempWalls = new List<Building>();
    }

    void Update()
    {
        if (currentBuilding != null)
        {
            MoveCurrentObjectToMouse();
            ChangeColor();
            RotateBuilding();
            BuildingPlacement();
        }
        CancelSelection();
    }

    //void OnGUI()
    //{
    //    for (int i = 0; i < placeableBuildings.Count; i++)
    //    {
    //        if (GUI.Button(new Rect(Screen.width / 20, Screen.height / 15 + Screen.height / 12 * i, 100, 30),
    //            placeableBuildings[i].name))
    //        {
    //            SetItem(placeableBuildings[i]);
    //        }
    //    }
    //}

    #endregion

    #region HelperMethods

    //Checks if current building position contains any other buildings and if building is on terrain that is even enough for placement
    bool IsPositionViable()
    {
        if (!currentBuildingCollisionManager.IsColliding() && currentBuildingHeightChecking.CanPlace)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Sets currently selected building
    public void SetItem(Building b)
    {
        CleanUp();
        currentBuilding = b;
        currentBuildingSelection = Instantiate(b);

        currentBuildingCollisionManager = currentBuildingSelection.GetComponent<BuildingCollisionManager>();
        if (currentBuildingSelection.GetComponent<HeightChecking>() != null)
        {
            currentBuildingSelection.transform.GetComponent<HeightChecking>().enabled = true;
            currentBuildingHeightChecking = currentBuildingSelection.GetComponent<HeightChecking>();
        }
        GetMeshRenderersOfCurrentBuilding();

        if (b.BuildingType == BuildingType.WoodenWall || b.BuildingType == BuildingType.StoneWall)
        {
            wallBoxCollider = currentBuildingSelection.GetComponentInChildren<BoxCollider>();
            wallStepSizeX = wallBoxCollider.bounds.size.x;
            wallStepSizeZ = wallBoxCollider.bounds.size.z;
        }

        if (b.BuildingType == BuildingType.StoneGatehouse ||
            b.BuildingType == BuildingType.StoneTower ||
            b.BuildingType == BuildingType.WoodenTower)
        {
            //Exclude walls from collision checking, because gate house and towers can be built on the walls
            currentBuildingCollisionManager.AddException(BuildingType.WoodenWall);
            currentBuildingCollisionManager.AddException(BuildingType.StoneWall);
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
            if (currentBuildingSelection.BuildingType == BuildingType.WoodenWall || currentBuildingSelection.BuildingType == BuildingType.StoneWall)
            {
                currentBuildingSelection.transform.position = new Vector3(
                    Mathf.Floor(hitInfo.point.x / wallStepSizeX) * wallStepSizeX,
                    currentBuildingSelection.transform.position.y, //Position Y is set in HeightChecking script
                    Mathf.Floor(hitInfo.point.z / wallStepSizeZ) * wallStepSizeZ);

            }
            else
            {
                currentBuildingSelection.transform.position = new Vector3(
                    Mathf.Floor(hitInfo.point.x / tileSize) * tileSize,
                    currentBuildingSelection.transform.position.y, //Position Y is set in HeightChecking script
                    Mathf.Floor(hitInfo.point.z / tileSize) * tileSize);
            }
        }
    }

    private void RotateBuilding()
    {
        //Walls can't be rotated to avoid various issues when wall isn't square
        if (Input.GetKey(KeyCode.R) && currentBuilding.BuildingType != BuildingType.WoodenWall && currentBuilding.BuildingType != BuildingType.StoneWall)
        {
            if (rotationDelay >= 10)
            {
                currentBuildingSelection.transform.Rotate(Vector3.up, -90);
                rotationDelay = 0;
            }
        }
        else if (Input.GetKey(KeyCode.T))
        {
            if (rotationDelay >= 10)
            {
                currentBuildingSelection.transform.Rotate(Vector3.up, 90);
                rotationDelay = 0;
            }
        }

        rotationDelay++;
    }

    private void BuildingPlacement()
    {
        if (currentBuildingSelection.BuildingType == BuildingType.WoodenWall || currentBuildingSelection.BuildingType == BuildingType.StoneWall)
        {
            PlaceWall();
        }
        else if (currentBuildingSelection.BuildingType == BuildingType.Townhall || currentBuildingSelection.BuildingType == BuildingType.Warehouse)
        {
            PlaceUniqueBuilding();
        }
        else if (currentBuildingSelection.BuildingType == BuildingType.StoneGatehouse || currentBuildingSelection.BuildingType == BuildingType.StoneTower ||
                 currentBuildingSelection.BuildingType == BuildingType.WoodenTower)
        {
            PlaceBuildingThatCanOverlapWithWalls();
        }
        else
        {
            PlaceBuilding();
        }
        placementDelay++;
    }

    #region WALL PLACEMENT

    private void PlaceWall()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            isBuildingWalls = true;
            if (startingLocation == null)
            {
                //position where mouse button was initially clicked
                startingLocation = currentBuildingSelection.transform.position;
                endLocation = startingLocation;
                tempWalls = new List<Building>();
                MakeWallBetweenPoints();
            }
            else if(LocationChanged())
            {
                ClearWallList();
                endLocation = currentBuildingSelection.transform.position;
                MakeWallBetweenPoints();
            }
        }
        //On release
        else
        {
            //If walls are currently being placed - build them.
            if (isBuildingWalls)
            {
                BuildWalls();
            }
            if (startingLocation != null || endLocation != null)
            {
                ClearWallList();
                startingLocation = null;
                endLocation = null;
                tempWalls = null;
            }
        }
    }

    //Makes a wall between initial click point and current mouse position
    private void MakeWallBetweenPoints()
    {
        //Gets a number of walls needed in both X and Z axis.
        //Y value is 0 because you can't stack walls on top of each other. You just can't.
        Vector3 wallCount = new Vector3((endLocation.Value.x - startingLocation.Value.x) / wallStepSizeX, 0,
                                        (endLocation.Value.z - startingLocation.Value.z) / wallStepSizeZ);
        //Setting first wall location
        Vector3 nextWallLocation =  new Vector3(startingLocation.Value.x,
            HeightChecking.GetOptimalHeightAtWorldPoint(startingLocation.Value.x, startingLocation.Value.z),
            startingLocation.Value.z);
        //If mouse was only clicked but not moved
        if (startingLocation == endLocation)
        {
            var firstWall = GameObject.Instantiate(currentBuilding, nextWallLocation, currentBuildingSelection.transform.rotation);
            tempWalls.Add(firstWall);
        }
        //Setting maximum iteration count to avoid infinite loop
        int iterationCount = 0;
        while (nextWallLocation != endLocation && iterationCount < 500 && wallCount != new Vector3(0, 0, 0))
        {
            var nextWall = GameObject.Instantiate(currentBuilding, nextWallLocation, currentBuildingSelection.transform.rotation);
            //ChangeColor(nextWall);
            tempWalls.Add(nextWall);
            //Get next step for next wall. x / Abs(x) lets us determine where the next wall needs to be placed
            float nextWallStepX = Math.Round(wallCount.x) == 0 
                ? 0
                : (float)(Math.Round(wallCount.x / Math.Abs(wallCount.x)) * wallStepSizeX);
            float nextWallStepZ = Math.Round(wallCount.z) == 0 
                ? 0
                : (float)(Math.Round(wallCount.z / Math.Abs(wallCount.z)) * wallStepSizeZ);
            float nextWallStepY = HeightChecking.GetOptimalHeightAtWorldPoint(
                nextWallLocation.x + nextWallStepX,
                nextWallLocation.z + nextWallStepZ);
            nextWallLocation = new Vector3(nextWallLocation.x + nextWallStepX, nextWallStepY, nextWallLocation.z + nextWallStepZ);
            //Update wall counts
            float newWallCountX = Math.Round(wallCount.x) == 0 ? 0 : (float)Math.Round(wallCount.x - (wallCount.x / Math.Abs(wallCount.x)));
            float newWallCountZ = Math.Round(wallCount.z) == 0 ? 0 : (float)Math.Round(wallCount.z - (wallCount.z / Math.Abs(wallCount.z)));
            wallCount = new Vector3(newWallCountX, 0, newWallCountZ);
            iterationCount++;
        }
        if (startingLocation != endLocation)
        {
            var lastWall = GameObject.Instantiate(currentBuilding, nextWallLocation, currentBuildingSelection.transform.rotation);
            tempWalls.Add(lastWall);
        }
    }

    private void BuildWalls()
    {
        foreach (var wall in tempWalls)
        {
            //Implement check for dublicate buildings in one location
            if (!CheckIfLocationIsOccupied(wall.transform.position) && SettingsManager.Instance.ResourceManager.SubtractBuildingCostFromCurrentResources(wall.Cost))
            {
                wall.IsPlaced = true;
                wall.GetComponent<HeightChecking>().enabled = false;
                wall.GetComponent<BuildingCollisionManager>().enabled = false;
                allWalls.Add(wall);
            }
            else
            {
                wall.Destroy();
            }
        }
        //Detach placed walls from the list
        tempWalls = new List<Building>();
        isBuildingWalls = false;
        currentBuildingCollisionManager.ResetCollision();
    }

    //Location check for walls
    private bool CheckIfLocationIsOccupied(Vector3 location)
    {
        bool occupied = false;
        foreach (var wall in allWalls)
        {
            Vector3 topLeft = new Vector3(wall.Collider.bounds.center.x - wall.Collider.bounds.extents.x,
                wall.Collider.bounds.center.y,
                wall.Collider.bounds.center.z + wall.Collider.bounds.extents.z);
            Vector3 topRight = new Vector3(wall.Collider.bounds.center.x + wall.Collider.bounds.extents.x,
                wall.Collider.bounds.center.y,
                wall.Collider.bounds.center.z + wall.Collider.bounds.extents.z);
            Vector3 bottomLeft = new Vector3(wall.Collider.bounds.center.x - wall.Collider.bounds.extents.x,
                wall.Collider.bounds.center.y,
                wall.Collider.bounds.center.z - wall.Collider.bounds.extents.z);
            Vector3 bottomRight = new Vector3(wall.Collider.bounds.center.x + wall.Collider.bounds.extents.x,
                wall.Collider.bounds.center.y,
                wall.Collider.bounds.center.z - wall.Collider.bounds.extents.z);
            if ((location.x >= topLeft.x && location.x <= topRight.x) && (location.z >= bottomLeft.z && location.z <= topLeft.z))
            {
                occupied = true;
                return occupied;
            }
        }
        foreach (var building in allBuildings)
        {
            //List<Vector3> cornerPositions = new List<Vector3>();
            Vector3 topLeft = new Vector3(building.Collider.bounds.center.x - building.Collider.bounds.extents.x,
                building.Collider.bounds.center.y,
                building.Collider.bounds.center.z + building.Collider.bounds.extents.z);
            Vector3 topRight = new Vector3(building.Collider.bounds.center.x + building.Collider.bounds.extents.x,
                building.Collider.bounds.center.y,
                building.Collider.bounds.center.z + building.Collider.bounds.extents.z);
            Vector3 bottomLeft = new Vector3(building.Collider.bounds.center.x - building.Collider.bounds.extents.x,
                building.Collider.bounds.center.y,
                building.Collider.bounds.center.z - building.Collider.bounds.extents.z);
            Vector3 bottomRight = new Vector3(building.Collider.bounds.center.x + building.Collider.bounds.extents.x,
                building.Collider.bounds.center.y,
                building.Collider.bounds.center.z - building.Collider.bounds.extents.z);
            if ((location.x >= topLeft.x && location.x <= topRight.x) && (location.z >= bottomLeft.z && location.z <= topLeft.z))
            {
                //Walls can overlap with towers and gatehouses a bit
                if (building.BuildingType == BuildingType.StoneGatehouse ||
                    building.BuildingType == BuildingType.StoneTower)
                {
                    occupied = false;
                }
                else
                {
                    occupied = true;
                }

                return occupied;
            }
        }
        return occupied;
    }

    //Checks if wall placement end location was changed
    private bool LocationChanged()
    {
        return currentBuildingSelection.transform.position != endLocation;
    }

    private void ClearWallList()
    {
        foreach (var wall in tempWalls)
        {
            wall.Destroy();
        }
        tempWalls = new List<Building>();
    }

    #endregion

    public void PlaceBuildingThatCanOverlapWithWalls()
    {
        if (Input.GetKey(KeyCode.Mouse0) && IsPositionViable() && placementDelay >= 40f)
        {
            ClearLocationForBuilding();
            BuildBuilding(currentBuildingSelection.transform.position.x,
                currentBuildingHeightChecking.OptimalHeight,
                currentBuildingSelection.transform.position.z,
                currentBuildingSelection.transform.rotation);
            placementDelay = 0;
        }
    }

    //Clear location from walls for buildings like towers, gatehouses and etc.
    public void ClearLocationForBuilding()
    {
        Collider c = currentBuildingSelection.Collider;
        Vector3 topLeft = new Vector3(c.bounds.center.x - c.bounds.extents.x,
            c.bounds.center.y,
            c.bounds.center.z + c.bounds.extents.z);
        Vector3 topRight = new Vector3(c.bounds.center.x + c.bounds.extents.x,
            c.bounds.center.y,
            c.bounds.center.z + c.bounds.extents.z);
        Vector3 bottomLeft = new Vector3(c.bounds.center.x - c.bounds.extents.x,
            c.bounds.center.y,
            c.bounds.center.z - c.bounds.extents.z);
        Vector3 bottomRight = new Vector3(c.bounds.center.x + c.bounds.extents.x,
            c.bounds.center.y,
            c.bounds.center.z - c.bounds.extents.z);
        List<Building> wallsToDelete = new List<Building>();
        foreach (var wall in allWalls)
        {
            Vector3 wallPosition = wall.transform.position;
            //Check if there is a wall bellow the building, but let them overlap a bit
            if ((wallPosition.x >= topLeft.x +0.1f && wallPosition.x <= topRight.x - 0.1f) && (wallPosition.z >= bottomLeft.z +0.1f && wallPosition.z <= topLeft.z -0.1f))
            {
                wallsToDelete.Add(wall);
            }
        }
        foreach (var wall in wallsToDelete)
        {
            wall.Delete();
        }
    }

    private void PlaceBuilding()
    {
        if (Input.GetKey(KeyCode.Mouse0) && IsPositionViable() && placementDelay >= 120f)
        {
            BuildBuilding(currentBuildingSelection.transform.position.x,
                currentBuildingHeightChecking.OptimalHeight,
                currentBuildingSelection.transform.position.z,
                currentBuildingSelection.transform.rotation);
            placementDelay = 0;
        }
    }

    //Placement for unique building(Buildings that you can't have several of: Townhall, Warehouse)
    private void PlaceUniqueBuilding()
    {
        if (Input.GetKey(KeyCode.Mouse0) && IsPositionViable())
        {
            //Check if current building type isn't already built
            if (!allBuildings.Where(b => b.BuildingType == currentBuilding.BuildingType).ToList().Any())
            {
                BuildBuilding(currentBuildingSelection.transform.position.x,
                    currentBuildingHeightChecking.OptimalHeight,
                    currentBuildingSelection.transform.position.z,
                    currentBuildingSelection.transform.rotation);
                ExtendMaximumResourceCapacity();
                CleanUp();
            }
        }
    }

    //When building Townhall or Warehouse, extends maximum resource capacity
    private void ExtendMaximumResourceCapacity()
    {
        if (currentBuilding is Townhall)
        {
            SettingsManager.Instance.ResourceManager.BuildTownhall(currentBuilding as Townhall);
        }
        else if (currentBuilding is Warehouse)
        {
            SettingsManager.Instance.ResourceManager.BuildWarehouse(currentBuilding as Warehouse);
        }
    }

    //Method responsible for instantiating a new building and setting up its components
    private void BuildBuilding(float x, float y, float z, Quaternion rotation)
    {
        BuildBuilding(x, y, z, rotation, currentBuilding);
    }

    //Method responsible for instantiating a new building and setting up its components
    private void BuildBuilding(float x, float y, float z, Quaternion rotation, Building building)
    {
        if (SettingsManager.Instance.ResourceManager.SubtractBuildingCostFromCurrentResources(building.Cost))
        {
            var newCopy = GameObject.Instantiate(building, new Vector3(x, y, z), rotation);
            newCopy.IsPlaced = true;
            newCopy.GetComponent<HeightChecking>().enabled = false;
            newCopy.GetComponent<BuildingCollisionManager>().enabled = false;
            allBuildings.Add(newCopy);
        }
    }

    //Press C to cancel currently selected building
    private void CancelSelection()
    {
        if (Input.GetKey(KeyCode.C) && cancelDelay >= 10 && currentBuildingSelection != null)
        {
            if (tempWalls != null)
            {
                ClearWallList();
            }
            CleanUp();
            cancelDelay = 0;
        }
        cancelDelay++;
    }

    //Performs a clean up, resets all fields and properties
    public void CleanUp()
    {
        currentBuildingHeightChecking = null;
        currentBuildingCollisionManager = null;
        currentlySelectedBuildingRenderers = new List<Renderer>();
        tempWalls = null;
        isBuildingWalls = false;
        startingLocation = null;
        endLocation = null;
        wallStepSizeX = 0;
        wallStepSizeZ = 0;
        wallBoxCollider = null;
        if (currentBuilding != null)
        {
            currentBuildingSelection.Destroy();
            currentBuildingSelection = null;
            currentBuilding = null;
        }
    }

    //Deletes a building from allBuildings/allWalls list <-- Performed on Building.Delete()
    public void DeleteBuildingFromList(Building building)
    {
        if (building.BuildingType == BuildingType.WoodenWall || building.BuildingType == BuildingType.StoneWall)
        {
            SettingsManager.Instance.ResourceManager.ReturnPercentageOfBuildingCost(building.Cost, 50);
            allWalls.Remove(building);
        }
        else
        {
            SettingsManager.Instance.ResourceManager.ReturnPercentageOfBuildingCost(building.Cost, 25);
            allBuildings.Remove(building);
        }
    }

    public bool HasSelectedBuilding()
    {
        return currentBuildingSelection != null;
    }

    //To get Renderer component from Prefabs, that have main renderer attached to one of its child objects instead of parent
    private void GetMeshRenderersOfCurrentBuilding()
    {
        Renderer parentRenderer = currentBuildingSelection.GetComponent<Renderer>();
        if (parentRenderer != null)
        {
            currentlySelectedBuildingRenderers.Add(parentRenderer);
        }
        currentlySelectedBuildingRenderers.AddRange(currentBuildingSelection.GetComponentsInChildren<Renderer>().ToList());
    }

    private void ChangeColor()
    {
        if (currentBuilding.BuildingType == BuildingType.WoodenWall ||
            currentBuilding.BuildingType == BuildingType.StoneWall)
        {
            return;
        }
        //If position is clear and the height is good - set green color for all materials in all renderers
        if (IsPositionViable())
        {
            foreach(var renderer in currentlySelectedBuildingRenderers)
            {
                int size = renderer.materials.Length;
                Material[] newMaterials = new Material[size];
                for (int i = 0; i < size; i++)
                {
                    newMaterials[i] = materialCanBuild;
                }
                renderer.materials = newMaterials;
            }
        }
        //Else - set red color for all materials in all renderers
        else
        {
            foreach(var renderer in currentlySelectedBuildingRenderers)
            {
                int size = renderer.materials.Length;
                Material[] newMaterials = new Material[size];
                for (int i = 0; i < size; i++)
                {
                    newMaterials[i] = materialCantBuild;
                }
                renderer.materials = newMaterials;
            }
        }
    }

    public Building GetTownhall()
    {
        var th = allBuildings.Where(x => x.BuildingType == BuildingType.Townhall).ToList();
        return th.Count != 0 ? th.First() : null;
    }

    public Building GetWarehouse()
    {
        var w = allBuildings.Where(x => x.BuildingType == BuildingType.Warehouse).ToList();
        return w.Count != 0 ? w.First() : null;
    }

    public Building GetBarracks()
    {
        var w = allBuildings.Where(x => x.BuildingType == BuildingType.Barracks).ToList();
        return w.Count != 0 ? w.First() : null;
    }

    #endregion
}
