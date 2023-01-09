// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InGameBrainCloudDialog : Dialog
{
    public void OnSetUsernameButtonClicked()
    {
        DialogManager.sharedInstance.ShowChangeUsernameDialog();
    }

    public void OnLeaderboardsButtonClicked()
    {
        DialogManager.sharedInstance.ShowLeaderboardsDialog();
    }

    public void OnStatisticsButtonClicked()
    {
        DialogManager.sharedInstance.ShowStatisticsDialog();
    }

    public void OnAchievementsButtonClicked()
    {
        DialogManager.sharedInstance.ShowAchievementDialog();
    }
}
