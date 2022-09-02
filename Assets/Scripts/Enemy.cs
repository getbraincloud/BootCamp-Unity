// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseObject
{
    private Vector2 m_LinearVelocity;
    private Vector2 m_SpriteSize;
    private Vector2 m_ScreenBounds;
    private float m_LaserDelay;
    private float m_EnemyFourFiringDuration;   //Enemy.Type.Four only
    private float m_EnemyFourFiringCooldown;   //Enemy.Type.Four only
    private float m_EnemyFiveMissileDelay;     //Enemy.Type.Five only
    private float m_EnemyFiveMissileCooldown;  //Enemy.Type.Five only

    private bool m_LaserAlternate = true;


    public enum Type
    {
        Unknown = -1,
        One,
        Two,
        Three,
        Four,
        Five
    };

    private Type m_Type;

    // Start is called before the first frame update
    void Start()
    {
        m_ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        // Move the enemy
        Vector3 position = transform.localPosition;
        position += new Vector3(m_LinearVelocity.x, m_LinearVelocity.y, 0.0f) * Time.deltaTime;
        transform.localPosition = position;

        // Only fire a laser if the front of the enemy is on-screen
        if (position.x < m_ScreenBounds.x)
        {
            bool canFire = true;

            // Logic below is used for enemy four and five's firing behaviours

            if (m_Type == Type.Four)
            {
                if (m_EnemyFourFiringCooldown > 0.0f)
                {
                    m_EnemyFourFiringCooldown -= Time.deltaTime;

                    if (m_EnemyFourFiringCooldown < 0.0f)
                    {
                        m_EnemyFourFiringCooldown = 0.0f;
                        m_EnemyFourFiringDuration = Constants.kEnemyFourFiringDuration;
                    }
                    else
                        canFire = false;
                }
                else
                {

                    if (m_EnemyFourFiringDuration > 0.0f)
                    {
                        m_EnemyFourFiringDuration -= Time.deltaTime;

                        if (m_EnemyFourFiringDuration < 0.0f)
                        {
                            m_EnemyFourFiringDuration = 0.0f;
                            m_EnemyFourFiringCooldown = Constants.kEnemyFourFiringCooldown;
                            canFire = false;
                        }
                    }
                }
            }
            else if (m_Type == Type.Five)
            {
                if (m_EnemyFiveMissileCooldown > 0.0f)
                {
                    m_EnemyFiveMissileCooldown -= Time.deltaTime;

                    if (m_EnemyFiveMissileCooldown < 0.0f)
                    {
                        m_EnemyFiveMissileDelay = Random.Range(Constants.kEnemyFiveMissileMinDelay, Constants.kEnemyFiveMissileMaxDelay);
                        canFire = false;
                    }
                }
                else
                {

                    if (m_EnemyFiveMissileDelay > 0.0f)
                    {
                        m_EnemyFiveMissileDelay -= Time.deltaTime;

                        if (m_EnemyFiveMissileDelay < 0.0f)
                        {
                            m_EnemyFiveMissileDelay = 0.0f;
                            m_EnemyFiveMissileCooldown = Random.Range(Constants.kEnemyFiveFiringMinCooldown, Constants.kEnemyFiveFiringMaxCooldown);
                            FireMissle();
                            ResetLaserDelay();
                        }
                        else
                            canFire = false;
                    }
                }
            }

            if (m_LaserDelay > 0.0f && canFire)
            {
                m_LaserDelay -= Time.deltaTime;

                if (m_LaserDelay <= 0.0f)
                {
                    m_LaserDelay = 0.0f;
                    FireLaser();
                    ResetLaserDelay();
                }
            }
        }

        // Deactivate the enemy if it's gone off-screen
        if (position.x < -(m_SpriteSize.x * 0.5f) || position.y < -(m_SpriteSize.y * 0.5f) || position.y > m_ScreenBounds.y + (m_SpriteSize.y * 0.5f))
            gameObject.SetActive(false);
    }

    public void Spawn(Vector2 position, Vector2 linearVelocity, Type type, int health)
    {
        if (type != Enemy.Type.Unknown)
        {
            gameObject.SetActive(true);

            m_Type = type;
            m_LinearVelocity = linearVelocity;

            transform.localPosition = position;
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(linearVelocity.y, linearVelocity.x) * Mathf.Rad2Deg);

            SetHealth(health);
            ResetLaserDelay();

            int keyIndex = (int)m_Type;
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/" + Constants.kEnemyAtlasKeys[keyIndex]);

            if (m_Type == Type.Four)
            {
                m_EnemyFourFiringCooldown = Constants.kEnemyFourFiringCooldown * 2.0f;
                m_EnemyFourFiringDuration = 0.0f;
            }
            else if (m_Type == Type.Five)
            {
                m_EnemyFiveMissileCooldown = Random.Range(Constants.kEnemyFiveFiringMinCooldown, Constants.kEnemyFiveFiringMaxCooldown);
            }

            m_SpriteSize = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
            gameObject.GetComponent<CircleCollider2D>().radius = m_SpriteSize.x * 0.48f;
        }
    }

    private void FireMissle()
    {
        if (m_Type == Type.Five)
        {
            float radians = transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 position = transform.localPosition;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            Vector2 linearVelocity = direction * Constants.kMissileSmallSpeed;
            Vector2 missilePosition = position + direction * Constants.kEnemyGunOffset1;
            Spawner.sharedInstance.SpawnMissile(missilePosition, linearVelocity, Missile.Size.Small);
        }
    }

    private void FireLaser()
    {
        float radians = transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
        float halfPI = Mathf.PI * 0.5f;
        Vector2 position = transform.localPosition;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 linearVelocity = direction * Constants.kLaserSpeed;

        if (m_Type == Type.One)
        {
            Vector2 laserPosition = position + direction * Constants.kEnemyGunOffset1;
            Spawner.sharedInstance.SpawnRedLaser(laserPosition, linearVelocity);
        }
        else if (m_Type == Type.Two)
        {
            Vector2 edge1 = new Vector2(Mathf.Cos(radians + halfPI), Mathf.Sin(radians + halfPI)) * Constants.kEnemyGunOffset2;
            Vector2 position1 = position + direction * Constants.kEnemyGunOffset1 + edge1;
            Spawner.sharedInstance.SpawnRedLaser(position1, linearVelocity);

            Vector2 edge2 = new Vector2(Mathf.Cos(radians - halfPI), Mathf.Sin(radians - halfPI)) * Constants.kEnemyGunOffset2;
            Vector2 position2 = position + direction * Constants.kEnemyGunOffset1 + edge2;
            Spawner.sharedInstance.SpawnRedLaser(position2, linearVelocity);
        }
        else if (m_Type == Type.Three)
        {
            m_LaserAlternate = !m_LaserAlternate;
            if (m_LaserAlternate)
            {
                Vector2 edge1 = new Vector2(Mathf.Cos(radians + halfPI), Mathf.Sin(radians + halfPI)) * Constants.kEnemyGunOffset3;
                Vector2 position1 = position + direction * Constants.kEnemyGunOffset1 + edge1;
                Spawner.sharedInstance.SpawnRedLaser(position1, linearVelocity);
            }
            else
            {
                Vector2 edge2 = new Vector2(Mathf.Cos(radians - halfPI), Mathf.Sin(radians - halfPI)) * Constants.kEnemyGunOffset3;
                Vector2 position2 = position + direction * Constants.kEnemyGunOffset1 + edge2;
                Spawner.sharedInstance.SpawnRedLaser(position2, linearVelocity);
            }
        }
        else if (m_Type == Type.Four)
        {
            m_LaserAlternate = !m_LaserAlternate;
            if (m_LaserAlternate)
            {
                Vector2 edge1 = new Vector2(Mathf.Cos(radians + halfPI), Mathf.Sin(radians + halfPI)) * Constants.kEnemyGunOffset4_1;
                Vector2 edge4 = new Vector2(Mathf.Cos(radians - halfPI), Mathf.Sin(radians - halfPI)) * Constants.kEnemyGunOffset4_2;
                Vector2 position1 = position + direction * Constants.kEnemyGunOffset1 + edge1;
                Vector2 position4 = position + direction * Constants.kEnemyGunOffset1 + edge4;

                Spawner.sharedInstance.SpawnRedLaser(position1, linearVelocity);
                Spawner.sharedInstance.SpawnRedLaser(position4, linearVelocity);
            }
            else
            {
                Vector2 edge2 = new Vector2(Mathf.Cos(radians + halfPI), Mathf.Sin(radians + halfPI)) * Constants.kEnemyGunOffset4_2;
                Vector2 edge3 = new Vector2(Mathf.Cos(radians - halfPI), Mathf.Sin(radians - halfPI)) * Constants.kEnemyGunOffset4_1;
                Vector2 position2 = position + direction * Constants.kEnemyGunOffset1 + edge2;
                Vector2 position3 = position + direction * Constants.kEnemyGunOffset1 + edge3;

                Spawner.sharedInstance.SpawnRedLaser(position2, linearVelocity);
                Spawner.sharedInstance.SpawnRedLaser(position3, linearVelocity);

            }
        }
        else if (m_Type == Type.Five)
        {
            Vector2 edge1 = new Vector2(Mathf.Cos(radians + halfPI), Mathf.Sin(radians + halfPI)) * Constants.kEnemyGunOffset5;
            Vector2 position1 = position + direction * Constants.kEnemyGunOffset1 + edge1;
            Spawner.sharedInstance.SpawnRedLaser(position1, linearVelocity);

            Vector2 edge2 = new Vector2(Mathf.Cos(radians - halfPI), Mathf.Sin(radians - halfPI)) * Constants.kEnemyGunOffset5;
            Vector2 position2 = position + direction * Constants.kEnemyGunOffset1 + edge2;
            Spawner.sharedInstance.SpawnRedLaser(position2, linearVelocity);
        }
    }

    private void ResetLaserDelay()
    {
        if (m_Type == Type.Three)
            m_LaserDelay = Constants.kEnemyThreeFireDelay;

        else if (m_Type == Type.Four)
            m_LaserDelay = Constants.kEnemyFourFireDelay;
        else if(m_Type == Type.Five)
            m_LaserDelay = Random.Range(Constants.kEnemyFiveLaserMinDelay, Constants.kEnemyFiveLaserMaxDelay);
        else
            m_LaserDelay = Random.Range(Constants.kEnemyLaserMinDelay, Constants.kEnemyLaserMaxDelay);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.activeInHierarchy)
        {
            if (other.gameObject.layer == Constants.kAsteroidLayer)
            {
                Asteroid a = other.gameObject.GetComponent<Asteroid>();
                if (ApplyDamage(a.GetAttackDamage()))
                    Explode();
                a.Explode();
            }
            else if (other.gameObject.layer == Constants.kLaserLayer)
            {
                Laser l = other.gameObject.GetComponent<Laser>();

                if (l.GetLaserColor() == Laser.Color.Blue)
                {
                    Spawner.sharedInstance.SpawnLaserImpact(l.GetFront(), l.GetLaserColor());

                    if (ApplyDamage(l.GetAttackDamage()))
                    {
                        Explode();
                    }

                    other.gameObject.SetActive(false);
                }
            }
        }
    }

    public void Explode()
    {
        if (gameObject.activeInHierarchy)
        {
            Spawner.sharedInstance.SpawnExplosion(transform.localPosition, Vector2.one);

            int keyIndex = (int)m_Type;

            string leftTexture = Constants.kEnemyWingLeftAtlasKeys[keyIndex];
            GameObject leftWing = Spawner.sharedInstance.SpawnWing(transform.localPosition, transform.localRotation.eulerAngles.z * Mathf.Deg2Rad, leftTexture, true);
            if (leftWing)
                leftWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);

            string rightTexture = Constants.kEnemyWingRightAtlasKeys[keyIndex];
            GameObject rightWing = Spawner.sharedInstance.SpawnWing(transform.localPosition, transform.localRotation.eulerAngles.z * Mathf.Deg2Rad, rightTexture, false);
            if (rightWing)
                rightWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);

            if (Game.sharedInstance.GetShipShieldHealth() < Constants.kShipInitialShieldHealth && Random.Range(1, 6) == 1)
                Spawner.sharedInstance.SpawnPickup(transform.localPosition, m_LinearVelocity, Pickup.Type.Shield);

            gameObject.SetActive(false);
        }
    }
}
