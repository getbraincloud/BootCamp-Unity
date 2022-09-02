// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner sharedInstance;

    [SerializeField] private Boss boss;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject laserImpactPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private GameObject shipWingPrefab;

    private GameObject m_AsteroidPool;
    private GameObject m_MissilePool;
    private GameObject m_LaserPool;
    private GameObject m_LaserImpactPool;
    private GameObject m_ExplosionPool;
    private GameObject m_EnemyPool;
    private GameObject m_PickupPool;
    private GameObject m_ShipWingPool;

    private List<LevelData> m_LevelData;
    private LevelData m_DefaultLevel;
    private Vector2 m_ScreenBounds;
    private float m_AsteroidSpawnTimer;
    private float m_EnemySpawnTimer;
    private int m_LevelIndex;
    private bool m_CanSpawn = false;


    private void Awake()
    {
        sharedInstance = this;

        m_ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        boss.gameObject.SetActive(false);

        // Creeate the object pools
        m_AsteroidPool = new GameObject("AsteroidPool");
        m_AsteroidPool.AddComponent<ObjectPool>().Init(asteroidPrefab, Constants.kAsteroidPoolSize);

        m_MissilePool = new GameObject("MissilePool");
        m_MissilePool.AddComponent<ObjectPool>().Init(missilePrefab, Constants.kMissilePoolSize);

        m_LaserPool = new GameObject("LaserPool");
        m_LaserPool.AddComponent<ObjectPool>().Init(laserPrefab, Constants.kLaserPoolSize);

        m_LaserImpactPool = new GameObject("LaserImpactPool");
        m_LaserImpactPool.AddComponent<ObjectPool>().Init(laserImpactPrefab, Constants.kLaserImpactPoolSize);

        m_ExplosionPool = new GameObject("ExplosionPool");
        m_ExplosionPool.AddComponent<ObjectPool>().Init(explosionPrefab, Constants.kExplosionPoolSize);

        m_EnemyPool = new GameObject("EnemyPool");
        m_EnemyPool.AddComponent<ObjectPool>().Init(enemyPrefab, Constants.kEnemyPoolSize);

        m_PickupPool = new GameObject("PickupPool");
        m_PickupPool.AddComponent<ObjectPool>().Init(pickupPrefab, Constants.kPickupPoolSize);

        m_ShipWingPool = new GameObject("ShipWingPool");
        m_ShipWingPool.AddComponent<ObjectPool>().Init(shipWingPrefab, Constants.kShipWingPoolSize);

        m_DefaultLevel = new LevelData();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_CanSpawn)
            return;

        if (HandleSpawnTimer(ref m_AsteroidSpawnTimer))
        {
            // Reset the asteroid spawn timer
            float minSpawnTime = CurrentLevelData.AsteroidMinSpawnTime;
            float maxSpawnTime = CurrentLevelData.AsteroidMaxSpawnTime;
            m_AsteroidSpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);

            Asteroid.Size size = Asteroid.Size.Unknown;
            int minSpawnCount = CurrentLevelData.AsteroidMinSpawnCount;
            int maxSpawnCount = CurrentLevelData.AsteroidMaxSpawnCount;
            int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

            for (int i = 0; i < spawnCount; ++i)
            {
                size = Random.Range(0, 5) >= 2 ? Asteroid.Size.Big : Asteroid.Size.Medium; //More likely to spawn a big asteroid
                SpawnAsteroid(size);
            }
        }

        if (HandleSpawnTimer(ref m_EnemySpawnTimer))
        {
            if (CurrentLevelData.EnemyTypes.Count > 0)
            {
                // Reset the enemy spawn timer
                float minSpawnTime = CurrentLevelData.EnemyMinSpawnTime;
                float maxSpawnTime = CurrentLevelData.EnemyMaxSpawnTime;
                m_EnemySpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);

                int minSpawnCount = CurrentLevelData.EnemyMinSpawnCount;
                int maxSpawnCount = CurrentLevelData.EnemyMaxSpawnCount;
                int spawnCount = maxSpawnCount > 1 ? Random.Range(minSpawnCount, maxSpawnCount + 1) : 1;

                int index;
                int health;
                string type;
                for (int i = 0; i < spawnCount; ++i)
                {
                    index = Random.Range(0, CurrentLevelData.EnemyTypes.Count);
                    health = CurrentLevelData.EnemyTypes[index].Health;
                    type = CurrentLevelData.EnemyTypes[index].Type;

                    SpawnEnemy(type, health);
                }
            }
        }
    }

    public void SetLevelData(ref List<LevelData> levelData)
    {
        m_LevelData = levelData;
    }

    public void StartSpawning(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < m_LevelData.Count || levelIndex == -1)
        {
            m_LevelIndex = levelIndex;
            m_CanSpawn = true;

            float minSpawnTime = CurrentLevelData.AsteroidMinSpawnTime;
            float maxSpawnTime = CurrentLevelData.AsteroidMaxSpawnTime;
            m_AsteroidSpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);

            minSpawnTime = CurrentLevelData.EnemyMinSpawnTime;
            maxSpawnTime = CurrentLevelData.EnemyMaxSpawnTime;
            m_EnemySpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);

            foreach (string type in CurrentLevelData.StartingAsteroids)
            {
                if (type == "Big")
                    SpawnAsteroid(Asteroid.Size.Big);
                else if (type == "Medium")
                    SpawnAsteroid(Asteroid.Size.Medium);
            }

            foreach (string type in CurrentLevelData.StartingEnemies)
              SpawnEnemy(type, CurrentLevelData.GetHealthForEnemyType(type));
        }
    }

    public void StopSpawning()
    {
        m_CanSpawn = false;
    }

    public void Reset()
    {
        m_CanSpawn = false;

        boss.gameObject.SetActive(false);
        m_AsteroidPool.GetComponent<ObjectPool>().DeactivateAll();
        m_LaserPool.GetComponent<ObjectPool>().DeactivateAll();
        m_LaserImpactPool.GetComponent<ObjectPool>().DeactivateAll();
        m_ExplosionPool.GetComponent<ObjectPool>().DeactivateAll();
        m_EnemyPool.GetComponent<ObjectPool>().DeactivateAll();
        m_PickupPool.GetComponent<ObjectPool>().DeactivateAll();
        m_ShipWingPool.GetComponent<ObjectPool>().DeactivateAll();

        float minSpawnTime = CurrentLevelData.AsteroidMinSpawnTime;
        float maxSpawnTime = CurrentLevelData.AsteroidMaxSpawnTime;
        m_AsteroidSpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);

        minSpawnTime = CurrentLevelData.EnemyMinSpawnTime;
        maxSpawnTime = CurrentLevelData.EnemyMaxSpawnTime;
        m_EnemySpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    public void ExplodeAllActive()
    {
        // Explode the enemies and the asteroids
        List<GameObject> activeAsteroids = m_AsteroidPool.GetComponent<ObjectPool>().GetActiveObjects();
        foreach (GameObject go in activeAsteroids)
            go.GetComponent<Asteroid>().Explode(true);

        List<GameObject> activeEnemies = m_EnemyPool.GetComponent<ObjectPool>().GetActiveObjects();
        foreach (GameObject go in activeEnemies)
            go.GetComponent<Enemy>().Explode();

        // Deactivate the pickups, lasers and missiles
        List<GameObject> activePickups = m_PickupPool.GetComponent<ObjectPool>().GetActiveObjects();
        foreach (GameObject go in activePickups)
            go.SetActive(false);

        List<GameObject> activeLasers = m_LaserPool.GetComponent<ObjectPool>().GetActiveObjects();
        foreach (GameObject go in activeLasers)
            go.SetActive(false);

        List<GameObject> activeMissiles = m_MissilePool.GetComponent<ObjectPool>().GetActiveObjects();
        foreach (GameObject go in activeMissiles)
            go.SetActive(false);
    }

    public GameObject SpawnBoss()
    {
        if (boss)
            boss.GetComponent<Boss>().Spawn();
        return boss.gameObject;
    }

    public GameObject SpawnAsteroid(Asteroid.Size size)
    {
        float radians = Random.Range(Constants.kAsteroidMinSpawnRadians, Constants.kAsteroidMaxSpawnRadians);
        float minSpeed = CurrentLevelData.AsteroidMinSpeed;
        float maxSpeed = CurrentLevelData.AsteroidMaxSpeed;
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector2 linearVelocity = new Vector2(Mathf.Cos(radians) * speed, Mathf.Sin(radians) * speed);
        Vector2 position = new Vector2(m_ScreenBounds.x + Constants.kOffScreenSpawnBuffer, Random.Range(0.0f, m_ScreenBounds.y));
        return SpawnAsteroid(position, linearVelocity, size);
    }

    public GameObject SpawnAsteroid(Vector2 position, Vector2 linearVelocity, Asteroid.Size size)
    {
        GameObject go = m_AsteroidPool.GetComponent<ObjectPool>().GetObjectFromPool();
        if (go)
            go.GetComponent<Asteroid>().Spawn(position, linearVelocity, size);
        return go;
    }

    public GameObject SpawnEnemy(Enemy.Type type, int health)
    {
        float radians = Random.Range(Constants.kEnemyMinSpawnAngle, Constants.kEnemyMaxSpawnAngle);
        float minSpeed = CurrentLevelData.EnemyMinSpeed;
        float maxSpeed = CurrentLevelData.EnemyMaxSpeed;
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector2 linearVelocity = new Vector2(Mathf.Cos(radians) * speed, Mathf.Sin(radians) * speed);
        Vector2 position = new Vector2(screenBounds.x + Constants.kOffScreenSpawnBuffer, Random.Range(0.0f, screenBounds.y));
        return SpawnEnemy(position, linearVelocity, type, health);
    }

    public GameObject SpawnEnemy(Vector2 position, Vector2 linearVelocity, Enemy.Type type, int health)
    {
        GameObject go = m_EnemyPool.GetComponent<ObjectPool>().GetObjectFromPool();
        if (go)
            go.GetComponent<Enemy>().Spawn(position, linearVelocity, type, health);
        return go;
    }

    public GameObject SpawnMissile(Vector2 position, Vector2 linearVelocity, Missile.Size size)
    {
        GameObject go = m_MissilePool.GetComponent<ObjectPool>().GetObjectFromPool();
        if (go)
            go.GetComponent<Missile>().Spawn(position, linearVelocity, size);
        return go;
    }

    public GameObject SpawnBlueLaser(Vector2 position, Vector2 linearVelocity)
    {
        return SpawnLaser(position, linearVelocity, Laser.Color.Blue);
    }

    public GameObject SpawnRedLaser(Vector2 position, Vector2 linearVelocity)
    {
        return SpawnLaser(position, linearVelocity, Laser.Color.Red);
    }

    private GameObject SpawnLaser(Vector2 position, Vector2 linearVelocity, Laser.Color color)
    {
        GameObject go = m_LaserPool.GetComponent<ObjectPool>().GetObjectFromPool();
        if (go)
            go.GetComponent<Laser>().Spawn(position, linearVelocity, color);
        return go;
    }

    public GameObject SpawnLaserImpact(Vector2 position, Laser.Color color)
    {
        GameObject go = m_LaserImpactPool.GetComponent<ObjectPool>().GetObjectFromPool();
        if (go)
            go.GetComponent<LaserImpact>().Spawn(position, color);
        return go;
    }

    public GameObject SpawnExplosion(Vector2 position, Vector2 scale, float delay = 0.0f)
    {
        GameObject go = m_ExplosionPool.GetComponent<ObjectPool>().GetObjectFromPool();
        if (go)
            go.GetComponent<Explosion>().Spawn(position, scale, delay);
        return go;
    }

    public GameObject SpawnPickup(Vector2 position, Vector2 linearVelocity, Pickup.Type type)
    {
        GameObject go = m_PickupPool.GetComponent<ObjectPool>().GetObjectFromPool();
        if (go)
            go.GetComponent<Pickup>().Spawn(position, linearVelocity, type);
        return go;
    }

    public GameObject SpawnBossWing(Vector2 initialPosition, float initialRadians, float offsetRadians, string texture)
    {
        float radians = initialRadians + offsetRadians; 
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Vector2 position = initialPosition + direction * Constants.kBossWingExplosionOffset;
        Vector2 linearVelocity = direction * Constants.kBossWingExplosionSpeed;

        return SpawnWing(position, linearVelocity, initialRadians, texture);
    }

    public GameObject SpawnWing(Vector2 initialPosition, float initialRadians, string texture, bool isLeft)
    {
        float speed = Random.Range(Constants.kShipWingExplosionMinSpeed, Constants.kShipWingExplosionMaxSpeed);
        float halfPI = Mathf.PI * 0.5f;
        float radians = initialRadians + (isLeft ? halfPI : -halfPI); // Perpendicular to the current direction
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        Vector2 position = initialPosition + direction * Constants.kShipWingExplosionOffset;
        Vector2 linearVelocity = direction * speed;

        return SpawnWing(position, linearVelocity, initialRadians, texture);
    }

    private GameObject SpawnWing(Vector2 initialPosition, Vector2 linearVelocity, float initialRadians, string texture)
    {
        GameObject go = m_ShipWingPool.GetComponent<ObjectPool>().GetObjectFromPool();
        if (go)
            go.GetComponent<ShipWing>().Spawn(initialPosition, linearVelocity, initialRadians, texture);
        return go;
    }

    private void SpawnEnemy(string type, int health)
    {
        if (type == "Enemy-1")
            SpawnEnemy(Enemy.Type.One, health);
        else if (type == "Enemy-2")
            SpawnEnemy(Enemy.Type.Two, health);
        else if (type == "Enemy-3")
            SpawnEnemy(Enemy.Type.Three, health);
        else if (type == "Enemy-4")
            SpawnEnemy(Enemy.Type.Four, health);
        else if (type == "Enemy-5")
            SpawnEnemy(Enemy.Type.Five, health);
        else if (type == "Boss")
            SpawnBoss();
    }

    private LevelData CurrentLevelData
    {
        get
        {   if (m_LevelIndex == -1)
                return m_DefaultLevel;
            return m_LevelData[m_LevelIndex];
        }
    }

    private bool HandleSpawnTimer(ref float timer)
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                return true;
            }
        }
        return false;
    }
}
