// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EmailLoginDialog : Dialog
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;


    public void OnLoginButtonClicked()
    {
        //TODO: Implement email authentication
    }

    public void OnAnonymousButtonClicked()
    {
        //TODO: Implement anonymous authentication
    }

    public void OnTwitchButtonClicked()
    {
        //TODO: Implement Twitch external authentication
    }

    protected override void OnClose()
    {
        // Dialog closed without logging in, show the main menu dialog
        DialogManager.sharedInstance.ShowMainMenuDialog();
    }
}
