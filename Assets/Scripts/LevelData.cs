// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrainCloud.LitJson;

public class LevelData
{
    public class EnemyType
    {
        string m_Type;
        int m_Health;

        public EnemyType(string type, int health)
        {
            m_Type = type;
            m_Health = health;
        }

        public string Type
        {
            get { return m_Type; }
        }

        public int Health
        {
            get { return m_Health; }
        }
    }

    private List<EnemyType> m_EnemyTypes;
    private List<string> m_StartingAsteroids;
    private List<string> m_StartingEnemies;
    private string m_EntityType;
    private string m_EntityID;
    private string m_Description;
    private float m_Duration;
    private int m_AsteroidMinSpawnCount;
    private int m_AsteroidMaxSpawnCount;
    private float m_AsteroidMinSpawnTime;
    private float m_AsteroidMaxSpawnTime;
    private float m_AsteroidMinSpeed;
    private float m_AsteroidMaxSpeed;
    private int m_EnemyMinSpawnCount;
    private int m_EnemyMaxSpawnCount;
    private float m_EnemyMinSpawnTime;
    private float m_EnemyMaxSpawnTime;
    private float m_EnemyMinSpeed;
    private float m_EnemyMaxSpeed;

    public LevelData()
    {
        m_EntityType = "Default";
        m_EntityID = "-1";
        m_EnemyTypes = new List<EnemyType>();
        m_StartingAsteroids = new List<string>();
        m_StartingEnemies = new List<string>();

        m_Duration = -1.0f;
        m_Description = "Survive as long as possible";
        m_AsteroidMinSpawnCount = Constants.kAsteroidMinSpawnCount;
        m_AsteroidMaxSpawnCount = Constants.kAsteroidMaxSpawnCount;
        m_AsteroidMinSpawnTime = Constants.kAsteroidMinSpawnTime;
        m_AsteroidMaxSpawnTime = Constants.kAsteroidMaxSpawnTime;
        m_AsteroidMinSpeed = Constants.kAsteroidMinSpeed;
        m_AsteroidMaxSpeed = Constants.kAsteroidMaxSpeed;

        m_EnemyMinSpawnCount = Constants.kEnemyMinSpawnCount;
        m_EnemyMaxSpawnCount = Constants.kEnemyMaxSpawnCount;
        m_EnemyMinSpawnTime = Constants.kEnemyMinSpawnTime;
        m_EnemyMaxSpawnTime = Constants.kEnemyMaxSpawnTime;
        m_EnemyMinSpeed = Constants.kEnemyMinSpeed;
        m_EnemyMaxSpeed = Constants.kEnemyMaxSpeed;

        m_EnemyTypes.Add(new EnemyType("Enemy-1", 2));
        m_EnemyTypes.Add(new EnemyType("Enemy-2", 3));
        m_EnemyTypes.Add(new EnemyType("Enemy-3", 3));
        m_EnemyTypes.Add(new EnemyType("Enemy-4", 4));
        m_EnemyTypes.Add(new EnemyType("Enemy-5", 4));

        m_StartingAsteroids.Add("Big");
        m_StartingAsteroids.Add("Big");
        m_StartingAsteroids.Add("Medium");
    }

    public int GetHealthForEnemyType(string enemyType)
    {
        foreach (EnemyType et in m_EnemyTypes)
            if (et.Type == enemyType)
                return et.Health;

        return 1;
    }

    public string EntityType
    {
        get { return m_EntityType; }
    }

    public string EntityID
    {
        get { return m_EntityID; }
    }

    public string Description
    {
        get { return m_Description; }
    }

    public float Duration
    {
        get { return m_Duration; }
    }

    public int AsteroidMinSpawnCount
    {
        get { return m_AsteroidMinSpawnCount; }
    }

    public int AsteroidMaxSpawnCount
    {
        get { return m_AsteroidMaxSpawnCount; }
    }

    public float AsteroidMinSpawnTime
    {
        get { return m_AsteroidMinSpawnTime; }
    }

    public float AsteroidMaxSpawnTime
    {
        get { return m_AsteroidMaxSpawnTime; }
    }

    public float AsteroidMinSpeed
    {
        get { return m_AsteroidMinSpeed; }
    }

    public float AsteroidMaxSpeed
    {
        get { return m_AsteroidMaxSpeed; }
    }

    public int EnemyMinSpawnCount
    {
        get { return m_EnemyMinSpawnCount; }
    }

    public int EnemyMaxSpawnCount
    {
        get { return m_EnemyMaxSpawnCount; }
    }

    public float EnemyMinSpawnTime
    {
        get { return m_EnemyMinSpawnTime; }
    }

    public float EnemyMaxSpawnTime
    {
        get { return m_EnemyMaxSpawnTime; }
    }

    public float EnemyMinSpeed
    {
        get { return m_EnemyMinSpeed; }
    }

    public float EnemyMaxSpeed
    {
        get { return m_EnemyMaxSpeed; }
    }

    public List<EnemyType> EnemyTypes
    {
        get { return m_EnemyTypes; }
    }

    public List<string> StartingAsteroids
    {
        get { return m_StartingAsteroids; }
    }

    public List<string> StartingEnemies
    {
        get { return m_StartingEnemies; }
    }
}
