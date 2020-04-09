using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState : short
{
    Idle = 1,
    Moving = 2,
    MovingToResource = 3,
    Gathering = 4,
    MovingToUnload = 5,
    Unloading = 6,
    Attacking = 7,
}
