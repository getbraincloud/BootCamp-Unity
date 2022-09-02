// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UniversalLoginDialog : Dialog
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;

    public void OnLoginButtonClicked()
    {
        //TODO: Implement Universal authentication
    }

    protected override void OnClose()
    {
        // Dialog closed without logging in, show the main menu dialog
        DialogManager.sharedInstance.ShowMainMenuDialog();
    }
}
