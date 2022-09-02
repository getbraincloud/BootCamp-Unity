// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseDialog : Dialog
{
    [SerializeField] private Button brainCloudButton;

    protected override void OnShow()
    {
        if (Network.sharedInstance.IsAuthenticated())
            brainCloudButton.interactable = true;
        else
            brainCloudButton.interactable = false;
    }

    public void OnResumeButtonClicked()
    {
        Hide();
    }

    public void OnBrainCloudButtonClicked()
    {
        DialogManager.sharedInstance.ShowBrainCloudDialog(true);
    }

    public void OnMainMenuButtonClicked()
    {
        Hide();
        DialogManager.sharedInstance.ShowMainMenuDialog();
        Game.sharedInstance.Reset(false);
    }
}
