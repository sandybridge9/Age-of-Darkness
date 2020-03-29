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
    private const float tileSize = 0.5f; // Grid snapping step size
    private float rotationDelay = 60f; // Used to delay rotation
    private float cancelDelay = 60f; // Used to delay canceling
    private float placementDelay = 120f;
    private LayerMask groundLayerMask;
    private Material materialCanBuild;
    private Material materialCantBuild;

    //Fields used in wall placement
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
        walls = new List<Building>();
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

    void OnGUI()
    {
        for (int i = 0; i < placeableBuildings.Count; i++)
        {
            if (GUI.Button(new Rect(Screen.width / 20, Screen.height / 15 + Screen.height / 12 * i, 100, 30),
                placeableBuildings[i].name))
            {
                SetItem(placeableBuildings[i]);
            }
        }
    }

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

        if (b.BuildingType == BuildingTypes.WoodenWall)
        {
            wallBoxCollider = currentBuildingSelection.GetComponentInChildren<BoxCollider>();
            wallStepSizeX = wallBoxCollider.bounds.size.x;
            wallStepSizeZ = wallBoxCollider.bounds.size.z;
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
            if (currentBuildingSelection.BuildingType == BuildingTypes.WoodenWall)
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
        if (Input.GetKey(KeyCode.R) && currentBuilding.BuildingType != BuildingTypes.WoodenWall)
        {
            if (rotationDelay >= 60)
            {
                currentBuildingSelection.transform.Rotate(Vector3.up, -90);
                rotationDelay = 0;
            }
        }
        else if (Input.GetKey(KeyCode.T))
        {
            if (rotationDelay >= 60)
            {
                currentBuildingSelection.transform.Rotate(Vector3.up, 90);
                rotationDelay = 0;
            }
        }

        rotationDelay++;
    }

    private void BuildingPlacement()
    {
        if (currentBuildingSelection.BuildingType == BuildingTypes.WoodenWall)
        {
            PlaceWall();
        }
        else if (currentBuildingSelection.BuildingType == BuildingTypes.Townhall)
        {
            PlaceUniqueBuilding();
        }
        else
        {
            PlaceBuilding();
        }
        placementDelay++;
    }
    
    private Vector3? startingLocation;
    private Vector3? endLocation;
    private List<Building> walls;
    private List<Renderer> allWallRenderers;
    private bool isBuildingWalls;

    //2 separate step sizes in case wall isn't square shaped
    private float wallStepSizeX;
    private float wallStepSizeZ;


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
                walls = new List<Building>();
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
                walls = null;
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
            walls.Add(firstWall);
        }
        //Setting maximum iteration count to avoid infinite loop
        int iterationCount = 0;
        while (nextWallLocation != endLocation && iterationCount < 500 && wallCount != new Vector3(0, 0, 0))
        {
            var nextWall = GameObject.Instantiate(currentBuilding, nextWallLocation, currentBuildingSelection.transform.rotation);
            //ChangeColor(nextWall);
            walls.Add(nextWall);
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
            walls.Add(lastWall);
        }
        //ChangeWallsColor();
    }

    //Checks if wall placement end location was changed
    private bool LocationChanged()
    {
        return currentBuildingSelection.transform.position != endLocation;
    }

    private void ClearWallList()
    {
        foreach (var wall in walls)
        {
            wall.Destroy();
        }
        walls = new List<Building>();
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

    //Placement for unique building(Buildings that you can't have several of)
    private void PlaceUniqueBuilding()
    {
        if (Input.GetKey(KeyCode.Mouse0) && IsPositionViable())
        {
            BuildBuilding(currentBuildingSelection.transform.position.x,
                currentBuildingHeightChecking.OptimalHeight,
                currentBuildingSelection.transform.position.z,
                currentBuildingSelection.transform.rotation);
            CleanUp();
        }
    }

    private void BuildWalls()
    {
        foreach (var wall in walls)
        {
            wall.IsPlaced = true;
            wall.GetComponent<HeightChecking>().enabled = false;
            wall.GetComponent<BuildingCollisionManager>().enabled = false;
            allBuildings.Add(wall);
        }
        //Detach placed walls from the list
        walls = new List<Building>();
        isBuildingWalls = false;
        currentBuildingCollisionManager.ResetCollision();
    }

    //Method responsible for instantiating a new building and setting up its components
    private void BuildBuilding(float x, float y, float z, Quaternion rotation)
    {
        BuildBuilding(x, y, z, rotation, currentBuilding);
    }

    //Method responsible for instantiating a new building and setting up its components
    private void BuildBuilding(float x, float y, float z, Quaternion rotation, Building building)
    {
        var newCopy = GameObject.Instantiate(building, new Vector3(x,y,z), rotation);
        newCopy.IsPlaced = true;
        newCopy.GetComponent<HeightChecking>().enabled = false;
        newCopy.GetComponent<BuildingCollisionManager>().enabled = false;
        allBuildings.Add(newCopy);
    }

    private void CancelSelection()
    {
        if (Input.GetKey(KeyCode.C) && cancelDelay >= 60 && currentBuildingSelection != null)
        {
            if (walls != null)
            {
                ClearWallList();
            }
            CleanUp();
            cancelDelay = 0;
        }
        cancelDelay++;
    }

    public void CleanUp()
    {
        currentBuildingHeightChecking = null;
        currentBuildingCollisionManager = null;
        currentlySelectedBuildingRenderers = new List<Renderer>();
        walls = new List<Building>();
        isBuildingWalls = false;
        if (currentBuilding != null)
        {
            currentBuildingSelection.Destroy();
            currentBuildingSelection = null;
            currentBuilding = null;
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
        //If parent object doesn't have a renderer, search in children
        if(parentRenderer == null)
        {
            currentlySelectedBuildingRenderers = currentBuildingSelection.GetComponentsInChildren<Renderer>().ToList();
        }
        else
        {
            //Add parent object renderer
            currentlySelectedBuildingRenderers.Add(parentRenderer);
        }
    }

    //private List<Renderer> GetRenderers(Building building)
    //{
    //    List<Renderer> renderers= new List<Renderer>();
    //    Renderer parentRenderer = currentBuildingSelection.GetComponent<Renderer>();
    //    if (parentRenderer == null)
    //    {
    //        renderers = currentBuildingSelection.GetComponentsInChildren<Renderer>().ToList();
    //    }
    //    else
    //    {
    //        //Add parent object renderer
    //        renderers.Add(parentRenderer);
    //    }

    //    return renderers;
    //}

    //NOTE TO SELF: Implement differently if performance drops
    private void ChangeColor()
    {
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

    //private void ChangeColor(Building building)
    //{
    //    //Debug.Log("changing color");
    //    List<Renderer> renderers = GetRenderers(building);
    //    foreach (var rend in renderers)
    //    {
    //        int size = rend.materials.Length;
    //        Material[] newMaterials = new Material[size];
    //        for (int i = 0; i < size; i++)
    //        {
    //            newMaterials[i] = materialCanBuild;
    //        }
    //        rend.materials = newMaterials;
    //    }
    //}

    //private void ChangeWallsColor()
    //{
    //    //allWallRenderers = new List<Renderer>();
    //    foreach (var wall in walls)
    //    {
    //        var currentWallRenderers = new List<Renderer>();
    //        var parentRenderer = wall.GetComponent<Renderer>();
    //        if (parentRenderer == null)
    //        {
    //            var renderers = wall.GetComponentsInChildren<Renderer>().ToList();
    //            currentWallRenderers.AddRange(renderers);
    //        }
    //        else
    //        {
    //            currentWallRenderers.Add(parentRenderer);
    //        }

    //        //var heightChecker = wall.GetComponent<HeightChecking>();
    //        var collisionManager = wall.GetComponent<BuildingCollisionManager>();
    //        if (!collisionManager.IsOverlapping())
    //        {
    //            foreach (var r in currentWallRenderers)
    //            {
    //                int size = r.materials.Length;
    //                Material[] newMaterials = new Material[size];
    //                for (int i = 0; i < size; i++)
    //                {
    //                    newMaterials[i] = materialCanBuild;
    //                }
    //                r.materials = newMaterials;
    //            }
    //        }
    //        else
    //        {
    //            foreach (var r in currentWallRenderers)
    //            {
    //                int size = r.materials.Length;
    //                Material[] newMaterials = new Material[size];
    //                for (int i = 0; i < size; i++)
    //                {
    //                    newMaterials[i] = materialCantBuild;
    //                }
    //                r.materials = newMaterials;
    //            }
    //        }
    //    }

    //    //foreach (var r in allWallRenderers)
    //    //{
    //    //    int size = r.materials.Length;
    //    //    Material[] newMaterials = new Material[size];
    //    //    for (int i = 0; i < size; i++)
    //    //    {
    //    //        newMaterials[i] = materialCanBuild;
    //    //    }
    //    //    r.materials = newMaterials;
    //    //}
    //}

    #endregion
}
