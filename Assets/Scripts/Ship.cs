// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Ship : BaseObject
{
    [SerializeField] private GameObject shield;

    public delegate void ShipHasExploded();

    private Vector2 m_Acceleration;
    private Vector2 m_LinearVelocity;
    private Vector2 m_ScreenBounds;
    private float m_InvincibilityTimer;
    private ShipHasExploded m_ShipHasExploded;
    private Slider m_SpawnSlider;
    private bool m_IsSpawning = false;
    private bool m_IsHolding = false;

    // Start is called before the first frame update
    private void Awake()
    {
        m_ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        //gameObject.transform.localPosition = new Vector3(Constants.kShipOffScreenSpawnX, m_ScreenBounds.y * 0.5f, 0.0f);

        m_SpawnSlider = new Slider();

        SetHealth(Constants.kShipInitialHealth);
    }

    public void Spawn()
    {
        m_IsSpawning = true;

        Vector2 start = new Vector2(Constants.kShipOffScreenSpawnX, (m_ScreenBounds.y - Constants.kHudHeight) * 0.5f);
        Vector2 target = new Vector2(Constants.kShipSpawnX, (m_ScreenBounds.y - Constants.kHudHeight) * 0.5f);
        gameObject.transform.localPosition = start;
        m_SpawnSlider.StartSlide(start, target, 1.0f, OnSpawnCompleted);
    }

    public void HealUp()
    {
        SetHealth(Constants.kShipInitialHealth);

        shield.SetActive(true);
        shield.GetComponent<Shield>().SetHealth(Constants.kShipInitialShieldHealth);
    }

    public void Reset()
    {
        gameObject.SetActive(false);
        gameObject.transform.localPosition = new Vector3(Constants.kShipOffScreenSpawnX, (m_ScreenBounds.y - Constants.kHudHeight) * 0.5f, 0.0f);
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        m_InvincibilityTimer = 0.0f;
        m_IsSpawning = false;

        m_LinearVelocity = Vector2.zero;
        m_Acceleration = Vector2.zero;

        HealUp();
    }

    public void SetDelegate(ShipHasExploded shipHasExploded)
    {
        m_ShipHasExploded = shipHasExploded;
    }

    public new bool ApplyDamage(int damage)
    {
        if (IsInvincible())
            return false;

        // Set the player's invincibility timer, since they took damage
        m_InvincibilityTimer = Constants.kShipInvincibilityDuration;

        // If the player still has the shield, then take damage from it first
        if (HasShield())
        {
            if (shield.GetComponent<Shield>().GetHealth() < damage)
            {
                int diff = damage - shield.GetComponent<Shield>().GetHealth();
                shield.GetComponent<Shield>().ApplyDamage(damage);
                shield.gameObject.SetActive(false);
                return base.ApplyDamage(diff);
            }
            else
            {
                if (shield.GetComponent<Shield>().ApplyDamage(damage))
                    shield.gameObject.SetActive(false);
                return false;
            }
        }

        return base.ApplyDamage(damage);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsSpawning)
        {
            if (m_SpawnSlider.IsSliding())
            {
                m_SpawnSlider.Update();
                gameObject.transform.localPosition = m_SpawnSlider.Current;
            }
            return;
        }

        if (m_InvincibilityTimer > 0.0f)
        {
            m_InvincibilityTimer -= Time.deltaTime;
            if (m_InvincibilityTimer <= 0.0f)
            {
                m_InvincibilityTimer = 0.0f;
                SetAlpha(1.0f);
            }
            else
                SetAlpha(Constants.kShipInvincibleAlpha);
        }

        Vector2 drag;
        drag.x = m_Acceleration.x != 0.0f ? (m_Acceleration.x - (m_Acceleration.x * Constants.kShipDrag)) / m_Acceleration.x : -Constants.kShipDrag;
        drag.y = m_Acceleration.y != 0.0f ? (m_Acceleration.y - (m_Acceleration.y * Constants.kShipDrag)) / m_Acceleration.y : -Constants.kShipDrag;
        m_LinearVelocity += m_Acceleration + (m_LinearVelocity * drag) * Mathf.Pow(Time.deltaTime, 0.5f);

        // Cap the speed
        if (Mathf.Abs(m_LinearVelocity.magnitude) > Constants.kShipMaxSpeed)
            m_LinearVelocity = m_LinearVelocity.normalized * Constants.kShipMaxSpeed;

        // Move the ship
        Vector2 position = new Vector2(transform.localPosition.x, transform.localPosition.y);

        //if(!m_IsHolding)
            position += m_LinearVelocity * Time.deltaTime;

        // Clamp the position on screen
        position.x = Mathf.Clamp(position.x, Constants.kShipMinX, m_ScreenBounds.x - Constants.kShipMaxOffsetX);
        position.y = Mathf.Clamp(position.y, 0.0f, m_ScreenBounds.y - Constants.kHudHeight);
        transform.localPosition = position;

        // Tilt the ship when moving along the y-axis
        Vector3 rotation = transform.localRotation.eulerAngles;
        rotation.z = (Mathf.Rad2Deg * Constants.kShipTurnTilt) * (m_LinearVelocity.y / Constants.kShipMaxSpeed);
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation.z);
    }

    public bool HasShield()
    {
        return shield.activeInHierarchy;
    }

    public GameObject GetShield()
    {
        return shield;
    }

    public bool IsInvincible()
    {
        return m_InvincibilityTimer > 0.0f;
    }

    public void OnFire()
    {
        if (!m_IsSpawning && GetHealth() > 0)
        {
            FireLaser(true);
            FireLaser(false);
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        m_IsHolding = context.ReadValue<float>() != 0.0f;

        if (!m_IsHolding)
        {
            m_LinearVelocity = Vector2.zero;
            m_Acceleration = Vector2.zero;
        }
    }

    public void OnMovementUpStart()
    {
        if (!m_IsSpawning)
        {
            m_Acceleration.y += Constants.kShipAcceleration;
            m_LinearVelocity.y = m_Acceleration.y;
        }
    }

    public void OnMovementDownStart()
    {
        if (!m_IsSpawning)
        {
            m_Acceleration.y -= Constants.kShipAcceleration;
            m_LinearVelocity.y = m_Acceleration.y;
        }
    }

    public void OnMovementLeftStart()
    {
        if (!m_IsSpawning)
        {
            m_Acceleration.x -= Constants.kShipAcceleration;
            m_LinearVelocity.x = m_Acceleration.x;
        }
    }

    public void OnMovementRightStart()
    {
        if (!m_IsSpawning)
        {
            m_Acceleration.x += Constants.kShipAcceleration;
            m_LinearVelocity.x = m_Acceleration.x;
        }
    }

    public void OnMovementUpStop()
    {
        if (!m_IsSpawning)
        {
            m_Acceleration.y -= Constants.kShipAcceleration;
        }
    }

    public void OnMovementDownStop()
    {
        if (!m_IsSpawning)
        {
            m_Acceleration.y += Constants.kShipAcceleration;
        }
    }

    public void OnMovementLeftStop()
    {
        if (!m_IsSpawning)
        {
            m_Acceleration.x += Constants.kShipAcceleration;
        }
    }

    public void OnMovementRightStop()
    {
        if (!m_IsSpawning)
        {
            m_Acceleration.x -= Constants.kShipAcceleration;
        }
    }

    private void FireLaser(bool left)
    {
        Vector2 size = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;

        float radians = transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
        float magnitude = (size.y * 0.5f) - Constants.kShipGunOffset;
        float radiansTilt = left ? -Constants.kShipGunAngleTilt : Constants.kShipGunAngleTilt;
        float edgeRadians = radians + (left ? (Mathf.PI * 0.5f) : -(Mathf.PI * 0.5f));
        Vector2 direction = new Vector2(Mathf.Cos(radians + radiansTilt), Mathf.Sin(radians + radiansTilt));
        Vector2 edge = new Vector2(Mathf.Cos(edgeRadians), Mathf.Sin(edgeRadians)) * magnitude;
        Vector2 position = new Vector2(transform.localPosition.x, transform.localPosition.y) + edge;
        Vector2 linearVelocity = direction * Constants.kLaserSpeed;

        Spawner.sharedInstance.SpawnBlueLaser(position, linearVelocity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == Constants.kAsteroidLayer)
        {
            Asteroid a = other.gameObject.GetComponent<Asteroid>();
            int asteroidAttackDamage = a.GetAttackDamage();

            Statistic asteroidDestroyedStat = StatisticsManager.sharedInstance.GetStatisticByName(Constants.kBrainCloudStatAsteroidDestroyed);
            if (asteroidDestroyedStat != null)
                asteroidDestroyedStat.ApplyIncrement();

            a.Explode();

            if (ApplyDamage(asteroidAttackDamage))
                Explode();
        }
        else if (other.gameObject.layer == Constants.kEnemyLayer)
        {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            int enemyAttackDamage = e.GetAttackDamage();

            Statistic enemiesKilledStat = StatisticsManager.sharedInstance.GetStatisticByName(Constants.kBrainCloudStatEnemiesKilled);
            if (enemiesKilledStat != null)
                enemiesKilledStat.ApplyIncrement();

            e.Explode();

            if (ApplyDamage(enemyAttackDamage))
                Explode();
        }
        else if (other.gameObject.layer == Constants.kBossLayer)
        {
            Boss b = other.gameObject.GetComponent<Boss>();
            if (ApplyDamage(b.GetAttackDamage()))
                Explode();
        }
        else if (other.gameObject.layer == Constants.kLaserLayer)
        {
            Laser l = other.gameObject.GetComponent<Laser>();

            if (l.GetLaserColor() == Laser.Color.Red)
            {
                Spawner.sharedInstance.SpawnLaserImpact(l.GetFront(), l.GetLaserColor());

                if (ApplyDamage(l.GetAttackDamage()))
                    Explode();

                other.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.layer == Constants.kMissileLayer)
        {
            Missile m = other.gameObject.GetComponent<Missile>();

            if(m.GetMissleSize() == Missile.Size.Small)
                Spawner.sharedInstance.SpawnExplosion(m.GetMiddle(), new Vector2(0.2f, 0.2f));
            else
                Spawner.sharedInstance.SpawnExplosion(m.GetMiddle(), new Vector2(0.45f, 0.45f));

            if (ApplyDamage(m.GetAttackDamage()))
                Explode();

            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.layer == Constants.kPickupLayer)
        {
            HandlePickup(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }

    private void HandlePickup(GameObject go)
    {
        Pickup p = go.GetComponent<Pickup>();

        if (p.GetPickupType() == Pickup.Type.Shield)
        {
            if (!shield.activeInHierarchy)
                shield.SetActive(true);

            Shield s = shield.GetComponent<Shield>();
            if (s.GetHealth() < Constants.kShipInitialShieldHealth)
                s.SetHealth(s.GetHealth() + 1);
        }
    }

    private void Explode()
    {
        if (gameObject.activeInHierarchy)
        {
            Spawner.sharedInstance.SpawnExplosion(transform.localPosition, Vector2.one);

            GameObject leftWing = Spawner.sharedInstance.SpawnWing(transform.localPosition, transform.localRotation.z * Mathf.Deg2Rad, "Ship-LeftWing", true);
            if (leftWing) leftWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);

            GameObject rightWing = Spawner.sharedInstance.SpawnWing(transform.localPosition, transform.localRotation.z * Mathf.Deg2Rad, "Ship-RightWing", false);
            if (rightWing) rightWing.GetComponent<ShipWing>().FadeOut(Constants.kShipWingExplosionFadeOutTime);

            gameObject.SetActive(false);

            if (m_ShipHasExploded != null)
                m_ShipHasExploded();
        }
    }

    private void OnSpawnCompleted(Slider slider)
    {
        m_IsSpawning = false;
    }
}
