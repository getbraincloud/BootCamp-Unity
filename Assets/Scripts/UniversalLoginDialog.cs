// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UniversalLoginDialog : Dialog
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;

    private Network.AuthenticationRequestCompleted m_AuthenticationRequestCompleted;
    private Network.AuthenticationRequestFailed m_AuthenticationRequestFailed;

    public void Set(Network.AuthenticationRequestCompleted authenticationRequestCompleted, Network.AuthenticationRequestFailed authenticationRequestFailed)
    {
        m_AuthenticationRequestCompleted = authenticationRequestCompleted;
        m_AuthenticationRequestFailed = authenticationRequestFailed;
    }

    public void OnLoginButtonClicked()
    {
        Hide();
        DialogManager.sharedInstance.ShowConnectingDialog();
        Network.sharedInstance.RequestAuthenticationUniversal(usernameField.text, passwordField.text, m_AuthenticationRequestCompleted, m_AuthenticationRequestFailed);
    }

    protected override void OnClose()
    {
        // Dialog closed without logging in, show the main menu dialog
        DialogManager.sharedInstance.ShowMainMenuDialog();
    }
}
