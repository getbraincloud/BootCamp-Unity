// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorDialog : Dialog
{
    [SerializeField] private TMPro.TMP_Text errorMessage;

    public void Set(string message)
    {
        errorMessage.text = message;
    }

    protected override void OnClose()
    {
        // Dialog closed without logging in, show the main menu dialog
        DialogManager.sharedInstance.ShowMainMenuDialog();
    }
}
