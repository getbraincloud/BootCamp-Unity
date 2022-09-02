// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private float m_Delay;

    void Update()
    {
        if (m_Delay > 0.0f)
        {
            m_Delay -= Time.deltaTime;

            if (m_Delay < 0.0f)
            {
                m_Delay = 0.0f;
                Explode();
            }
        }
    }

    public void Spawn(Vector2 position, Vector2 scale, float delay)
    {
        m_Delay = delay;
        transform.localPosition = new Vector3(position.x, position.y, 0.0f);
        transform.localScale = new Vector3(scale.x, scale.y, 1.0f);

        if (m_Delay == 0.0f)
            Explode();
    }

    private void Explode()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<Animator>().Play("Explosion", -1, 0f);
    }

    public void OnExplosionEnd()
    {
        gameObject.SetActive(false);
    }
}
