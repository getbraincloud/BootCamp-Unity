// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsDialog : Dialog
{
    [SerializeField] private StatisticsElement[] statisticsElements;

    protected override void OnShow()
    {
        Statistic statistic;

        foreach (StatisticsElement se in statisticsElements)
            se.Reset();

        for (int i = 0; i < StatisticsManager.sharedInstance.GetCount(); i++)
        {
            statistic = StatisticsManager.sharedInstance.GetStatisticAtIndex(i);
            if (statistic != null && i < statisticsElements.Length)
                statisticsElements[i].Set(statistic);
        }
    }
}
