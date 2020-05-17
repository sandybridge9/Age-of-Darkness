using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySkeleton : Unit
{
    #region PROPERTIES

    public float AttackRange = 1.5f;
    public float Damage = 10f;

    #endregion

    #region FIELDS

    private float attackDelay = 60f;
    private bool isCurrentlyInCombat = false;

    private Unit enemyToAttack;
    private Vector3 lastKnownEnemyLocation;

    #endregion

    #region CONSTRUCTORS

    public EnemySkeleton()
    {
        Health = 75f;
        Cost = new ResourceBundle(0, 10, 0, 15, 30);
        CurrentUnitState = UnitState.Idle;
        CombatMode = UnitState.Aggressive;
    }

    #endregion

    #region OVERRIDEN METHODS

    protected override void SelectedUnitSpecificOrdersOnUpdate()
    {

    }

    protected override void UnitSpecificOrdersOnUpdate()
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
                switch (CombatMode)
                {
                    case UnitState.Aggressive:
                        SearchForEnemy();
                        break;
                    case UnitState.Defensive:
                        //Defend target
                        break;
                    case UnitState.StandGround:
                        //Do nothing
                        break;
                }
                break;
        }
        //CheckIfBeingAttacked();
        ResetEnemyOnDeath();
    }

    #endregion

    #region COMBAT

    public void AttackEnemy()
    {
        //Get nearest enemy unit
        Unit target = SettingsManager.Instance.UnitManager.GetRandomTarget();
        if (target != null)
        {
            enemyToAttack = target;
            MoveToEnemyOrder();
        }
    }

    private void MoveToEnemyOrder()
    {
        if (enemyToAttack != null)
        {
            agent.ResetPath();
            lastKnownEnemyLocation = enemyToAttack.transform.position;
            isCurrentlyInCombat = true;
            CurrentUnitState = UnitState.MovingToAttack;
            var correctedPosition =
                SettingsManager.Instance.SelectionManager.GetPositionForUnit(this, enemyToAttack.transform.position);
            //agent.SetDestination(enemyToAttack.transform.position);
            agent.SetDestination(correctedPosition);
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
            Debug.Log("I am attacking the enemy");
            //TODO add some sort of detection/vision range
            lastKnownEnemyLocation = enemyToAttack.transform.position;
            agent.SetDestination(lastKnownEnemyLocation);
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
            ResetAttackAnimations();
        }
        attackDelay++;
    }

    //Checks if enemy is still in range, and if not, sets a MoveToEnemyOrder()
    private bool CheckIfEnemyIsInRange()
    {
        if ((enemyToAttack.transform.position - transform.position).sqrMagnitude < AttackRange * AttackRange)
        {
            Debug.Log("enemy is in range");
            PlayAttackAnimation();
            return true;
        }
        else
        {
            Debug.Log("enemy is not in range");
            ResetAttackAnimations();
            MoveToEnemyOrder();
            return false;
        }
    }

    private void RotateToFaceEnemy()
    {
        Debug.Log("Trying to rotate");
        var direction = (enemyToAttack.transform.position - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(direction);
        if (Quaternion.Angle(lookRotation, transform.rotation) > 15f)
        {
            Debug.Log("Rotating");
            character.Move(direction, false, false);
        }
    }

    private void ResetEnemyOnDeath()
    {
        if (enemyToAttack == null || enemyToAttack.IsDead)
        {
            enemyToAttack = null;
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

    #endregion

    #region COMBAT MODES

    //Aggressive
    private void SearchForEnemy()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRange).ToList();
        foreach (var c in colliders)
        {
            Unit u = c.GetComponent<Unit>();
            if (u != null && CheckIfUnitIsEnemy(u))
            {
                enemyToAttack = u;
                MoveToEnemyOrder();
            }
        }
    }

    private bool CheckIfUnitIsEnemy(Unit unit)
    {
        return IsEnemy != unit.IsEnemy;
    }

    #endregion
}
