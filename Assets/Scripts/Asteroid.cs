// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : BaseObject
{
    private Vector2 m_LinearVelocity;
    private Vector2 m_SpriteSize;
    private Vector2 m_ScreenBounds;
    private float m_AngularVelocity;
    private float m_SpinDirection;

    public enum Size
    {
        Unknown = -1,
        Big,
        Medium,
        Small,
        Tiny
    };

    private Size m_Size;

    // Start is called before the first frame update
    private void Awake()
    {
        m_ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

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

        // If the asteroid goes off-screen deactivate it
        if (transform.position.x < -(m_SpriteSize.x * 0.5f) || transform.position.y < -(m_SpriteSize.y * 0.5f) || transform.position.y > (m_ScreenBounds.y - Constants.kHudHeight) + (m_SpriteSize.y * 0.5f))
            gameObject.SetActive(false);
    }

    public void Spawn(Vector2 position, Vector2 linearVelocity, Size size)
    {
        ResetFade();

        gameObject.SetActive(true);

        transform.localPosition = position;
        m_LinearVelocity = linearVelocity;
        m_AngularVelocity = Random.Range(Constants.kAsteroidMinAngularVelocity, Constants.kAsteroidMaxAngularVelocity);
        m_SpinDirection = Random.Range(0, 2) == 1 ? 1.0f : -1.0f;
        m_Size = size;

        int sizeValue = (int)m_Size;
        SetHealth(Constants.kAsteroidHealth[sizeValue]);
        SetAttackDamage(Constants.kAsteroidAttackDamage[sizeValue]);

        // Determine the atlas frame to use base on the size of the asteroid
        int keyIndex = Random.Range(0, Constants.kNumAsteroidVariations[sizeValue]);

        switch (m_Size)
        {
            case Size.Big:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/" + Constants.kBigAsteroidAtlasKeys[keyIndex]);
                }
                break;

            case Size.Medium:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/" + Constants.kMediumAsteroidAtlasKeys[keyIndex]);
                }
                break;

            case Size.Small:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/" + Constants.kSmallAsteroidAtlasKeys[keyIndex]);
                }
                break;

            case Size.Tiny:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/" + Constants.kTinyAsteroidAtlasKeys[keyIndex]);
                }
                break;

            default:
                {
                    gameObject.SetActive(false);
                    return;
                }
        }

        // Lastly update the sprite size and collider radius (as it might have changed)
        m_SpriteSize = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        gameObject.GetComponent<CircleCollider2D>().radius = m_SpriteSize.x * 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.activeInHierarchy && !IsFading())
        {
            if (other.gameObject.layer == Constants.kLaserLayer)
            {
                Laser l = other.gameObject.GetComponent<Laser>();
                Spawner.sharedInstance.SpawnLaserImpact(l.GetFront(), l.GetLaserColor());

                if (ApplyDamage(l.GetAttackDamage()))
                {
                    Explode();

                    if (l.GetLaserColor() == Laser.Color.Blue)
                    {
                        Statistic asteroidDestroyedStat = StatisticsManager.sharedInstance.GetStatisticByName(Constants.kBrainCloudStatAsteroidDestroyed);
                        if (asteroidDestroyedStat != null)
                            asteroidDestroyedStat.ApplyIncrement();
                    }
                }
                other.gameObject.SetActive(false);
            }
        }
    }

    public void Explode(bool onlySmallDebris = false)
    {
        if (gameObject.activeInHierarchy && (m_Size == Size.Big || m_Size == Size.Medium))
        {
            Vector2 position = transform.localPosition;
            Vector2 linearVelocity;
            Asteroid.Size size = Asteroid.Size.Unknown;

            int numDirections = m_Size == Asteroid.Size.Big ? 6 : 3;
            float directionsIncrement = (Mathf.PI * 2.0f) / numDirections;
            float offset = Random.Range(0.0f, Mathf.PI * 0.5f);
            float radians = 0.0f;
            float speed = 0.0f;

            // Spawn little asteroids as a result of the explosion
            for (int i = 0; i < numDirections; ++i)
            {
                if (onlySmallDebris)
                    size = Random.Range(0, 2) == 1 ? Asteroid.Size.Small : Asteroid.Size.Tiny;
                else
                {
                    if (m_Size == Asteroid.Size.Big)
                        size = Random.Range(0, 5) > 2 ? Asteroid.Size.Medium : Asteroid.Size.Small;
                    else
                        size = Random.Range(0, 2) == 1 ? Asteroid.Size.Small : Asteroid.Size.Tiny;
                }

                radians = (directionsIncrement * i) + offset;
                speed = Random.Range(Constants.kAsteroidExplosionMinSpeed, Constants.kAsteroidExplosionMaxSpeed);
                linearVelocity.x = Mathf.Cos(radians) * speed;
                linearVelocity.y = Mathf.Sin(radians) * speed;


                GameObject go = Spawner.sharedInstance.SpawnAsteroid(position, linearVelocity, size);
                if (go && size != Asteroid.Size.Medium)
                    go.GetComponent<Asteroid>().FadeOut(Constants.kAsteroidExplosionFadeOutTime, Constants.kAsteroidExplosionFadeDelay);
            }

            // Spawn an explosion
            float scaleF = m_Size == Asteroid.Size.Big ? 1.0f : 0.6f;
            Vector2 scale = new Vector2(scaleF, scaleF);
            Spawner.sharedInstance.SpawnExplosion(position, scale);

            // Lasterly deactivate the asteroid
            gameObject.SetActive(false);
        }
    }
}