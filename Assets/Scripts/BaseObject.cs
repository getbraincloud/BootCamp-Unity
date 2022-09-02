// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    private int m_Health = 1;
    private int m_AttackDamage = 1;
    private float m_FadeDelay;
    private float m_FadeTimer;
    private float m_FadeDuration;


    protected void SetAlpha(float alpha)
    {
        Color c = gameObject.GetComponent<SpriteRenderer>().GetComponent<SpriteRenderer>().color;
        c.a = alpha;
        gameObject.GetComponent<SpriteRenderer>().GetComponent<SpriteRenderer>().color = c;
    }

    public bool ApplyDamage(int damage)
    {
        m_Health -= damage;

        if (m_Health <= 0)
        {
            m_Health = 0;
            return true;
        }

        return false;
    }

    public void SetHealth(int health)
    {
        m_Health = health;
    }

    public int GetHealth()
    {
        return m_Health;
    }

    public void SetAttackDamage(int attackDamange)
    {
        m_AttackDamage = attackDamange;
    }

    public int GetAttackDamage()
    {
        return m_AttackDamage;
    }

    public bool IsFading()
    {
        return m_FadeTimer > 0.0f;
    }

    public void FadeOut(float fadeTime, float fadeDelay = 0.0f)
    {
        m_FadeDelay = fadeDelay;
        m_FadeTimer = 0.0f;
        m_FadeDuration = fadeTime;
    }

    protected void ResetFade()
    {
        m_FadeDelay = 0.0f;
        m_FadeTimer = 0.0f;
        m_FadeDuration = 0.0f;
        SetAlpha(1.0f);
    }

    protected void HandleFade()
    {
        if (m_FadeDelay > 0.0f)
        {
            m_FadeDelay -= Time.deltaTime;
            if (m_FadeDelay <= 0.0f)
                m_FadeDelay = 0.0f;
            else
                return;
        }

        if (m_FadeDuration > 0.0f)
        {
            m_FadeTimer += Time.deltaTime;
            if (m_FadeTimer >= m_FadeDuration)
            {
                ResetFade();
                gameObject.SetActive(false);
                return;
            }

            SetAlpha(1.0f - (m_FadeTimer / m_FadeDuration));
        }
    }
}
