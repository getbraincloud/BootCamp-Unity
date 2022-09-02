// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HighScoreDialog : Dialog
{
    [SerializeField] private HighScoreRanking[] highScoreRankings;
    [SerializeField] private Button leftSegmentButton;
    [SerializeField] private Button middleSegmentButton;
    [SerializeField] private Button rightSegmentButton;

    private enum SegmentControlState
    {
        Main = 0,
        Daily,
        Country
    };

    protected override void OnShow()
    {
        SetSegmentControlState(SegmentControlState.Main);
        SetLeaderboardData(Constants.kBrainCloudMainHighScoreID);
    }

    public void OnMainScoresClicked()
    {
        SetSegmentControlState(SegmentControlState.Main);
        SetLeaderboardData(Constants.kBrainCloudMainHighScoreID);
    }

    public void OnDailyScoresClicked()
    {
        SetSegmentControlState(SegmentControlState.Daily);
        SetLeaderboardData(Constants.kBrainCloudDailyHighScoreID);
    }

    public void OnCountryScoresClicked()
    {
        SetSegmentControlState(SegmentControlState.Country);
        SetLeaderboardData(Constants.kBrainCloudCountryHighScoreID);
    }

    private void SetSegmentControlState(SegmentControlState segmentControlState)
    {
        string leftTexture = GetTextureFile(SegmentControlState.Main, segmentControlState == SegmentControlState.Main);
        string middleTexture = GetTextureFile(SegmentControlState.Daily, segmentControlState == SegmentControlState.Daily);
        string rightTexture = GetTextureFile(SegmentControlState.Country, segmentControlState == SegmentControlState.Country);

        leftSegmentButton.image.sprite = Resources.Load<Sprite>(leftTexture);
        middleSegmentButton.image.sprite = Resources.Load<Sprite>(middleTexture);
        rightSegmentButton.image.sprite = Resources.Load<Sprite>(rightTexture);

        Color32 selectedColor = new Color32(255, 255, 255, 255);
        Color32 unselectedColor = new Color32(255, 255, 255, 255);

        leftSegmentButton.GetComponentInChildren<TMPro.TMP_Text>().color = segmentControlState == SegmentControlState.Main ? selectedColor : unselectedColor;
        middleSegmentButton.GetComponentInChildren<TMPro.TMP_Text>().color = segmentControlState == SegmentControlState.Daily ? selectedColor : unselectedColor;
        rightSegmentButton.GetComponentInChildren<TMPro.TMP_Text>().color = segmentControlState == SegmentControlState.Country ? selectedColor : unselectedColor;
    }

    private void SetLeaderboardData(string leaderboardId)
    {
        HighScore hs;

        ResetLeaderboardData();

        Leaderboard leaderboard = HighScoreManager.sharedInstance.GetLeaderboardByName(leaderboardId);

        if (leaderboard != null)
        {
            for (int i = 0; i < leaderboard.GetCount(); i++)
            {
                hs = leaderboard.GetHighScoreAtIndex(i);
                if (hs != null)
                    highScoreRankings[i].Set(hs);
            }
        }
    }

    private void ResetLeaderboardData()
    {
        foreach (HighScoreRanking hsr in highScoreRankings)
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
                texture += "Middle-";
                break;
            case SegmentControlState.Country:
                texture += "Right-";
                break;
        }

        texture += isSelected ? "Selected" : "Unselected";
        return texture;
    }
}