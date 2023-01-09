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

    public void Set(LeaderboardEntry highScore)
    {
        this.rank.text = highScore.Rank.ToString() + ".";
        this.username.text = highScore.Nickname;
        this.time.text = TimeSpan.FromSeconds(highScore.Time).ToString(@"mm\:ss");

        if(highScore.IsUserScore)
        {
            Color green = new Color32(207, 198, 0, 255);
            this.rank.color = green;
            this.username.color = green;
            this.time.color = green;
        }
        else
        {
            Color white = new Color32(255, 255, 255, 255);
            this.rank.color = white;
            this.username.color = white;
            this.time.color = white;
        }
    }
}
