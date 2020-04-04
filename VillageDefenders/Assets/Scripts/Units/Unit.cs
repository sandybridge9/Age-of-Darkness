using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float Health;
    [HideInInspector]
    public bool IsSelected { get; set; } = false;

    public Unit()
    {
        Health = 100f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSelected)
        {
            if (Input.GetKey(KeyCode.Delete))
            {
                Delete();
            }
        }
    }

    public void Move()
    {

    }

    public void Delete()
    {
        //Some previous logic for removing this unit from lists, etc.
        SettingsManager.Instance.SelectionManager.RemoveGameObjectFromSelection(this.gameObject);
        Destroy();
    }

    public void Destroy()
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
