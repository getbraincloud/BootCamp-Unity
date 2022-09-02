// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplayScrollingElement : MonoBehaviour
{
    public delegate void HeadsUpDisplayScrollingElementHoldCompleted(HeadsUpDisplayScrollingElement headsUpDisplayScrollingElement);

    [SerializeField] private Text output;

    private Slider m_Slider;
    private float m_DisplayMin;
    private HeadsUpDisplayScrollingElementHoldCompleted m_HeadsUpDisplayScrollingElementHoldCompleted;


    private void Awake()
    {
        m_Slider = new Slider();
    }

    void Update()
    {
        m_Slider.Update();
        gameObject.transform.localPosition = m_Slider.Current;

        if (m_DisplayMin > 0.0f)
        {
            m_DisplayMin -= Time.deltaTime;
            if (m_DisplayMin < 0.0f)
            {
                m_DisplayMin = 0.0f;

                if (m_HeadsUpDisplayScrollingElementHoldCompleted != null)
                    m_HeadsUpDisplayScrollingElementHoldCompleted(this);
            }
        }
    }

    public void Init(LeaderboardEntry leaderboardEntry, Vector2 position, HeadsUpDisplayScrollingElementHoldCompleted headsUpDisplayScrollingElement = null)
    {
        Init("High Score #" + leaderboardEntry.Rank.ToString() + ": " + TimeSpan.FromSeconds(leaderboardEntry.Time).ToString(@"mm\:ss"), position, headsUpDisplayScrollingElement);
    }

    public void Init(string overrideText, Vector2 position, HeadsUpDisplayScrollingElementHoldCompleted headsUpDisplayScrollingElement = null)
    {
        Init(overrideText, position, 0.0f, headsUpDisplayScrollingElement);
    }

    public void Init(string overrideText, Vector2 position, float displayMin, HeadsUpDisplayScrollingElementHoldCompleted headsUpDisplayScrollingElement = null)
    {
        gameObject.SetActive(false);

        output.text = overrideText;
        transform.localPosition = position;

        m_DisplayMin = displayMin;
        m_HeadsUpDisplayScrollingElementHoldCompleted = headsUpDisplayScrollingElement;
    }

    public void MoveTo(Vector2 target)
    {
        gameObject.SetActive(true);

        Vector2 start = transform.localPosition;
        float distance = Vector2.Distance(start, target);
        float time = distance / Constants.kHudHighScoreMovementSpeed;
        m_Slider.StartSlide(start, target, time);
    }

    public bool IsMoving()
    {
        return m_Slider.IsSliding();
    }

    public bool CanPush()
    {
        return m_DisplayMin == 0.0f;
    }
}
