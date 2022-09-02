// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AchievementInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text status;

    public void Reset()
    {
        this.description.text = "";
        this.status.text = "";
    }
}


