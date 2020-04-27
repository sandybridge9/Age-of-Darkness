using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Unit
{
    public float AttackRange = 1.5f;
    public float Damage = 10f;
    public UnitState CombatMode = UnitState.StandGround;
    private float attackDelay = 60f;
    private bool isCurrentlyInCombat = false;

    private Unit enemyToAttack;
    private Vector3 lastKnownEnemyLocation;

    private GameObject shield;
    private GameObject sword;
    private GameObject swordSide;

    public Warrior()
    {
        Health = 150f;
        Cost = new ResourceBundle(0, 0, 0, 0, 15);
        CurrentUnitState = UnitState.Idle;
    }

    protected override void UnitSpecificStartup()
    {
        base.UnitSpecificStartup();

        shield = GameObject.Find("Shield");
        sword = GameObject.Find("Sword");
        swordSide = GameObject.Find("SwordSide");

        shield.SetActive(true);
        sword.SetActive(false);
        swordSide.SetActive(true);
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
                Move();
                break;
            case UnitState.MovingToAttack:
                MoveToEnemy();
                break;
            case UnitState.Attacking:
                Attack();
                break;
            case UnitState.Idle:
                character.Move(Vector3.zero, false, false);
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
            if (CheckIfHitEnemyUnit(hitUnit) && hitUnit != this)
            {
                Debug.Log("Got orders to move to attack. ");
                enemyToAttack = hitUnit;
                MoveToEnemyOrder();
            }
            else
            {
                Debug.Log("Can't attack friendly units. ");
                agent.ResetPath();
                SheatheWeapon();
            }
        }
        else
        {
            Debug.Log("Got orders to move to a location. ");
            SheatheWeapon();
            MoveOrder();
        }
        ResetAttackAnimations();
    }

    private bool CheckIfHitEnemyUnit(Unit unit)
    {
        //This unit can have IsEnemy flag too
        if ((IsEnemy && !unit.IsEnemy) || (!IsEnemy && unit.IsEnemy))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MoveToEnemyOrder()
    {
        if (enemyToAttack != null)
        {
            agent.ResetPath();
            lastKnownEnemyLocation = enemyToAttack.transform.position;
            isCurrentlyInCombat = true;
            UnsheatheWeapon();
            CurrentUnitState = UnitState.MovingToAttack;
            agent.SetDestination(enemyToAttack.transform.position);
        }
        else
        {
            CurrentUnitState = UnitState.Idle;
        }
    }

    private void MoveToEnemy()
    {
        if (enemyToAttack != null && !enemyToAttack.IsDead)
        {
            //TODO add some sort of detection/vision range
            lastKnownEnemyLocation = enemyToAttack.transform.position;
            CheckIfArrivedAtEnemyLocation();
            if (CurrentUnitState == UnitState.MovingToAttack)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }
        else
        {
            CurrentUnitState = UnitState.Idle;
        }
    }

    private void CheckIfArrivedAtEnemyLocation()
    {
        if ((lastKnownEnemyLocation - transform.position).sqrMagnitude < AttackRange * AttackRange)
        {
            Debug.Log("I have arrived at enemy location, and I should now commence the attack");
            agent.ResetPath();
            CurrentUnitState = UnitState.Attacking;
            character.Move(Vector3.zero, false, false);
        }
    }

    protected virtual void Attack()
    {
        //Check if enemy has been killed
        if (enemyToAttack != null && !enemyToAttack.IsDead)
        {
            if (CheckIfEnemyIsInRange())
            {
                RotateToFaceEnemy();
                //Check if CurrentUnitState is Attacking, because it can be changed to Rotating
                if (attackDelay >= 60f && CurrentUnitState == UnitState.Attacking)
                {
                    enemyToAttack.Health -= Damage;
                    attackDelay = 0;
                }
            }
        }
        else
        {
            //TODO find closest enemy to attack if aggresive, else idle
            CurrentUnitState = UnitState.Idle;
            SheatheWeapon();
            ResetAttackAnimations();
        }
        attackDelay++;
    }

    //Checks if enemy is still in range, and if not, sets a MoveToEnemyOrder()
    private bool CheckIfEnemyIsInRange()
    {
        if ((enemyToAttack.transform.position - transform.position).sqrMagnitude < AttackRange * AttackRange)
        {
            PlayAttackAnimation();
            return true;
        }
        else
        {
            ResetAttackAnimations();
            MoveToEnemyOrder();
            return false;
        }
    }

    private void RotateToFaceEnemy()
    {
        var direction = (enemyToAttack.transform.position - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(direction);
        if (Quaternion.Angle(lookRotation, transform.rotation) > 15f)
        {
            Debug.Log("Rotating");
            character.Move(direction, false, false);
        }
        else
        {
            CurrentUnitState = UnitState.Attacking;
            character.Move(Vector3.zero, false, false);
        }
    }

    private void PlayAttackAnimation()
    {
        animator.SetBool("Attacking", true);
    }

    private void ResetAttackAnimations()
    {
        animator.SetBool("Attacking", false);
    }

    private void UnsheatheWeapon()
    {
        swordSide.SetActive(false);
        sword.SetActive(true);
    }

    private void SheatheWeapon()
    {
        swordSide.SetActive(true);
        sword.SetActive(false);
    }
}