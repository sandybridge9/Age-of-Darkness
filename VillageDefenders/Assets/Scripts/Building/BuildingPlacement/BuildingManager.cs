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
    private AxisLock? WallBuildingAxisLock;
    private BoxCollider wallBoxCollider;
    private bool creatingWall;
    private Building lastWall = null;

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
            wallRenderer = currentBuilding.GetComponentInChildren<Renderer>();
            wallBounds = wallRenderer.bounds;
            stepSize = wallBoxCollider.bounds.size.x > wallBoxCollider.bounds.size.z
                ? wallBoxCollider.bounds.size.x
                : wallBoxCollider.bounds.size.z;
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
                float wallTileSize = wallBoxCollider.size.x > wallBoxCollider.size.z
                    ? wallBoxCollider.size.x
                    : wallBoxCollider.size.z;

                if (WallBuildingAxisLock != null)
                {
                    if (WallBuildingAxisLock.Value.Axis == 'x')
                    {
                        currentBuildingSelection.transform.position = new Vector3(
                            WallBuildingAxisLock.Value.Value,
                            currentBuildingSelection.transform.position.y, //Position Y is set in HeightChecking script
                            Mathf.Floor(hitInfo.point.z / wallTileSize) * wallTileSize);
                    }
                    else
                    {
                        currentBuildingSelection.transform.position = new Vector3(
                            Mathf.Floor(hitInfo.point.x / wallTileSize) * wallTileSize,
                            currentBuildingSelection.transform.position.y, //Position Y is set in HeightChecking script
                            WallBuildingAxisLock.Value.Value);
                    }
                }
                else
                {
                    currentBuildingSelection.transform.position = new Vector3(
                        Mathf.Floor(hitInfo.point.x / wallTileSize) * wallTileSize,
                        currentBuildingSelection.transform.position.y, //Position Y is set in HeightChecking script
                        Mathf.Floor(hitInfo.point.z / wallTileSize) * wallTileSize);
                }
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
        if (Input.GetKey(KeyCode.R))
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
            //PlaceSpammableBuilding();
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

    //Axis lock is used for wall building, to make straight line wall building easier.
    struct AxisLock
    {
        public char Axis { get; set; }
        public float Value { get; set; }

        public AxisLock(char axis, float value)
        {
            this.Axis = axis;
            this.Value = value;
        }
    }

    private Vector3? startingLocation;
    private Vector3? endLocation;
    private Renderer wallRenderer;
    private Bounds? wallBounds;
    private List<Building> walls;
    private float stepSize;
    private bool isBuildingWalls;

    private void PlaceWall()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            isBuildingWalls = true;
            if (startingLocation == null)
            {
                //position where mouse button was initially clicked
                startingLocation = currentBuildingSelection.transform.position;
                endLocation = currentBuildingSelection.transform.position;
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
                wallBounds = null;
                wallRenderer = null;
            }
        }
    }

    //Makes a wall between initial click point and current mouse position
    private void MakeWallBetweenPoints()
    {
        //Gets a number of walls needed in both X and Z axis.
        //Y value is 0 because you can't stack walls on top of each other. You just can't, its hardly possible.
        Vector3 wallCount = new Vector3((endLocation.Value.x - startingLocation.Value.x) / stepSize, 0,
                                        (endLocation.Value.z - startingLocation.Value.z) / stepSize);
        //Setting first wall location
        Vector3 nextWallLocation =  new Vector3(startingLocation.Value.x,
            HeightChecking.GetOptimalHeightAtWorldPoint(startingLocation.Value.x, startingLocation.Value.z),
            startingLocation.Value.z);
        //Setting maximum iteration count to avoid infinite loop
        int iterationCount = 0;
        while (nextWallLocation != endLocation && iterationCount < 500 && wallCount != new Vector3(0, 0, 0))
        {
            var nextWall = GameObject.Instantiate(currentBuilding, nextWallLocation, currentBuildingSelection.transform.rotation);
            ChangeColor(nextWall);
            walls.Add(nextWall);
            //Get next step for next wall. x / Abs(x) lets us determine where the next wall needs to be placed
            float nextWallCountX = Math.Round(wallCount.x) == 0 ? 0 : (float)Math.Round(wallCount.x / Math.Abs(wallCount.x));
            float nextWallCountZ = Math.Round(wallCount.z) == 0 ? 0 : (float)Math.Round(wallCount.z / Math.Abs(wallCount.z));
            float nextWallOptimalY = HeightChecking.GetOptimalHeightAtWorldPoint(nextWallLocation.x + nextWallCountX * stepSize,
                nextWallLocation.z + nextWallCountZ * stepSize);
            nextWallLocation = new Vector3(nextWallLocation.x + nextWallCountX * stepSize, nextWallOptimalY, nextWallLocation.z + nextWallCountZ * stepSize);
            //Update wall counts
            float newWallCountX = Math.Round(wallCount.x) == 0 ? 0 : (float)Math.Round(wallCount.x - (wallCount.x / Math.Abs(wallCount.x)));
            float newWallCountZ = Math.Round(wallCount.z) == 0 ? 0 : (float)Math.Round(wallCount.z - (wallCount.z / Math.Abs(wallCount.z)));
            wallCount = new Vector3(newWallCountX, 0, newWallCountZ);
            iterationCount++;
        }
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

    //private void PlaceWall()
    //{
    //    //If user is holding x key down -> Lock X axis
    //    if (Input.GetKey(KeyCode.X))
    //    {
    //        if (WallBuildingAxisLock == null || WallBuildingAxisLock.Value.Axis == 'z')
    //        {
    //            WallBuildingAxisLock = new AxisLock('x', currentBuildingSelection.transform.position.x);
    //        }
    //    }
    //    //If user is holding z key down -> Lock Z axis
    //    else if (Input.GetKey(KeyCode.Z))
    //    {
    //        if (WallBuildingAxisLock == null || WallBuildingAxisLock.Value.Axis == 'x')
    //        {
    //            WallBuildingAxisLock = new AxisLock('z', currentBuildingSelection.transform.position.z);
    //        }
    //    }
    //    // User has released x/z buttons, reset lock
    //    else
    //    {
    //        WallBuildingAxisLock = null;
    //    }

    //    if (Input.GetKey(KeyCode.Mouse0))
    //    {
    //        if (IsPositionViable())
    //        {
    //            if (WallBuildingAxisLock != null)
    //            {
    //                if (WallBuildingAxisLock.Value.Axis == 'x')
    //                {
    //                    BuildBuilding(WallBuildingAxisLock.Value.Value,
    //                        currentBuildingHeightChecking.OptimalHeight,
    //                        currentBuildingSelection.transform.position.z,
    //                        currentBuildingSelection.transform.rotation);
    //                }
    //                else if (WallBuildingAxisLock.Value.Axis == 'z')
    //                {
    //                    BuildBuilding(currentBuildingSelection.transform.position.x,
    //                        currentBuildingHeightChecking.OptimalHeight,
    //                        WallBuildingAxisLock.Value.Value,
    //                        currentBuildingSelection.transform.rotation);
    //                }
    //            }
    //            else
    //            {
    //                BuildBuilding(currentBuildingSelection.transform.position.x,
    //                    currentBuildingHeightChecking.OptimalHeight,
    //                    currentBuildingSelection.transform.position.z,
    //                    currentBuildingSelection.transform.rotation);
    //            }
    //        }
    //    }
    //}

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

    private List<Renderer> GetRenderers(Building building)
    {
        List<Renderer> renderers= new List<Renderer>();
        Renderer parentRenderer = currentBuildingSelection.GetComponent<Renderer>();
        if (parentRenderer == null)
        {
            renderers = currentBuildingSelection.GetComponentsInChildren<Renderer>().ToList();
        }
        else
        {
            //Add parent object renderer
            renderers.Add(parentRenderer);
        }

        return renderers;
    }

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

    private void ChangeColor(Building building)
    {
        //Debug.Log("changing color");
        List<Renderer> renderers = GetRenderers(building);
        foreach (var rend in renderers)
        {
            int size = rend.materials.Length;
            Material[] newMaterials = new Material[size];
            for (int i = 0; i < size; i++)
            {
                newMaterials[i] = materialCanBuild;
            }
            rend.materials = newMaterials;
        }
    }

    #endregion
}
