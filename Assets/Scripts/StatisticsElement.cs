// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class StatisticsElement : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text value;

    public void Reset()
    {
        this.title.text = "";
        this.value.text = "";
    }
}
