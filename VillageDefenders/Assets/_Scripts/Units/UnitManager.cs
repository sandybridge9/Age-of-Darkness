using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private List<Unit> allUnits;

    private List<Unit> workers;
    private List<Unit> warriors;

    public void AddUnit(Unit unit)
    {
        allUnits.Add(unit);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
