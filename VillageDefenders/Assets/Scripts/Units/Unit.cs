using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class Unit : MonoBehaviour
{
    public float Health;
    public ResourceBundle Cost;
    //[HideInInspector]
    public bool IsSelected = false;
    public bool IsEnemy = false;

    public UnitState CurrentUnitState;
    protected NavMeshAgent agent;
    protected ThirdPersonCharacter character;
    protected Animator animator;

    public Unit()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 0, 0, 15);
        CurrentUnitState = UnitState.Idle;
    }

    #region Start

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        UnitSpecificStartup();
    }

    //Method that can be overriden when some unit specific actions are needed on start
    protected virtual void UnitSpecificStartup()
    {

    }

    #endregion

    #region Update

    void Update()
    {
        //Unit's health reached 0, it is now dead
        if (IsDead())
        {
            UnitSpecificDeathActions();
        }
        if (IsSelected)
        {
            //All units can be deleted
            DeleteOrder();
            //All units can have specific orders when selected
            SelectedUnitSpecificOrders();
        }
        else
        {
            //All units can have specific orders when deselected (patroling, gathering etc.)
            DeSelectedUnitSpecificOrders();
        }

        UnitSpecificOrders();
    }

    public bool IsDead()
    {
        if (Health <= 0)
        {
            return true;
        }
        return false;
    }

    //Method which can be overriden when some unit specific actions are needed on update (when unit is selected (moving, manual unloading etc.))
    protected virtual void SelectedUnitSpecificOrders()
    {
        MoveOrder();
    }

    //Method which can be overriden when some unit specific actions are needed on update (when unit is DeSelected)
    protected virtual void DeSelectedUnitSpecificOrders()
    {

    }

    //Method which can be overriden when some unit specific actions are needed on update (when unit is not necessarily selected (automatic gathering, attacking etc.)
    protected virtual void UnitSpecificOrders()
    {
        switch (CurrentUnitState)
        {
            case UnitState.Moving:
                Move();
                break;
            case UnitState.Idle:
                character.Move(Vector3.zero, false, false);
                break;
            default:
                break;
        }
    }

    //Method which can be overriden in derived classes when some specific actions need to happen on unit's death - drop loot, play animation etc.
    protected virtual void UnitSpecificDeathActions()
    {
        //TODO Add some death actions
        Delete();
    }

    private void DeleteOrder()
    {
        if (Input.GetKey(KeyCode.Delete))
        {
            Delete();
        }
    }

    protected void MoveOrder()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                //agent.ResetPath();
                CurrentUnitState = UnitState.Moving;
                agent.SetDestination(hitInfo.point);
                Move();
            }
        }
    }

    #endregion

    #region Other Methods

    protected virtual void Move()
    {
        CheckIfArrivedAtDestination();
        if (CurrentUnitState == UnitState.Moving)
        {
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
    }

    private void CheckIfArrivedAtDestination()
    {
        if (Vector3.Distance(agent.destination, transform.position) < 0.1)
        {
            Debug.Log("I have arrived at my destination and I am now idle.");
            agent.ResetPath();
            CurrentUnitState = UnitState.Idle;
            character.Move(Vector3.zero, false, false);
        }
    }

    public void Select()
    {
        IsSelected = true;
    }

    public void DeSelect()
    {
        IsSelected = false;
    }
    

    public void Delete()
    {
        //Some previous logic for removing this unit from lists, etc.
        SettingsManager.Instance.SelectionManager.RemoveGameObjectFromSelection(this.gameObject);
        Destroy();
    }

    private void Destroy()
    {
        Object.Destroy(this.gameObject);
    }

    #endregion
}
