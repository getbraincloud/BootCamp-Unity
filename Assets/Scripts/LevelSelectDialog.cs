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

    protected override void OnShow()
    {
        levelTwoButton.interactable = Game.sharedInstance.GetUserData().LevelOneCompleted;
        levelThreeButton.interactable = Game.sharedInstance.GetUserData().LevelTwoCompleted;
        levelBossButton.interactable = Game.sharedInstance.GetUserData().LevelThreeCompleted;
    }

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
