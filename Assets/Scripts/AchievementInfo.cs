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

    public void Set(Achievement achievement)
    {
        this.description.text = achievement.Description;
        this.status.text = achievement.GetStatusString();

        if (achievement.IsAwarded)
        {
            Color green = new Color32(207, 198, 0, 255);
            this.description.color = green;
            this.status.color = green;
        }
        else
        {
            Color white = new Color32(255, 255, 255, 255);
            this.description.color = white;
            this.status.color = white;
        }
    }
}


