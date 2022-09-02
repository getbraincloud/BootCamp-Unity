// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectDialog : Dialog
{
    [SerializeField] private Button levelTwoButton;
    [SerializeField] private Button levelThreeButton;
    [SerializeField] private Button levelBossButton;

    public void OnLevelOneButtonClicked()
    {
        Hide();
        Game.sharedInstance.StartHordeMode(Constants.kHordeModeLevelOne);
    }

    public void OnLevelTwoButtonClicked()
    {
        Hide();
        Game.sharedInstance.StartHordeMode(Constants.kHordeModeLevelTwo);
    }

    public void OnLevelThreeButtonClicked()
    {
        Hide();
        Game.sharedInstance.StartHordeMode(Constants.kHordeModeLevelThree);
    }

    public void OnLevelBossButtonClicked()
    {
        Hide();
        Game.sharedInstance.StartHordeMode(Constants.kHordeModeLevelBoss);
    }

    public void OnMainMenuButtonClicked()
    {
        Hide();
        DialogManager.sharedInstance.ShowMainMenuDialog();
    }
}
