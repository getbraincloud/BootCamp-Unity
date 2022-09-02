// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : BaseObject
{
    private Vector2 m_LinearVelocity;
    private Vector2 m_ScreenBounds;
    private float m_Lifetime;

    public enum Type
    {
        Unknown = -1,
        Shield
    };

    private Type m_Type;

    private void Awake()
    {
        m_ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        HandleFade();

        if (m_Lifetime > 0.0f)
        {
            m_Lifetime -= Time.deltaTime;
            if (m_Lifetime <= 0.0f)
            {
                m_Lifetime = 0.0f;
                FadeOut(Constants.kPickupFadeOutTime);
            }
        }

        Vector3 position = transform.localPosition;
        position += new Vector3(m_LinearVelocity.x, m_LinearVelocity.y, 0.0f) * Time.deltaTime;
        transform.localPosition = position;

        Vector2 size = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        if (position.x < -size.x)
            gameObject.SetActive(false);
    }

    public void Spawn(Vector2 position, Vector2 linearVelocity, Type type)
    {
        gameObject.SetActive(true);

        m_Type = type;
        transform.localPosition = position;
        m_LinearVelocity = linearVelocity;
        m_Lifetime = Constants.kPickupLifetime;
    }

    public Type GetPickupType()
    {
        return m_Type;
    }
}
