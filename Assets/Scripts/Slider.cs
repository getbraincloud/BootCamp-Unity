// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider
{
    public delegate void OnSlideHasCompleted(Slider slider);

    private OnSlideHasCompleted m_OnSlideHasCompleted;
    private Vector2 m_Current;
    private Vector2 m_Start;
    private Vector2 m_Target;
    private float m_Elapsed = 0.0f;
    private float m_Duration = 0.0f;
    private float m_Delay = 0.0f;
    private bool m_IsSliding = false;

    // Update is called once per frame
    public void Update()
    {
        if (m_IsSliding)
        {
            if (m_Delay > 0.0f)
            {
                m_Delay -= Time.deltaTime;
                if (m_Delay <= 0.0f)
                    m_Delay = 0.0f;
                else
                    return;
            }

            if (m_Duration > 0.0f)
            {
                m_Elapsed += Time.deltaTime;
                if (m_Elapsed >= m_Duration)
                {
                    m_Elapsed = m_Duration;
                    m_IsSliding = false;
                }

                m_Current = m_Start + (m_Target - m_Start) * (m_Elapsed / m_Duration);

                if (m_Elapsed == m_Duration && m_OnSlideHasCompleted != null)
                    m_OnSlideHasCompleted(this);
            }
        }
    }

    public void StartSlide(Vector2 start, Vector2 target, float duration, OnSlideHasCompleted onSlideHasCompleted = null)
    {
        StartSlide(start, target, duration, 0.0f, onSlideHasCompleted);
    }

    public void StartSlide(Vector2 start, Vector2 target, float duration, float delay, OnSlideHasCompleted onSlideHasCompleted = null)
    {
        m_OnSlideHasCompleted = onSlideHasCompleted;
        m_Start = start;
        m_Current = m_Start;
        m_Target = target;
        m_Duration = duration;
        m_Elapsed = 0.0f;
        m_Delay = delay;
        m_IsSliding = true;
    }

    public void Pause()
    {
        m_IsSliding = false;
    }

    public void Stop()
    {
        m_Start = Vector2.zero;
        m_Current = Vector2.zero;
        m_Target = Vector2.zero;
        m_Duration = 0.0f;
        m_Elapsed = 0.0f;
        m_Delay = 0.0f;
        m_IsSliding = false;
    }

    public bool IsSliding()
    {
        return m_IsSliding;
    }

    public bool IsDelayed()
    {
        return m_Delay > 0.0f && m_IsSliding;
    }

    public Vector2 Current
    {
        get { return m_Current; }
    }
}
