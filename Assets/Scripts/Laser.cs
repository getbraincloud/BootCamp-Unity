// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BaseObject
{
    public enum Color
    {
        Unknown = -1,
        Blue,
        Red
    };

    private Vector2 m_LinearVelocity;
    private Vector2 m_ScreenBounds;
    private Color m_Color;

    // Start is called before the first frame update
    void Start()
    {
        m_ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = transform.localPosition;
        position += m_LinearVelocity * Time.deltaTime;
        transform.localPosition = position;

        Vector2 size = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        if (position.x > m_ScreenBounds.x || position.x < -size.x)
            gameObject.SetActive(false);
    }

    public void Spawn(Vector2 position, Vector2 linearVelocity, Color color)
    {
        gameObject.SetActive(true);
 
        transform.localPosition = new Vector3(position.x, position.y, 0.0f);
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(linearVelocity.y, linearVelocity.x) * Mathf.Rad2Deg);

        m_LinearVelocity = linearVelocity;
        m_Color = color;

        if (m_Color != Color.Unknown)
        {
            string texture = m_Color == Color.Blue ? "Laser-Blue" : "Laser-Red";
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/" + texture);
        }
        else
            gameObject.SetActive(false);
    }

    public Vector3 GetFront()
    {
        Vector2 size = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        Vector2 displacement = m_LinearVelocity.normalized * size.x;
        return transform.position + new Vector3(displacement.x, displacement.y, 0.0f);
    }

    public Color GetLaserColor()
    {
        return m_Color;
    }
}
