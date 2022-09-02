// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuDialog : Dialog
{
    [SerializeField] private Button hordeModeButton;


    protected override void OnShow()
    {
        if (Network.sharedInstance.IsAuthenticated())
            hordeModeButton.interactable = true;
        else
            hordeModeButton.interactable = false;
    }

    public void OnEndlessButtonClicked()
    {
        Hide();
        Game.sharedInstance.StartEndlessMode();
    }

    public void OnHordeModeButtonClicked()
    {
        Hide();
        DialogManager.sharedInstance.ShowLevelSelectDialog();
    }

    public void OnBrainCloudButtonClicked()
    {
        DialogManager.sharedInstance.ShowBrainCloudDialog();
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
