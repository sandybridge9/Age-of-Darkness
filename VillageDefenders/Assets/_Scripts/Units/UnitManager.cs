using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    #region FIELDS

    private List<Unit> allUnits = new List<Unit>();

    private List<Unit> workers = new List<Unit>();
    private List<Unit> warriors = new List<Unit>();
    private List<Unit> enemies = new List<Unit>();

    private Vector3 workerSpawnLocation;
    private Vector3 warriorSpawnLocation;
    private bool isWorkerSpawnLocationSet = false;
    private bool isWarriorSpawnLocationSet = false;

    #endregion

    #region START AND UPDATE


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWorkerSpawnLocationSet)
        {
            Townhall townhall = (Townhall)SettingsManager.Instance.BuildingManager.GetTownhall();
            if (townhall != null)
            {
                workerSpawnLocation = townhall.transform.Find("SpawnPoint").position;
                isWorkerSpawnLocationSet = true;
            }
        }
        if (!isWarriorSpawnLocationSet)
        {
            Barracks barracks = (Barracks)SettingsManager.Instance.BuildingManager.GetBarracks();
            if (barracks != null)
            {
                warriorSpawnLocation = barracks.transform.Find("SpawnPoint").position;
                isWarriorSpawnLocationSet = true;
            }
        }
    }

    #endregion

    #region UNIT TRAINING

    public void SetUnit(Unit u)
    {
        switch (u)
        {
            case Worker w:
                if (isWorkerSpawnLocationSet)
                {
                    TrainWorker(w);
                }
                break;
            case Warrior w:
                if (isWarriorSpawnLocationSet)
                {
                    TrainWarrior(w);
                }
                break;
            case ArmedPeasant ap:
                if (isWarriorSpawnLocationSet)
                {
                    TrainWarrior(ap);
                }
                break;
        }
    }

    private void TrainWarrior(Unit u)
    {
        if (SettingsManager.Instance.ResourceManager.SubtractUnitCostFromCurrentResources(u.Cost))
        {
            Debug.Log("Trying to train warrior");
            var newCopy = GameObject.Instantiate(u, warriorSpawnLocation, Quaternion.identity);
            allUnits.Add(newCopy);
            warriors.Add(newCopy);
        }
    }    

    private void TrainWorker(Unit u)
    {
        if (SettingsManager.Instance.ResourceManager.SubtractUnitCostFromCurrentResources(u.Cost))
        {
            Debug.Log("Trying to train worker");
            var newCopy = GameObject.Instantiate(u, workerSpawnLocation, Quaternion.identity);
            allUnits.Add(newCopy);
            workers.Add(newCopy);
        }
    }

    #endregion

    #region UNIT REMOVAL AND OBJECTIVE

    public void RemoveUnitFromLists(Unit u)
    {
        Debug.Log(u);
        switch (u)
        {
            case Worker w:
                workers.Remove(w);
                break;
            case EnemySkeleton s:
                if (enemies.Contains(s))
                {
                    enemies.Remove(s);
                }
                break;
            case EnemyWarrior w:
                if (enemies.Contains(w))
                {
                    enemies.Remove(w);
                }
                break;
            default:
                warriors.Remove(u);
                break;
        }
        if (allUnits.Contains(u))
        {
            allUnits.Remove(u);
        }

        if (u.IsObjective)
        {
            Debug.Log("Renewing");
            RenewObjective(u);
        }
    }

    private void RenewObjective(Unit u)
    {
        Debug.Log(u);
        switch (u)
        {
            case EnemySkeleton s:
                SettingsManager.Instance.UIManager.RenewObjectiveProgress("SkeletonBoss", 1);
                Debug.Log("Renewing skel");
                break;
            case EnemyWarrior w:
                SettingsManager.Instance.UIManager.RenewObjectiveProgress("ForestBandit", 1);
                Debug.Log("Renewing fb");
                break;
        }
    }

    #endregion

    #region HELPER METHODS

    public Unit GetRandomTarget()
    {
        List<Unit> units = allUnits.Where(x => x.IsEnemy == false).ToList();
        System.Random rand = new System.Random();
        if (units.Count > 0)
        {
            return units[rand.Next(0, units.Count - 1)];
        }
        else
        {
            return null;
        }
    }

    #endregion
}
