// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Network : MonoBehaviour
{
    public static Network sharedInstance;

    private void Awake()
    {
        sharedInstance = this;
    }

    void Update()
    {

    }

    public bool IsAuthenticated()
    {
        return false;
    }
}
