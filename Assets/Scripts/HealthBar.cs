// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthBar : StatusBar
{
    [SerializeField] private GameObject ship;

    // Update is called once per frame
    void Update()
    {
        float pct = (float)ship.GetComponent<Ship>().GetHealth() / (float)Constants.kShipInitialHealth;
        UpdateBar(pct);
    }
}
