// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ConnectingDialog : Dialog
{
    [SerializeField] private TMPro.TMP_Text dots;
    private int m_DotsCount;
    private float m_DotsTimer;

    protected override void OnShow()
    {
        m_DotsCount = 0;
        m_DotsTimer = Constants.kConnectingDialogDotsInterval;
    }

    // Update is called once per frame
    void Update()
    {
        m_DotsTimer -= Time.deltaTime;

        if (m_DotsTimer <= 0.0f)
        {
            m_DotsTimer += Constants.kConnectingDialogDotsInterval;

            m_DotsCount++;

            if (m_DotsCount >= 4)
                m_DotsCount = 0;

            string dotsText = "";
            for (int i = 0; i < m_DotsCount; i++)
                dotsText += ".";
            dots.text = dotsText;
        }
    }
}
