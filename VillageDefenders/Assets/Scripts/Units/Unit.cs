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

    public Unit()
    {
        Health = 100f;
        Cost = new ResourceBundle(0,0,0,0,15);
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
        if (IsSelected)
        {
            DeleteOrder();
            UnitSpecificOrders();
        }
    }

    //Method that can be overriden when some unit specific actions are needed on start
    protected virtual void UnitSpecificStartup()
    {

    }

    //Method which can be overriden when some unit specific actions are needed on update
    protected virtual void UnitSpecificOrders()
    {
        MoveOrder();
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
            agent.SetDestination(hitInfo.point);
        }
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
