using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCamp : MonoBehaviour
{
    #region FIELDS

    private float currentTime = 0f;

    #endregion

    #region PROPERTIES

    public Transform SpawnPoint;
    public Unit TroopToSpawn;
    public float TimeBetweenWaves = 6000f;
    public int CurrentWave = 0;
    public int EnemyCountToSpawn = 2;

    #endregion

    #region UNITY METHODS

    private void FixedUpdate()
    {
        currentTime++;
    }

    private void Update()
    {
        if (currentTime >= TimeBetweenWaves)
        {
            currentTime = 0f;
            SpawnWave();
        }
    }

    #endregion

    #region METHODS

    private void SpawnWave()
    {
        for (int i = 0; i < EnemyCountToSpawn; i++)
        {
            var unit = GameObject.Instantiate(TroopToSpawn, SpawnPoint);
            switch (unit)
            {
                case EnemyWarrior w:
                    OrderEnemyWarriorToAttack(w);
                    break;
                case EnemySkeleton s:
                    OrderEnemySkeletonToAttack(s);
                    break;
            }
        }
        CurrentWave++;
    }

    private void OrderEnemyWarriorToAttack(EnemyWarrior warrior)
    {
        warrior.AttackEnemy();
    }

    private void OrderEnemySkeletonToAttack(EnemySkeleton skeleton)
    {
        skeleton.AttackEnemy();
    }

    #endregion
}
