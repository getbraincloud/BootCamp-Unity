// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AttachEmailDialog : Dialog
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;

    private Network.AttachEmailIdentityCompleted m_AttachEmailIdentityCompleted;
    private Network.AttachEmailIdentityFailed m_AttachEmailIdentityFailed;

    public void Set(Network.AttachEmailIdentityCompleted attachEmailIdentityCompleted, Network.AttachEmailIdentityFailed attachEmailIdentityFailed)
    {
        m_AttachEmailIdentityCompleted = attachEmailIdentityCompleted;
        m_AttachEmailIdentityFailed = attachEmailIdentityFailed;
    }

    public void OnAttachButtonClicked()
    {
        Hide();

        DialogManager.sharedInstance.ShowConnectingDialog();

        Network.sharedInstance.AttachEmailIdentity(emailField.text, passwordField.text, m_AttachEmailIdentityCompleted, m_AttachEmailIdentityFailed);
    }

    protected override void OnClose()
    {
        Hide();
    }
}
