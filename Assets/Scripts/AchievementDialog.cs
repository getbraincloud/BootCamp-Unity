// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDialog : Dialog
{
    [SerializeField] private AchievementInfo[] achievementInfos;

    protected override void OnShow()
    {
        Achievement achievement;

        foreach (AchievementInfo ai in achievementInfos)
            ai.Reset();

        for (int i = 0; i < AchievementManager.sharedInstance.GetCount(); i++)
        {
            achievement = AchievementManager.sharedInstance.GetAchievementAtIndex(i);
            if (achievement != null && i < achievementInfos.Length)
                achievementInfos[i].Set(achievement);
        }
    }
}
