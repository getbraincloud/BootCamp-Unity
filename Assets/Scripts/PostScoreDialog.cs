// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PostScoreDialog : Dialog
{
    [SerializeField] private TMP_InputField inputField;

    private Network.PostScoreRequestCompleted m_PostScoreRequestCompleted;
    private Network.PostScoreRequestFailed m_PostScoreRequestFailed;
    private float m_Time;

    public void Set(float time, Network.PostScoreRequestCompleted postScoreRequestCompleted, Network.PostScoreRequestFailed postScoreRequestFailed)
    {
        m_Time = time;
        m_PostScoreRequestCompleted = postScoreRequestCompleted;
        m_PostScoreRequestFailed = postScoreRequestFailed;
    }

    public void OnSubmitButtonClicked()
    {
        Network.sharedInstance.PostScoreToLeaderboards(m_Time, inputField.text.ToString(), m_PostScoreRequestCompleted, m_PostScoreRequestFailed);
        Network.sharedInstance.RequestUpdateUsername(inputField.text.ToString());
        Hide();
    }
}
