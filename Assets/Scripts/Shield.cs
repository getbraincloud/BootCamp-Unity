// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : BaseObject
{
    private void Awake()
    {
        SetHealth(Constants.kShipInitialShieldHealth);
    }
}