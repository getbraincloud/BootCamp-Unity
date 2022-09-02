// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EmailLoginDialog : Dialog
{
    [SerializeField] private TMP_InputField emailField;
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
        Network.sharedInstance.RequestAuthenticationEmail(emailField.text, passwordField.text, m_AuthenticationRequestCompleted, m_AuthenticationRequestFailed);
    }

    public void OnAnonymousButtonClicked()
    {
        Hide();
        DialogManager.sharedInstance.ShowConnectingDialog();
        Network.sharedInstance.RequestAnonymousAuthentication(m_AuthenticationRequestCompleted, m_AuthenticationRequestFailed);
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
