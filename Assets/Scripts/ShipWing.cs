// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWing : BaseObject
{
    private Vector2 m_LinearVelocity;
    private float m_AngularVelocity;
    private float m_SpinDirection;

    // Update is called once per frame
    void Update()
    {
        HandleFade();

        // Move the asteroid
        Vector3 position = transform.localPosition;
        position += new Vector3(m_LinearVelocity.x, m_LinearVelocity.y, 0.0f) * Time.deltaTime;
        transform.localPosition = position;

        float angle = transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
        angle += m_AngularVelocity * m_SpinDirection * Time.deltaTime;
        if (m_SpinDirection > 0.0f && angle > Mathf.PI * 2.0f)
        {
            angle -= Mathf.PI * 2.0f;
        }
        else if (m_SpinDirection < 0.0f && angle < -(Mathf.PI * 2.0f))
        {
            angle += Mathf.PI * 2.0f;
        }

        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle * Mathf.Rad2Deg);
    }

    public void Spawn(Vector2 position, Vector2 linearVelocity, float radiansZ, string texture)
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/" + texture);

        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, radiansZ * Mathf.Rad2Deg);
        transform.localPosition = position;
        m_LinearVelocity = linearVelocity;
        m_AngularVelocity = Random.Range(Constants.kShipWingMinAngularVelocity, Constants.kShipWingMaxAngularVelocity);
        m_SpinDirection = Random.Range(0, 2) == 1 ? 1.0f : -1.0f;

        SetAlpha(1.0f);
    }
}
