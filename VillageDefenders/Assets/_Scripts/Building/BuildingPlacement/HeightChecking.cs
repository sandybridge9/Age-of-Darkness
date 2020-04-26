using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class HeightChecking : MonoBehaviour
{
    #region Fields

    private float tallestHeight;
    private float shortestHeight;
    private float greatestDifference;

    private LayerMask groundLayerMask;
    private GameObject heightCheckers;
    private float BuildingHeightCheckerSensitivity;

    #endregion

    #region Properties

    public float MaximumHeightDifference = 1f;
    [HideInInspector] public bool CanPlace = false;
    [HideInInspector] public float OptimalHeight;

    #endregion

    #region Overriden Methods

    void Start()
    {
        heightCheckers = transform.Find("HeightCheckers").gameObject ?? null;
        BuildingHeightCheckerSensitivity = SettingsManager.Instance.BuildingHeightCheckerSensitivity;
    }

    void Update()
    {
        if (groundLayerMask == 0)
        {
            groundLayerMask = SettingsManager.Instance.GroundLayerMask;
        }

        if (heightCheckers != null)
        {
            CheckHeights();
        }
        else
        {
            OptimalHeight = transform.position.y;
            CanPlace = true;
        }
    }

    #endregion

    #region Height Checking

    public void CheckHeights()
    {
        //Reset values
        tallestHeight = 0;
        shortestHeight = 0;
        List<float> heights = new List<float>();
        //Number of measurements that were successful
        int successCount = 0;
        //iterate through all height checkers
        for (int i = 0; i < heightCheckers.transform.childCount; i++)
        {
            var heightChecker = heightCheckers.transform.GetChild(i).gameObject;
            RaycastHit info;
            //If raycast hits the ground
            if (Physics.Raycast(heightChecker.transform.position, Vector3.down, out info, 1000, groundLayerMask))
            {
                heights.Add(info.point.y);
                successCount++;
                if (info.point.y > tallestHeight)
                {
                    tallestHeight = info.point.y;
                }

                if (info.point.y < shortestHeight || shortestHeight == 0)
                {
                    shortestHeight = info.point.y;
                }
            }
        }

        //If Ground wasn't hit by some of the height checkers
        if (successCount < heightCheckers.transform.childCount)
        {
            //Try to correct the height of currently selected building
            transform.position += new Vector3(0, BuildingHeightCheckerSensitivity, 0);
        }
        //If ground was hit by all height checkers, find optimal height for building placement
        //And set the correct height for currently selected building (to avoid floating).
        else
        {
            SetOptimalHeightForBuildingPlacement();
            SetCorrectHeightForCurrentlySelectedBuilding(heights);
        }

        greatestDifference = tallestHeight - shortestHeight;
        CanPlace = !(greatestDifference > MaximumHeightDifference);
    }

    //Gets the optimal height for building placement
    public void SetOptimalHeightForBuildingPlacement()
    {
        //All height checkers have the almost same Y coordinate, so it doesn't matter which one we take
        var heightCheckerHeight = heightCheckers.transform.GetChild(0).gameObject.transform.position.y;
        //Calculate difference according to shortestHeight, to avoid Placed building floating
        var difference = heightCheckerHeight - shortestHeight;
        OptimalHeight = transform.position.y - difference;
    }

    public void SetCorrectHeightForCurrentlySelectedBuilding(List<float> heights)
    {
        float lowestDifference = 0;
        for (int i = 0; i < heights.Count; i++)
        {
            var heightCheckerHeight = heightCheckers.transform.GetChild(i).gameObject.transform.position.y;
            var difference = heightCheckerHeight - tallestHeight;
            if (lowestDifference == 0 || difference < lowestDifference)
            {
                lowestDifference = difference;
            }
        }

        transform.position = new Vector3(transform.position.x, transform.position.y - lowestDifference + 0.1f,
            transform.position.z);
    }

    public static float GetOptimalHeightAtWorldPoint(float x, float z)
    {
        RaycastHit info;
        //If raycast hits the ground
        if (Physics.Raycast(new Vector3(x, 500, z), Vector3.down, out info, 1000, SettingsManager.Instance.GroundLayerMask))
        {
            //Return a slighly bigger height to avoid objects appearing in the ground
            return info.point.y + 0.1f;
        }
        else
        {
            return 0;
        }
    }

    #endregion
}
