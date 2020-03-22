using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class BuildingManager : MonoBehaviour
{
    #region Fields

    private BuildingCollisionManager currentBuildingCollisionManager; //Used to detect if there is already a building in place
    private Building currentBuilding; //Current building that needs to be placed
    private HeightChecking currentBuildingHeightChecking; //Responsible for checking height of currently selected building
    private Renderer currentBuildingRenderer;
    private Material currentBuildingMaterial;
    private Material materialCanBuild;
    private Material materialCantBuild;

    private float tileSize = 0.5f; // Grid snapping step size
    private float rotationDelay = 60f; // Used to delay rotation
    private float cancelDelay = 60f; // Used to delay canceling
    private float placementDelay = 120f;
    private LayerMask groundLayerMask;

    private AxisLock? WallBuildingAxisLock;
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
        currentBuilding = Instantiate(b);
        //TODO fix wall transparent state
        currentBuildingRenderer = currentBuilding.GetComponent<Renderer>();
        currentBuildingMaterial = currentBuildingRenderer.material;
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
            if (currentBuilding.BuildingType == BuildingTypes.WoodenWall)
            {
                var wallTileSize = currentBuilding.transform.Find("Wood Wall").Find("Wall").Find("Mesh")
                    .GetComponent<MeshRenderer>().bounds.size.z;

                if (WallBuildingAxisLock != null)
                {
                    if (WallBuildingAxisLock.Value.Axis == 'x')
                    {
                        currentBuilding.transform.position = new Vector3(
                            WallBuildingAxisLock.Value.Value,
                            currentBuilding.transform.position.y, //Position Y is set in HeightChecking script
                            Mathf.Floor(hitInfo.point.z / wallTileSize) * wallTileSize);
                    }
                    else
                    {
                        currentBuilding.transform.position = new Vector3(
                            Mathf.Floor(hitInfo.point.x / wallTileSize) * wallTileSize,
                            currentBuilding.transform.position.y, //Position Y is set in HeightChecking script
                            WallBuildingAxisLock.Value.Value);
                    }
                }
                else
                {
                    currentBuilding.transform.position = new Vector3(
                        Mathf.Floor(hitInfo.point.x / wallTileSize) * wallTileSize,
                        currentBuilding.transform.position.y, //Position Y is set in HeightChecking script
                        Mathf.Floor(hitInfo.point.z / wallTileSize) * wallTileSize);
                }
            }
            else
            {
                currentBuilding.transform.position = new Vector3(
                    Mathf.Floor(hitInfo.point.x / tileSize) * tileSize,
                    currentBuilding.transform.position.y, //Position Y is set in HeightChecking script
                    Mathf.Floor(hitInfo.point.z / tileSize) * tileSize);
            }
        }
    }

    private void ChangeColor()
    {
        if (IsPositionEmpty() && currentBuildingHeightChecking.CanPlace)
        {
            Debug.Log("Can place");
            currentBuildingRenderer.material = null;
            currentBuildingRenderer.material = materialCanBuild;
        }
        else
        {
            Debug.Log("Can't place");
            currentBuildingRenderer.material = null;
            currentBuildingRenderer.material = materialCantBuild;
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
            //PlaceSpammableBuilding();
            PlaceWallBuilding();
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

    #region ADVANCED WALL PLACING

    //private void PlaceWall()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        StartWall();
    //    }
    //    else if (Input.GetMouseButtonUp(0))
    //    {
    //        SetWall();
    //    }
    //    else
    //    {
    //        if (lastWall != null)
    //        {
    //            UpdateWall();
    //        }
    //    }
    //}

    //private void StartWall()
    //{
    //    creatingWall = true;
    //    var startWall = GameObject.Instantiate(currentBuilding, currentBuilding.transform.position,
    //        currentBuilding.transform.rotation);
    //    startWall.IsPlaced = true;
    //    startWall.GetComponent<HeightChecking>().enabled = false;
    //    lastWall = startWall;
    //}

    //private void SetWall()
    //{
    //    creatingWall = false;
    //}

    //private void UpdateWall()
    //{
    //    if (currentBuilding.transform.position != lastWall.transform.position)
    //    {
    //        CreateNextWall();
    //    }
    //}

    //private void CreateNextWall()
    //{
    //    var wallSize = currentBuilding.GetComponent<BoxCollider>().bounds.size;
    //}

    //private void CreateWallChain()
    //{
    //    Vector3 startPosition = lastWall.transform.position;
    //    Vector3 endPosition = Input.mousePosition.normalized;
    //    Vector3 currentPosition = startPosition;
    //    float stepSize = currentBuilding.GetComponent<BoxCollider>().bounds.size.x;
    //    Vector3 difference = startPosition - endPosition;
    //    while(difference.x != currentPosition.x && difference)
    //    //groundLayerMask = SettingsManager.Instance.GroundLayerMask;
    //}

    #endregion

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

    private void PlaceWallBuilding()
    {
        //If user is holding x key down -> Lock X axis
        if (Input.GetKey(KeyCode.X))
        {
            if (WallBuildingAxisLock == null || WallBuildingAxisLock.Value.Axis == 'z')
            {
                Debug.Log("New Axis X Lock");
                WallBuildingAxisLock = new AxisLock('x', currentBuilding.transform.position.x);
            }
        }
        //If user is holding z key down -> Lock Z axis
        else if (Input.GetKey(KeyCode.Z))
        {
            if (WallBuildingAxisLock == null || WallBuildingAxisLock.Value.Axis == 'x')
            {
                WallBuildingAxisLock = new AxisLock('z', currentBuilding.transform.position.z);
            }
        }
        // User has released x/z buttons, reset lock
        else
        {
            WallBuildingAxisLock = null;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (IsPositionEmpty())
            {
                if (currentBuildingHeightChecking.CanPlace)
                {
                    if (WallBuildingAxisLock != null)
                    {
                        if (WallBuildingAxisLock.Value.Axis == 'x')
                        {
                            var newCopy = GameObject.Instantiate(currentBuilding, new Vector3(
                                    WallBuildingAxisLock.Value.Value,
                                    currentBuildingHeightChecking.OptimalHeight,
                                    currentBuilding.transform.position.z),
                                currentBuilding.transform.rotation);
                            newCopy.IsPlaced = true;
                            newCopy.GetComponent<HeightChecking>().enabled = false;
                            newCopy.GetComponent<Renderer>().material = currentBuildingMaterial;
                        }
                        else if(WallBuildingAxisLock.Value.Axis == 'z')
                        {
                            var newCopy = GameObject.Instantiate(currentBuilding, new Vector3(
                                    currentBuilding.transform.position.x,
                                    currentBuildingHeightChecking.OptimalHeight,
                                    WallBuildingAxisLock.Value.Value),
                                currentBuilding.transform.rotation);
                            newCopy.IsPlaced = true;
                            newCopy.GetComponent<HeightChecking>().enabled = false;
                            newCopy.GetComponent<Renderer>().material = currentBuildingMaterial;
                        }
                    }
                    else
                    {
                        var newCopy = GameObject.Instantiate(currentBuilding, new Vector3(
                                currentBuilding.transform.position.x,
                                currentBuildingHeightChecking.OptimalHeight,
                                currentBuilding.transform.position.z),
                            currentBuilding.transform.rotation);
                        newCopy.IsPlaced = true;
                        newCopy.GetComponent<HeightChecking>().enabled = false;
                        newCopy.GetComponent<Renderer>().material = currentBuildingMaterial;
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
                        newCopy.IsPlaced = true;
                        newCopy.GetComponent<HeightChecking>().enabled = false;
                        newCopy.GetComponent<Renderer>().material = currentBuildingMaterial;
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
                    currentBuilding.GetComponent<Renderer>().material = currentBuildingMaterial;
                    currentBuilding.IsPlaced = true;
                    currentBuildingHeightChecking = null;
                    currentBuildingCollisionManager = null;
                    currentBuildingRenderer = null;
                    currentBuildingMaterial = null;
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
                    currentBuildingRenderer = null;
                    currentBuildingMaterial = null;
                    currentBuilding.Destroy();
                    currentBuilding = null;
                    cancelDelay = 0;
                }
            }
        }
        cancelDelay++;
    }

    public bool HasSelectedBuilding()
    {
        Debug.Log("Checking if currently selected building exists");
        return currentBuilding != null;
    }

    #endregion
}
