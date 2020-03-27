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

    private BuildingCollisionManager currentBuildingCollisionManager; //Used to detect if there is already a building in place
    private Building currentBuilding; //Current building that needs to be placed -> for instantiating
    private Building currentBuildingSelection; //Current building for checking placement position, changing color etc.
    private HeightChecking currentBuildingHeightChecking; //Responsible for checking height of currently selected building

    private List<Renderer> currentlySelectedBuildingRenderers;
    private Material materialCanBuild;
    private Material materialCantBuild;

    private float tileSize = 0.5f; // Grid snapping step size
    private float rotationDelay = 60f; // Used to delay rotation
    private float cancelDelay = 60f; // Used to delay canceling
    private float placementDelay = 120f;
    private LayerMask groundLayerMask;

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
                //TODO this part is actually pretty bad, use GetComponentInChildren
                //var wallTileSize = currentBuildingSelection.transform.Find("Wood Wall").Find("Wall").Find("Mesh")
                //    .GetComponent<MeshRenderer>().bounds.size.z;
                //float wallTileSize = wallBoxCollider.bounds.size.x > wallBoxCollider.bounds.size.y 
                //    ? wallBoxCollider.bounds.size.x 
                //    : wallBoxCollider.bounds.size.y;
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

    //Axis lock is used for wall building, to make straight line wall buiilding easier.
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

    private void PlaceWall()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, 10000f, groundLayerMask))
            {
                if (startingLocation == null)
                {
                    startingLocation = currentBuildingSelection.transform.position; //hitInfo.point;
                }
                else
                {
                    foreach (var wall in walls)
                    {
                        wall.Destroy();
                    }
                }
                endLocation = hitInfo.point;
                MakeWallBetweenPoints();
                Debug.Log("start: " +startingLocation +" end: " +endLocation);
            }
        }
        //Reset locations on release
        else
        {
            if (startingLocation != null || endLocation != null)
            {
                foreach (var wall in walls)
                {
                    wall.Destroy();
                }
                startingLocation = null;
                endLocation = null;
                walls = null;
                wallBounds = null;
                wallRenderer = null;
            }
        }
    }

    private void MakeWallBetweenPoints()
    {
        walls = new List<Building>();
        Vector3 currentLocation = startingLocation.Value;
        Vector3 currentLocationFloored;
        if (wallBounds == null || wallRenderer == null)
        {
            wallRenderer = currentBuilding.GetComponentInChildren<Renderer>();
            wallBounds = wallRenderer.bounds;
            //stepSize = wallBounds.Value.size.x > wallBounds.Value.size.z
            //    ? wallBounds.Value.size.x
            //    : wallBounds.Value.size.z;
            //stepSize = wallBounds.Value.size.z;
            stepSize = wallBoxCollider.bounds.size.x > wallBoxCollider.bounds.size.z
                ? wallBoxCollider.bounds.size.x
                : wallBoxCollider.bounds.size.z;
        }
        endLocation = currentBuildingSelection.transform.position;
        Vector3 difference = new Vector3(endLocation.Value.x - startingLocation.Value.x,
                                        endLocation.Value.y - startingLocation.Value.y,
                                        endLocation.Value.z - startingLocation.Value.z);
        var firstWall = GameObject.Instantiate(currentBuilding, startingLocation.Value, currentBuildingSelection.transform.rotation);
        walls.Add(firstWall);
        Vector3 nextWallLocation = startingLocation.Value;
        Vector3 wallCount = new Vector3(difference.x / stepSize, difference.y, difference.z / stepSize);
        int count = 0;
        while (nextWallLocation != endLocation && count < 100 && wallCount != new Vector3(0, 0, 0))
        {
            Debug.Log(wallCount.x +" " +wallCount.z);
            count++;
            var nextWall = GameObject.Instantiate(currentBuilding, nextWallLocation, currentBuildingSelection.transform.rotation);
            walls.Add(nextWall);
            float wallCountX = Math.Round(wallCount.x) == 0 ? 0 : (float)Math.Round(wallCount.x / Math.Abs(wallCount.x));
            float wallCountZ = Math.Round(wallCount.z) == 0 ? 0 : (float)Math.Round(wallCount.z / Math.Abs(wallCount.z));
            nextWallLocation = new Vector3(nextWallLocation.x + wallCountX * stepSize,
                                            nextWallLocation.y,
                                            nextWallLocation.z + wallCountZ * stepSize);
            float newWallCountX = Math.Round(wallCount.x) == 0 ? 0 : (float)Math.Round(wallCount.x - (wallCount.x / Math.Abs(wallCount.x)));
            float newWallCountZ = Math.Round(wallCount.z) == 0 ? 0 : (float)Math.Round(wallCount.z - (wallCount.z / Math.Abs(wallCount.z)));
            wallCount = new Vector3(newWallCountX, wallCount.y, newWallCountZ);
            Debug.Log(wallCount.x + " " + wallCount.z);

            //Debug.Log(count +" CL: " + nextWallLocation + " EL: " +endLocation + " dif: " + wallCount);
            //Debug.Log("iteration: " +count + " wallcount: " + wallCount);
        }
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
    }

    private void CancelSelection()
    {
        if (Input.GetKey(KeyCode.C) && cancelDelay >= 60 && currentBuildingSelection != null)
        {
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
        if(currentBuilding != null)
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

    #endregion
}
