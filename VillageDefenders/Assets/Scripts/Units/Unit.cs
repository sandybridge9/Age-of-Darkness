using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public float Health;
    public ResourceBundle Cost;
    protected NavMeshAgent agent;
    [HideInInspector]
    public bool IsSelected { get; set; } = false;
    public bool IsEnemy = false;
    
    public UnitState CurrentUnitState;

    public Unit()
    {
        Health = 100f;
        Cost = new ResourceBundle(0,0,0,0,15);
        CurrentUnitState = UnitState.Idle;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        UnitSpecificStartup();
    }

    // Update is called once per frame
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
        //All units can have specific orders when unselected - actions that are completed automatically without user input
        UnitSpecificOrders();
    }

    //Method that can be overriden when some unit specific actions are needed on start
    protected virtual void UnitSpecificStartup()
    {

    }

    //Method which can be overriden when some unit specific actions are needed on update (when unit is selected (moving, manual unloading etc.))
    protected virtual void SelectedUnitSpecificOrders()
    {
        MoveOrder();
    }

    //Method which can be overriden when some unit specific actions are needed on update (when unit is not necessarily selected (automatic gathering, attacking etc.)
    protected virtual void UnitSpecificOrders()
    {

    }

    //Method which can be overriden in derived classes when some specific actions need to happen on unit's death - drop loot, play animation etc.
    protected virtual void UnitSpecificDeathActions()
    {
        Delete();
    }

    private void MoveOrder()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Move();
        }
    }

    protected void Move()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            agent.ResetPath();
            CurrentUnitState = UnitState.Moving;
            agent.SetDestination(hitInfo.point);
        }
    }

    public bool IsDead()
    {
        if (Health <= 0)
        {
            return true;
        }
        return false;
    }

    private void DeleteOrder()
    {
        if (Input.GetKey(KeyCode.Delete))
        {
            Delete();
        }
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

    public void Select()
    {
        IsSelected = true;
    }

    public void DeSelect()
    {
        IsSelected = false;
    }
}
