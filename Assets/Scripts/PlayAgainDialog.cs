// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAgainDialog : Dialog
{
    public void OnPlayAgainButtonClicked()
    {
        Game.sharedInstance.Reset(true);
        Hide();
    }

    public void OnBrainCloudButtonClicked()
    {
        DialogManager.sharedInstance.ShowBrainCloudDialog();
    }

    public void OnMainMenuButton()
    {
        Game.sharedInstance.Reset(false);
        Hide();
        DialogManager.sharedInstance.ShowMainMenuDialog();
    }
}
