// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LeaderboardDialog : Dialog
{
    [SerializeField] private LeaderboardRanking[] leaderboardRankings;
    [SerializeField] private Button leftSegmentButton;
    [SerializeField] private Button rightSegmentButton;

    private enum SegmentControlState
    {
        Main = 0,
        Daily
    };

    protected override void OnShow()
    {
        SetSegmentControlState(SegmentControlState.Main);

        ResetLeaderboardData(); //Remove

        //TODO: Implement Leaderboards
    }

    public void OnMainScoresClicked()
    {
        SetSegmentControlState(SegmentControlState.Main);

        //TODO: Multiple leaderboards
    }

    public void OnDailyScoresClicked()
    {
        SetSegmentControlState(SegmentControlState.Daily);

        //TODO: Multiple leaderboards
    }

    private void SetSegmentControlState(SegmentControlState segmentControlState)
    {
        string leftTexture = GetTextureFile(SegmentControlState.Main, segmentControlState == SegmentControlState.Main);
        string rightTexture = GetTextureFile(SegmentControlState.Daily, segmentControlState == SegmentControlState.Daily);

        leftSegmentButton.image.sprite = Resources.Load<Sprite>(leftTexture);
        rightSegmentButton.image.sprite = Resources.Load<Sprite>(rightTexture);

        Color32 selectedColor = new Color32(255, 255, 255, 255);
        Color32 unselectedColor = new Color32(255, 255, 255, 255);

        leftSegmentButton.GetComponentInChildren<TMPro.TMP_Text>().color = segmentControlState == SegmentControlState.Main ? selectedColor : unselectedColor;
        rightSegmentButton.GetComponentInChildren<TMPro.TMP_Text>().color = segmentControlState == SegmentControlState.Daily ? selectedColor : unselectedColor;
    }

    private void SetLeaderboardData(string leaderboardId)
    {
        //TODO: Implement Leaderboards
    }

    private void ResetLeaderboardData()
    {
        foreach (LeaderboardRanking hsr in leaderboardRankings)
            hsr.Reset();
    }

    private string GetTextureFile(SegmentControlState segmentControlState, bool isSelected)
    {
        string texture = "Textures/SegmentControl";

        switch (segmentControlState)
        {
            case SegmentControlState.Main:
                texture += "Left-";
                break;
            case SegmentControlState.Daily:
                texture += "Right-";
                break;
        }

        texture += isSelected ? "Selected" : "Unselected";
        return texture;
    }
}