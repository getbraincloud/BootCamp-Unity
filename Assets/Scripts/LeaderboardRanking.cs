// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LeaderboardRanking : MonoBehaviour
{
    [SerializeField] private TMP_Text rank;
    [SerializeField] private TMP_Text username;
    [SerializeField] private TMP_Text time;

    public void Reset()
    {
        this.rank.text = "";
        this.username.text = "";
        this.time.text = "";
    }
}
