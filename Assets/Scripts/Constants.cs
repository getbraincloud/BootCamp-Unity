// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    // BrainCloud constants
    public const string kBrainCloudMainLeaderboardID = "Main";
    public const int kBrainCloudDefaultMinHighScoreIndex = 0;
    public const int kBrainCloudDefaultMaxHighScoreIndex = 9;
    public const string kBrainCloudGlobalEntityIndexedID = "Level";
    public const string kBrainCloudStatGamesPlayed = "GamesCompleted";
    public const string kBrainCloudStatEnemiesKilled = "EnemiesKilled";
    public const string kBrainCloudStatAsteroidDestroyed = "AsteroidsDestroyed";

    public static readonly Dictionary<string, string> kBrainCloudStatDescriptions = new Dictionary<string, string> {{kBrainCloudStatGamesPlayed, "Games completed"},
                                                                                                                    {kBrainCloudStatEnemiesKilled, "Enemies killed"},
                                                                                                                    {kBrainCloudStatAsteroidDestroyed, "Asteroids destroyed"}};


    // General constants
    public const float kOffScreenSpawnBuffer = 128.0f;
    public const float kBackgroundSpeed = 250.0f;
    public const float kEndOfGameDisplayDuration = 2.5f;
    public const float kLevelDisplayDuration = 2.5f;
    public const int kHordeModeLevelOne = 0;
    public const int kHordeModeLevelTwo = 1;
    public const int kHordeModeLevelThree = 2;
    public const int kHordeModeLevelBoss = 3;
    public const float kConnectingDialogDotsInterval = 0.75f;


    // Object pool constants
    public const int kAsteroidPoolSize = 50;
    public const int kEnemyPoolSize = 18;
    public const int kMissilePoolSize = 20;
    public const int kLaserPoolSize = 50;
    public const int kLaserImpactPoolSize = kLaserPoolSize;
    public const int kExplosionPoolSize = 15;
    public const int kShipWingPoolSize = (kEnemyPoolSize + 1) * 2;
    public const int kPickupPoolSize = 4;


    // Unity layer tags
    public const int kShipLayer = 6;
    public const int kAsteroidLayer = 7;
    public const int kEnemyLayer = 8;
    public const int kBossLayer = 9;
    public const int kLaserLayer = 10;
    public const int kMissileLayer = 11;
    public const int kPickupLayer = 12;


    // HUD constants
    public const int kHudHeight = 48;
    public const float kHudStatusBarInset = 5.0f;
    public const float kHudSmallBuffer = 2.0f;
    public const float kHudBigBuffer = 6.0f;
    public const float kHudTextInset = kHudBigBuffer * 4.0f;
    public const float kHudDangerFlashIncrement = Mathf.PI;
    public const float kHudHighScoreScissorWidth = 280.0f;
    public const float kHudHighScoreMovementSpeed = 150.0f;


    // Ship constants
    public const int kShipInitialHealth = 4;
    public const int kShipInitialShieldHealth = 4;
    public const float kShipInvincibilityDuration = 0.75f;
    public const float kShipInvincibleAlpha = 0.65f;
    public const float kShipOffScreenSpawnX = -100.0f;
    public const float kShipSpawnX = 100.0f;
    public const float kShipAcceleration = 30.0f;
    public const float kShipDrag = 0.8f;
    public const float kShipMaxSpeed = 900.0f;
    public const float kShipMinX = 100.0f;
    public const float kShipMaxOffsetX = 200.0f;
    public const float kShipTurnTilt = 0.34906f;        // 20 degrees
    public const float kShipGunOffset = 10.0f;
    public const float kShipGunAngleTilt = 0.0209436f;  // 1.2 degrees
    public const float kShipCircleCollisionRadius = 20.0f;
    public const float kShipEdgeCollisionAngle = 2.094395f;  // 120 degrees
    public const float kShipEdgeCollisionMagnitude = 42.0f;


    // Asteroid constants
    public const float kAsteroidMinSpawnRadians = 2.792f;  // 160 degrees
    public const float kAsteroidMaxSpawnRadians = 3.49f;   // 200 degrees
    public const float kAsteroidMinSpeed = 100.0f;
    public const float kAsteroidMaxSpeed = 300.0f;
    public const float kAsteroidExplosionMinSpeed = 150.0f;
    public const float kAsteroidExplosionMaxSpeed = 325.0f;
    public const float kAsteroidExplosionFadeOutTime = 1.0f;
    public const float kAsteroidExplosionFadeDelay = 0.25f;
    public const float kAsteroidMinAngularVelocity = Mathf.PI * 0.5f;               // 90 degrees per second
    public const float kAsteroidMaxAngularVelocity = Mathf.PI + Mathf.PI * 0.5f;    // 270 degrees per second
    public const float kAsteroidMinSpawnTime = 0.8f;
    public const float kAsteroidMaxSpawnTime = 2.5f;
    public const int kAsteroidMinSpawnCount = 1;
    public const int kAsteroidMaxSpawnCount = 5;
    public const int kNumAsteroidSizes = 4;
    public static readonly int[] kNumAsteroidVariations = { 3, 2, 2, 2 };
    public static readonly string[] kBigAsteroidAtlasKeys = { "AsteroidBig-1", "AsteroidBig-2", "AsteroidBig-3" };
    public static readonly string[] kMediumAsteroidAtlasKeys = { "AsteroidMedium-1", "AsteroidMedium-2" };
    public static readonly string[] kSmallAsteroidAtlasKeys = { "AsteroidSmall-1", "AsteroidSmall-2" };
    public static readonly string[] kTinyAsteroidAtlasKeys = { "AsteroidTiny-1", "AsteroidTiny-2" };
    public static readonly int[] kAsteroidHealth = { 2, 1, 0, 0};
    public static readonly int[] kAsteroidAttackDamage = { 2, 1, 0, 0};


    // Ship wing constants
    public const float kShipWingExplosionMinSpeed = 175.0f;
    public const float kShipWingExplosionMaxSpeed = 250.0f;
    public const float kShipWingExplosionFadeOutTime = 1.0f;
    public const float kShipWingExplosionOffset = 5.0f;
    public const float kShipWingMinAngularVelocity = Mathf.PI * 0.25f;         // 45 degrees per second
    public const float kShipWingMaxAngularVelocity = Mathf.PI * 0.5f;  // 90 degrees per second


    // Laser constants
    public const float kLaserSpeed = 1200.0f;
    public const float kLaserImpactLifetime = 0.05f;


    // Missile constants
    public const float kMissileSmallSpeed = 1000.0f;
    public const float kMissileBigSpeed = 750.0f;
    public const int kMissileSmallAttackDamage = 2;
    public const int kMissileBigAttackDamage = 4;


    // Enemy constants
    public const int kEnemyMinSpawnCount = 1;
    public const int kEnemyMaxSpawnCount = 3;
    public const float kEnemyMinSpeed = 200.0f;
    public const float kEnemyMaxSpeed = 300.0f;
    public const float kEnemyMinSpawnAngle = 2.96701f;  // 170 Degrees;
    public const float kEnemyMaxSpawnAngle = 3.31607f;  // 190 Degrees;
    public const float kEnemyLaserMinDelay = 0.75f;
    public const float kEnemyLaserMaxDelay = 2.0f;
    public const float kEnemyMinSpawnTime = 1.5f;
    public const float kEnemyMaxSpawnTime = 3.5f;
    public const float kEnemyThreeFireDelay = 0.35f;
    public const float kEnemyFourFireDelay = 0.1f;
    public const float kEnemyFourFiringDuration = 1.25f;
    public const float kEnemyFourFiringCooldown = 0.5f;
    public const float kEnemyFiveFiringMinCooldown = 1.75f;
    public const float kEnemyFiveFiringMaxCooldown = 3.0f;
    public const float kEnemyFiveMissileMinDelay = 0.8f;
    public const float kEnemyFiveMissileMaxDelay = 1.4f;
    public const float kEnemyFiveLaserMinDelay = 0.4f;
    public const float kEnemyFiveLaserMaxDelay = 1.2f;
    public const int kNumEnemyTypes = 5;
    public static readonly string[] kEnemyAtlasKeys = { "EnemyShip-1", "EnemyShip-2", "EnemyShip-3", "EnemyShip-4", "EnemyShip-5" };
    public static readonly string[] kEnemyWingLeftAtlasKeys = { "EnemyShipLeftWing-1", "EnemyShipLeftWing-2", "EnemyShipLeftWing-3", "EnemyShipLeftWing-4", "EnemyShipLeftWing-5" };
    public static readonly string[] kEnemyWingRightAtlasKeys = { "EnemyShipRightWing-1", "EnemyShipRightWing-2", "EnemyShipRightWing-3", "EnemyShipRightWing-4", "EnemyShipRightWing-5" };
    public const float kEnemyGunOffset1 = 16.0f;
    public const float kEnemyGunOffset2 = 6.0f;
    public const float kEnemyGunOffset3 = 18.0f;
    public const float kEnemyGunOffset4_1 = 21.0f;
    public const float kEnemyGunOffset4_2 = 27.0f;
    public const float kEnemyGunOffset5 = 25.0f;
    public const int kEnemyInitialHealth1 = 2;
    public const int kEnemyInitialHealth2 = 3;


    // Boss constants
    public const string kBossFrontLeftWingAtlasKey = "BossFrontLeftWing";
    public const string kBossFrontRightWingAtlasKey = "BossFrontRightWing";
    public const string kBossMiddleLeftWingAtlasKey = "BossMiddleLeftWing";
    public const string kBossMiddleRightWingAtlasKey = "BossMiddleRightWing";
    public const string kBossBackLeftWingAtlasKey = "BossBackLeftWing";
    public const string kBossBackRightWingAtlasKey = "BossBackRightWing";
    public const float kBossWingExplosionSpeed = 500.0f;
    public const float kBossWingExplosionOffset = 40.0f;
    public const float kBossOffScreenSpawnX = 1450.0f;
    public const float kBossSpawnX = 950.0f;
    public const float kBossMinX = 450.0f;
    public const float kBossMovementMinDelay = 0.05f;
    public const float kBossMovementMaxDelay = 0.2f;
    public const float kBossMovementMinRange = 200.0f;
    public const float kBossMovementMaxRange = 600.0f;
    public const float kBossSmallMissileMinDelay = 0.6f;
    public const float kBossSmallMissileMaxDelay = 1.2f;
    public const float kBossBigMissileMinDelay = 1.2f;
    public const float kBossBigMissileMaxDelay = 2.0f;
    public const float kBossSpeed = 450.0f;
    public const int kBossHealth = 40;
    public const int kBossAttackDamage = 10;
    public const float kBossGunOffset1 = 64.0f;
    public const float kBossGunOffset2 = 56.0f;


    // Pickup constants
    public const float kPickupLifetime = 5.0f;
    public const float kPickupFadeOutTime = 0.5f;
}

