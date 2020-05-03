using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class Unit : MonoBehaviour
{
    #region Properties

    public float Health;
    public ResourceBundle Cost;
    public bool IsSelected = false;
    public bool IsEnemy = false;
    public bool IsDead = false;
    public bool IsObjective = false;

    public UnitState CurrentUnitState;
    public UnitState CombatMode;

    #endregion

    #region Fields

    protected float sightRange = 25f;
    protected NavMeshAgent agent;
    protected ThirdPersonCharacter character;
    protected Animator animator;
    protected GameObject selectionMarker;
    private bool removedOnDeath = false;

    #endregion

    #region Constructors

    public Unit()
    {
        Health = 100f;
        Cost = new ResourceBundle(0, 0, 0, 0, 15);
        CurrentUnitState = UnitState.Idle;
        CombatMode = UnitState.StandGround;
    }

    #endregion

    #region Start

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        selectionMarker = transform.Find("SelectionMarker").gameObject;
        selectionMarker.SetActive(false);
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
        if (HasDied())
        {
            UnitSpecificDeathActions();
        }
        if (IsSelected)
        {
            //All units can be deleted
            DeleteOrder();
            //All units can have specific orders when selected
            SelectedUnitSpecificOrdersOnUpdate();
        }
        else
        {
            //All units can have specific orders when deselected (patroling, gathering etc.)
            DeSelectedUnitSpecificOrdersOnUpdate();
        }

        UnitSpecificOrdersOnUpdate();
    }

    public bool HasDied()
    {
        if (Health <= 0)
        {
            IsDead = true;
            return true;
        }
        return false;
    }

    //Method which can be overriden when some unit specific actions are needed on update (when unit is selected (moving, manual unloading etc.))
    protected virtual void SelectedUnitSpecificOrdersOnUpdate()
    {
        MoveOrder();
    }

    //Method which can be overriden when some unit specific actions are needed on update (when unit is DeSelected)
    protected virtual void DeSelectedUnitSpecificOrdersOnUpdate()
    {

    }

    //Method which can be overriden when some unit specific actions are needed on update (when unit is not necessarily selected (automatic gathering, attacking etc.)
    protected virtual void UnitSpecificOrdersOnUpdate()
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
        CurrentUnitState = UnitState.Idle;
        animator.SetBool("DeathTrigger", true);
        Delete(10f);
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
                Vector3 correctedPosition = SettingsManager.Instance.SelectionManager.GetPositionForUnit(this, hitInfo.point);
                //agent.ResetPath();
                CurrentUnitState = UnitState.Moving;
                //agent.SetDestination(hitInfo.point);
                agent.SetDestination(correctedPosition);
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
        selectionMarker.SetActive(true);
    }

    public void DeSelect()
    {
        IsSelected = false;
        selectionMarker.SetActive(false);
        Debug.Log("Deselecting");
    }
    

    public void Delete(float deathTimer = 0f)
    {
        //Some previous logic for removing this unit from lists, etc.
        if (!removedOnDeath)
        {
            SettingsManager.Instance.SelectionManager.RemoveUnitFromSelection(this);
            SettingsManager.Instance.UnitManager.RemoveUnitFromLists(this);
            removedOnDeath = true;
        }
        Destroy(deathTimer);
    }

    private void Destroy(float deathTimer = 0f)
    {
        DeSelect();
        Object.Destroy(this.gameObject, deathTimer);
    }

    #endregion
}
