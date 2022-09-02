// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserImpact : MonoBehaviour
{
    private float m_Lifetime;
    private Laser.Color m_Color;

    // Update is called once per frame
    void Update()
    {
        if (m_Lifetime > 0.0f)
        {
            m_Lifetime -= Time.deltaTime;

            if (m_Lifetime <= 0.0f)
            {
                m_Lifetime = 0.0f;
                gameObject.SetActive(false);
            }
        }
    }

    public void Spawn(Vector2 position, Laser.Color color)
    {
        gameObject.SetActive(true);

        transform.localPosition = new Vector3(position.x, position.y, 0.0f);
        m_Color = color;
        m_Lifetime = Constants.kLaserImpactLifetime;

        if (m_Color != Laser.Color.Unknown)
        {
            string texture = m_Color == Laser.Color.Blue ? "LaserImpact-Blue" : "LaserImpact-Red";
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/" + texture);
        }
        else
            gameObject.SetActive(false);
    }
}
