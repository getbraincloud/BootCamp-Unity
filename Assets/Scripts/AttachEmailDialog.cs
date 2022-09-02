// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AttachEmailDialog : Dialog
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;

    public void OnAttachButtonClicked()
    {
        Hide();
    }

    protected override void OnClose()
    {
        Hide();
    }
}
