// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeUsernameDialog : Dialog
{
    [SerializeField] private TMP_InputField inputField;

    public void OnChangeButtonClicked()
    {
        Network.sharedInstance.RequestUpdateUsername(inputField.text.ToString());
        Hide();
    }
}
