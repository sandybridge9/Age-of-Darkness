using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    private PlaceableObject currentBuildingProperties; //Used to detect if there is already a building in place
    private Transform currentBuilding;          //Current building that needs to be placed
    private bool hasPlaced;                     //Is set to true when selected building is placed
    private float buildingRotationSpeed = 1f;   //How fast building rotates using R and T buttons
    private float tileSize = 0.5f;              // Grid snapping step size
    private float rotationDelay = 0.5f;         // Used to delay rotation
    private float timer = 0.5f;                 //Used to delay rotation

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition; //Gets mouse position
        mousePos = new Vector3(mousePos.x, mousePos.y, transform.position.y + 5f); //Gets 3D mouse position and uses camera's y axis as z axis
        Vector3 truePos = GetComponent<Camera>().ScreenToWorldPoint(mousePos); //Transforms position from screen space to world space

        if (currentBuilding != null && hasPlaced == false)
        {
            Vector3 gridPosition = new Vector3();
            gridPosition.x = Mathf.Floor(truePos.x / tileSize) * tileSize;
            gridPosition.z = Mathf.Floor(truePos.z / tileSize) * tileSize;
            currentBuilding.position = new Vector3(gridPosition.x, currentBuilding.position.y, gridPosition.z);

            //Current building placement using mouse button
            if(Input.GetMouseButtonDown(0))
            {
                if (isPositionEmpty())
                {
                    hasPlaced = true;
                }
            }
            //Current building rotation using R and T buttons
            if (Input.GetKey(KeyCode.R))
            {
                timer += Time.deltaTime;
                if (timer > rotationDelay)
                {
                    currentBuilding.Rotate(0, 90, 0, Space.World);
                    timer -= rotationDelay;
                }
            }
            else if (Input.GetKey(KeyCode.T))
            {
                timer += Time.deltaTime;
                if (timer > rotationDelay)
                {
                    currentBuilding.transform.Rotate(0, -90, 0, Space.World);
                    timer -= rotationDelay;
                }
            }
        }
    }
    //Checks if current building position contains any other buildings
    bool isPositionEmpty()
    {
        if(currentBuildingProperties.colliders.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    //Sets currently selected building
    public void SetItem(GameObject b)
    {
        hasPlaced = false;
        currentBuilding = ((GameObject)Instantiate(b)).transform;
        currentBuildingProperties = currentBuilding.GetComponent<PlaceableObject>();
    }
}
