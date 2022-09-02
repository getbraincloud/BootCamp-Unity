// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PostScoreDialog : Dialog
{
    [SerializeField] private TMP_InputField inputField;

    private float m_Time;

    public void Set(float time)
    {
        m_Time = time;

        //TODO: Implement leaderboards
    }

    public void OnSubmitButtonClicked()
    {
        //TODO: Implement leaderboards

        Hide();
    }
}
