// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    [SerializeField] private Image dangerFillBar;
    private float m_DangerAlpha;
    private float m_DangerRadians;


    protected void UpdateBar(float pct)
    {
        m_DangerRadians += Constants.kHudDangerFlashIncrement * Time.deltaTime;

        if (m_DangerRadians >= Mathf.PI * 2.0f)
            m_DangerRadians -= Mathf.PI * 2.0f;

        m_DangerAlpha = Mathf.Abs(Mathf.Sin(m_DangerRadians));

        // If the pct is only a quarter or less, change the bar to red (danger)
        if (pct <= 0.25f)
        {
            dangerFillBar.color = new Color(1.0f, 1.0f, 1.0f, m_DangerAlpha);
            fillBar.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
        else
        {
            dangerFillBar.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            fillBar.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        // Scale the framesize to match the pct left
        fillBar.transform.localScale = new Vector3(pct, 1.0f, 1.0f);
        dangerFillBar.transform.localScale = new Vector3(pct, 1.0f, 1.0f);

    }
}
