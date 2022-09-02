// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FadeType
{
    None = -1,
    Out,
    In
};

public class Fader
{
    public delegate void OnFadeHasCompleted(Fader fader);

    private OnFadeHasCompleted m_OnFadeHasCompleted;
    private float m_Alpha;
    private float m_Elapsed = 0.0f;
    private float m_Duration = 0.0f;
    private float m_Delay = 0.0f;
    private FadeType m_Type = FadeType.None;

    // Update is called once per frame
    public void Update()
    {
        if (m_Type != FadeType.None)
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
                    m_Elapsed = m_Duration;

                m_Alpha = (m_Type == FadeType.Out) ? (1.0f - (m_Elapsed / m_Duration)) : (m_Elapsed / m_Duration);

                if (m_Elapsed == m_Duration && m_OnFadeHasCompleted != null)
                    m_OnFadeHasCompleted(this);
            }
        }
    }

    public void StartFade(FadeType fadeType, float duration, OnFadeHasCompleted onFadeHasCompleted = null)
    {
        StartFade(fadeType, duration, 0.0f, onFadeHasCompleted);
    }

    public void StartFade(FadeType fadeType, float duration, float delay, OnFadeHasCompleted onFadeHasCompleted = null)
    {
        m_OnFadeHasCompleted = onFadeHasCompleted;
        m_Type = fadeType;
        m_Alpha = m_Type == FadeType.Out ? 1.0f : 0.0f;
        m_Duration = duration;
        m_Elapsed = 0.0f;
        m_Delay = delay;
    }

    public void Reset(float alpha)
    {
        m_Type = FadeType.None;
        m_Alpha = alpha;
        m_Duration = 0.0f;
        m_Elapsed = 0.0f;
        m_Delay = 0.0f;
    }

    public bool IsFading()
    {
        return m_Duration > 0.0f && m_Type != FadeType.None;
    }

    public bool IsFadingIn()
    {
        return IsFading() && m_Type == FadeType.In;
    }

    public bool IsFadingOut()
    {
        return IsFading() && m_Type == FadeType.Out;
    }

    public bool IsDelayed()
    {
        return m_Delay > 0.0f && IsFading();
    }

    public float Alpha
    {
        get { return m_Alpha; }
    }
}
