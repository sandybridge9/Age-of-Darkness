﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform Player;

    private void LateUpdate()
    {
        Vector3 newPosition = Player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
