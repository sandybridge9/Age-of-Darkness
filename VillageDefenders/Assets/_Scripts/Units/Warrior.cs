using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Unit
{
    public float MeleeRange = 1.5f;
    public float Damage = 10f;
    private Unit enemyToAttack;
    private Vector3 lastKnownEnemyLocation;
    private float attackDelay = 60f;

    public Warrior()
    {
        Health = 150f;
        Cost = new ResourceBundle(0, 0, 0, 0, 15);
        CurrentUnitState = UnitState.Idle;
    }

    protected override void SelectedUnitSpecificOrders()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GiveOrder();
        }
    }

    protected override void UnitSpecificOrders()
    {
        switch (CurrentUnitState)
        {
            case UnitState.Moving:
                CheckIfArrivedAtDestination();
                break;
            case UnitState.MovingToAttack:
                CheckIfReachedEnemyLocation();
                break;
            case UnitState.Attacking:
                Attack();
                break;
            default:
                break;
        }
    }

    //Method that is responsible for processing mouse click worker commands (gather, unload, move).
    private void GiveOrder()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 1000f, SettingsManager.Instance.UnitLayerMask))
        {
            Unit hitUnit = hitInfo.transform.GetComponent<Unit>();
            //If hit unit is not THIS unit and hitUnit is an enemy, then move to attack
            if (CheckIfEnemy(hitUnit) && hitUnit != this)
            {
                enemyToAttack = hitUnit;
                MoveToEnemy();
                Debug.Log("Got orders to move to attack. ");
            }
            else
            {
                agent.ResetPath();
                Debug.Log("Can't attack friendly units. ");
            }
        }
        else
        {
            Debug.Log("Got orders to move to a location. ");
            Move();
        }
    }

    private void MoveToEnemy()
    {
        Debug.Log("Moving to enemy. ");
        lastKnownEnemyLocation = enemyToAttack.transform.position;
        CurrentUnitState = UnitState.MovingToAttack;
        agent.ResetPath();
        agent.SetDestination(lastKnownEnemyLocation);
    }

    private void CheckIfReachedEnemyLocation()
    {
        Debug.Log("Checking if arrived at enemy location. ");
        //If enemy changed location, reset movement to enemy
        if (lastKnownEnemyLocation != enemyToAttack.transform.position)
        {
            Debug.Log("Enemy location - changed. ");
            MoveToEnemy();
        }

        //If enemy is in melee range, attack
        if (Vector3.Distance(transform.position, enemyToAttack.transform.position) <= MeleeRange)
        {
            Debug.Log("Enemy location - reached. ");
            agent.ResetPath();
            CurrentUnitState = UnitState.Attacking;
        }
    }

    private bool CheckIfEnemy(Unit unit)
    {
        if ((IsEnemy && !unit.IsEnemy) || (!IsEnemy && unit.IsEnemy))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckIfEnemyIsInRange()
    {
        if (Vector3.Distance(transform.position, enemyToAttack.transform.position) > MeleeRange)
        {
            MoveToEnemy();
            return false;
        }
        return true;
    }

    private void Attack()
    {
        //Check if enemy has been killed or if enemy is still in range for an attack
        if (enemyToAttack != null && CheckIfEnemyIsInRange())
        {
            if (attackDelay >= 60f)
            {
                enemyToAttack.Health -= Damage;
                attackDelay = 0;
            }
        }
        attackDelay++;
    }

    //Check if worker has arrived at harvesting location (location doesn't have to be exact, because one cannot stand on top of resources)
    private void CheckIfArrivedAtDestination()
    {
        if (Vector3.Distance(agent.destination, transform.position) < 1)
        {
            Debug.Log("I have arrived at my destination and I am now idle.");
            agent.ResetPath();
            CurrentUnitState = UnitState.Idle;
        }
    }
}
