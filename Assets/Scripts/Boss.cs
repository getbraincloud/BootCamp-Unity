// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : BaseObject
{
    public delegate void BossHasExploded();

    private Vector2 m_SpriteSize;
    private Vector2 m_ScreenBounds;
    private float m_SmallMissileDelay;
    private float m_BigMissileDelay;
    private float m_MovementDelay;
    private Slider m_Slider;
    private BossHasExploded m_BossHasExploded;


    private enum State
    {
        Unknown = -1,
        Spawning,
        Attacking,
        Dead
    }

    private enum MovementDirection
    {
        Unknown = -1,
        Up,
        Down
    }

    private State m_State;
    private MovementDirection m_LastMovementDirection;

    private void Awake()
    {
        m_ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        m_Slider = new Slider();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Slider.IsSliding())
        {
            m_Slider.Update();
            gameObject.transform.localPosition = m_Slider.Current;
        }

        if (m_State == State.Attacking)
        {
            if (m_MovementDelay > 0.0f)
            {
                m_MovementDelay -= Time.deltaTime;

                if (m_MovementDelay < 0.0f)
                {
                    SetupNextMovement();
                }
            }

            // Clamp the position on screen
            Vector2 position = gameObject.transform.localPosition;
            Vector2 bossSize = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
            position.x = Mathf.Clamp(position.x, Constants.kBossMinX, m_ScreenBounds.x - Constants.kHudHeight - bossSize.x * 0.5f);
            position.y = Mathf.Clamp(position.y, bossSize.y * 0.5f, m_ScreenBounds.y - Constants.kHudHeight - bossSize.y * 0.5f);
            gameObject.transform.localPosition = position;


            if (m_SmallMissileDelay > 0.0f)
            {
                m_SmallMissileDelay -= Time.deltaTime;

                if (m_SmallMissileDelay <= 0.0f)
                {
                    m_SmallMissileDelay = 0.0f;
                    FireMissile(Missile.Size.Small);
                }
            }

            if (m_BigMissileDelay > 0.0f)
            {
                m_BigMissileDelay -= Time.deltaTime;

                if (m_BigMissileDelay <= 0.0f)
                {
                    m_BigMissileDelay = 0.0f;
                    FireMissile(Missile.Size.Big);
                }
            }

        }
    }

    public void SetDelegate(BossHasExploded bossHasExploded)
    {
        m_BossHasExploded = bossHasExploded;
    }

    public void Spawn()
    {
        gameObject.SetActive(true);

        m_State = State.Spawning;
        m_LastMovementDirection = MovementDirection.Unknown;

        Vector2 start = new Vector2(Constants.kBossOffScreenSpawnX, (m_ScreenBounds.y - Constants.kHudHeight) * 0.5f);
        Vector2 target = new Vector2(Constants.kBossSpawnX, (m_ScreenBounds.y - Constants.kHudHeight) * 0.5f);
        gameObject.transform.localPosition = start;
        m_Slider.StartSlide(start, target, 1.0f, OnSlideCompleted);

        ResetMissileDelay(Missile.Size.Small);
        ResetMissileDelay(Missile.Size.Big);

        SetHealth(Constants.kBossHealth);
    }

    private void FireMissile(Missile.Size size)
    {
        float radians = transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
        float halfPI = Mathf.PI * 0.5f;
        float sixRadians = 0.1047198f; //6 degrees
        Vector2 position = transform.localPosition;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        if (size == Missile.Size.Small)
        {
            Vector2 linearVelocity = direction * Constants.kMissileSmallSpeed;
            Vector2 direction2 = new Vector2(Mathf.Cos(radians + sixRadians), Mathf.Sin(radians + sixRadians));
            Vector2 linearVelocity2 = direction2 * Constants.kMissileSmallSpeed;
            Vector2 direction3 = new Vector2(Mathf.Cos(radians - sixRadians), Mathf.Sin(radians - sixRadians));
            Vector2 linearVelocity3 = direction3 * Constants.kMissileSmallSpeed;

            Vector2 edge1 = new Vector2(Mathf.Cos(radians + halfPI), Mathf.Sin(radians + halfPI)) * Constants.kBossGunOffset2;
            Vector2 position1 = position + direction * Constants.kBossGunOffset1 + edge1;
            Spawner.sharedInstance.SpawnMissile(position1, linearVelocity, Missile.Size.Small);
            //Spawner.sharedInstance.SpawnMissile(position1, linearVelocity2, Missile.Size.Small);
            //Spawner.sharedInstance.SpawnMissile(position1, linearVelocity3, Missile.Size.Small);

            Vector2 edge2 = new Vector2(Mathf.Cos(radians - halfPI), Mathf.Sin(radians - halfPI)) * Constants.kBossGunOffset2;
            Vector2 position2 = position + direction * Constants.kBossGunOffset1 + edge2;
            Spawner.sharedInstance.SpawnMissile(position2, linearVelocity, Missile.Size.Small);
            //Spawner.sharedInstance.SpawnMissile(position2, linearVelocity2, Missile.Size.Small);
            //Spawner.sharedInstance.SpawnMissile(position2, linearVelocity3, Missile.Size.Small);
        }
        else
        {
            Vector2 linearVelocity = direction * Constants.kMissileBigSpeed;
            Vector2 missilePosition = position + direction * Constants.kBossGunOffset1;
            Spawner.sharedInstance.SpawnMissile(missilePosition, linearVelocity, Missile.Size.Big);
        }

        ResetMissileDelay(size);
    }

    private void ResetMissileDelay(Missile.Size size)
    {
        if (size == Missile.Size.Small)
            m_SmallMissileDelay = Random.Range(Constants.kBossSmallMissileMinDelay, Constants.kBossSmallMissileMaxDelay);
        else
            m_BigMissileDelay = Random.Range(Constants.kEnemyLaserMinDelay, Constants.kEnemyLaserMaxDelay);
    }

    private void SetupNextMovement()
    {
        if (m_LastMovementDirection == MovementDirection.Down)
            SetupUpMovement();
        else if (m_LastMovementDirection == MovementDirection.Up)
            SetupDownMovement();
        else
        {
            if (Random.Range(0, 4) <= 1)
                SetupUpMovement();
            else
                SetupDownMovement();
        }
    }

    private void SetupUpMovement()
    {
        m_LastMovementDirection = MovementDirection.Up;

        float radians = Random.Range(0.0f, Mathf.PI);
        float distance = Random.Range(Constants.kBossMovementMinRange, Constants.kBossMovementMaxRange);
        float time = distance / Constants.kBossSpeed;
        Vector2 start = transform.localPosition;
        Vector2 target = start + new Vector2(Mathf.Cos(radians) * distance, Mathf.Sin(radians) * distance);

        m_Slider.StartSlide(start, target, time, OnSlideCompleted);
    }

    private void SetupDownMovement()
    {
        m_LastMovementDirection = MovementDirection.Down;

        float radians = -Random.Range(0.0f, Mathf.PI);
        float distance = Random.Range(Constants.kBossMovementMinRange, Constants.kBossMovementMaxRange);
        float time = distance / Constants.kBossSpeed;
        Vector2 start = transform.localPosition;
        Vector2 target = start + new Vector2(Mathf.Cos(radians) * distance, Mathf.Sin(radians) * distance);

        m_Slider.StartSlide(start, target, time, OnSlideCompleted);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.activeInHierarchy)
        {
            if (other.gameObject.layer == Constants.kAsteroidLayer)
            {
                Asteroid a = other.gameObject.GetComponent<Asteroid>();
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
            m_State = State.Dead;

            Spawner.sharedInstance.SpawnExplosion(transform.localPosition, new Vector2(4.0f, 4.0f));

            // Front wings
            float leftFrontOffset = Mathf.PI * 0.75f;
            GameObject leftFrontWing = Spawner.sharedInstance.SpawnBossWing(transform.localPosition, transform.localRotation.eulerAngles.z * Mathf.Deg2Rad, leftFrontOffset, Constants.kBossFrontLeftWingAtlasKey);
            if (leftFrontWing)
                leftFrontWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);

            float rightFrontOffset = -Mathf.PI * 0.75f;
            GameObject rightFrontWing = Spawner.sharedInstance.SpawnBossWing(transform.localPosition, transform.localRotation.eulerAngles.z * Mathf.Deg2Rad, rightFrontOffset, Constants.kBossFrontRightWingAtlasKey);
            if (rightFrontWing)
                rightFrontWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);

            // Middle wings
            float leftMiddleOffset = Mathf.PI * 0.5f;
            GameObject leftMiddleWing = Spawner.sharedInstance.SpawnBossWing(transform.localPosition, transform.localRotation.eulerAngles.z * Mathf.Deg2Rad, leftMiddleOffset, Constants.kBossMiddleLeftWingAtlasKey);
            if (leftMiddleWing)
                leftMiddleWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);

            float rightMiddleOffset = -Mathf.PI * 0.5f;
            GameObject rightMiddleWing = Spawner.sharedInstance.SpawnBossWing(transform.localPosition, transform.localRotation.eulerAngles.z * Mathf.Deg2Rad, rightMiddleOffset, Constants.kBossMiddleRightWingAtlasKey);
            if (rightMiddleWing)
                rightMiddleWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);


            // Back wings
            float leftBackOffset = Mathf.PI * 0.25f;
            GameObject leftBackWing = Spawner.sharedInstance.SpawnBossWing(transform.localPosition, transform.localRotation.eulerAngles.z * Mathf.Deg2Rad, leftBackOffset, Constants.kBossBackLeftWingAtlasKey);
            if (leftBackWing)
                leftBackWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);

            float rightBackOffset = -Mathf.PI * 0.25f;
            GameObject rightBackWing = Spawner.sharedInstance.SpawnBossWing(transform.localPosition, transform.localRotation.eulerAngles.z * Mathf.Deg2Rad, rightBackOffset, Constants.kBossBackRightWingAtlasKey);
            if (rightBackWing)
                rightBackWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);

            gameObject.SetActive(false);

            if (m_BossHasExploded != null)
                m_BossHasExploded();
        }
    }

    private void OnSlideCompleted(Slider slider)
    {
        if (m_State == State.Spawning)
        {
            m_State = State.Attacking;
        }
        
        m_MovementDelay = Random.Range(Constants.kBossMovementMinDelay, Constants.kBossMovementMaxDelay);
    }
}
