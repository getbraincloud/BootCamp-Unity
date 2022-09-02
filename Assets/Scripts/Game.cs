// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Ship ship;
    [SerializeField] private Boss boss;
    [SerializeField] private HeadsUpDisplay hud;

    private List<float> m_LevelDurations;
    private List<string> m_LevelDescriptions;
    private int m_LevelCount = 0;
    private float m_ElapsedTime;
    private float m_LevelIndicatorDisplayTime;
    private float m_EndOfGameDisplayTime;
    private int m_LevelIndex = -1;
    private int m_LeaderboardEntryIndex = -1;
    private UserData m_UserData;


    private enum GameState
    {
        Authenticating,
        LoadingData,
        Gameplay,
        LevelTransition,
        GameOver,
        Victory
    }

    public enum Mode
    {
        Unknown = -1,
        Endless,
        Horde
    }

    Mode m_Mode;

    private GameState m_GameState = GameState.Authenticating;

    public static Game sharedInstance;

    private void Awake()
    {
        sharedInstance = this;

        m_UserData = new UserData();

        ship.SetDelegate(OnShipHasExploded);
        ship.gameObject.SetActive(false);

        boss.SetDelegate(OnBossHasExploded);
        boss.gameObject.SetActive(false);

        m_LevelDurations = new List<float>();
        m_LevelDescriptions = new List<string>();

        HandleAuthentication();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_GameState == GameState.GameOver)
        {
            if (m_EndOfGameDisplayTime > 0.0f)
            {
                m_EndOfGameDisplayTime -= Time.deltaTime;
                if (m_EndOfGameDisplayTime < 0.0f)
                {
                    if(m_GameState == GameState.GameOver)
                        hud.HideGameOver();
                    if (m_GameState == GameState.Victory)
                        hud.HideGameWon();

                    if (m_Mode == Mode.Endless)
                    {
                        if (Network.sharedInstance.IsUsernameSaved())
                            Network.sharedInstance.PostScoreToLeaderboard(Constants.kBrainCloudMainLeaderboardID, m_ElapsedTime, OnPostScoreRequestCompleted);
                        else
                            DialogManager.sharedInstance.ShowPostScoreDialog(m_ElapsedTime, OnPostScoreRequestCompleted);
                    }
                    else if (m_Mode == Mode.Horde)
                    {
                        DialogManager.sharedInstance.ShowPlayAgainDialog();
                    }
                }
            }
        }
        else if (m_GameState == GameState.Gameplay)
        {
            m_ElapsedTime += Time.deltaTime;
            hud.SetElapsedTime(m_ElapsedTime);

            if (m_Mode == Mode.Endless)
            {
                CheckEndlessModeTimeAchievements();

                if (m_LeaderboardEntryIndex != -1)
                {
                    Leaderboard leaderboard = LeaderboardsManager.sharedInstance.GetLeaderboardByName(Constants.kBrainCloudMainLeaderboardID);
                    if (leaderboard != null)
                    {
                        LeaderboardEntry leaderboardEntry = leaderboard.GetLeaderboardEntryAtIndex(m_LeaderboardEntryIndex);
                        if (leaderboardEntry != null && m_ElapsedTime > leaderboardEntry.Time)
                            DisplayNextHighScore();
                    }
                }
            }
            else if (m_Mode == Mode.Horde)
            {
                if (m_ElapsedTime > m_LevelDurations[m_LevelIndex] && m_LevelDurations[m_LevelIndex] != -1.0f)
                {
                    m_ElapsedTime = m_LevelDurations[m_LevelIndex];
                    NextLevel();
                }
            }
        }
        else if (m_GameState == GameState.LevelTransition)
        {
            if (m_LevelIndicatorDisplayTime > 0.0f)
            {
                m_LevelIndicatorDisplayTime -= Time.deltaTime;

                if (m_LevelIndicatorDisplayTime < 0.0f)
                    StartLevel();
            }
        }
    }

    public void HandleAuthentication()
    {
        m_GameState = GameState.Authenticating;

        if (Network.sharedInstance.HasAuthenticatedPreviously())
        {
            DialogManager.sharedInstance.ShowConnectingDialog();
            Network.sharedInstance.Reconnect(OnAuthenticationRequestCompleted);
        }
        else
        {
            DialogManager.sharedInstance.ShowEmailLoginDialog(OnAuthenticationRequestCompleted);
        }
    }

    public void OnAuthenticationRequestCompleted()
    {
        if (m_GameState == GameState.Authenticating)
        {
            m_GameState = GameState.LoadingData;

            Network.sharedInstance.RequestGlobalEntityLevelData(OnGlobalEntityLevelDataRequestCompleted);
            Network.sharedInstance.RequestLeaderboard(Constants.kBrainCloudMainLeaderboardID, OnLeaderboardRequestCompleted);
            Network.sharedInstance.RequestUserStatistics(OnUserStatisticsRequestCompleted);
            Network.sharedInstance.RequestReadAchievements(OnAchievementRequestCompleted);
            Network.sharedInstance.RequestUserEntityData(OnUserEntityDataRequestCompleted);
        }
    }

    public bool IsGameOver()
    {
        return m_GameState == GameState.GameOver;
    }

    public bool IsGameWon()
    {
        return m_GameState == GameState.Victory;
    }

    public UserData GetUserData()
    {
        return m_UserData;
    }

    public void StartEndlessMode()
    {
        m_Mode = Mode.Endless;
        PrepareLevel(-1); // -1 is the default level used for endless mode
    }

    public void StartHordeMode(int levelIndex = 0)
    {
        m_Mode = Mode.Horde;
        PrepareLevel(levelIndex);
    }

    public void Reset(bool startNewGame)
    {
        Spawner.sharedInstance.Reset();
        ship.Reset();
        hud.Reset();

        if (startNewGame)
        {
            if (m_Mode == Mode.Endless)
                StartEndlessMode();
            else if (m_Mode == Mode.Horde)
                StartHordeMode();
        }
    }

    public int GetShipShieldHealth()
    {
        return ship.GetShield().GetComponent<Shield>().GetHealth();
    }

    public void OnCheatAddTime()
    {
        m_ElapsedTime += 15.0f;
    }

    public void OnCheatNextLevel()
    {
        NextLevel();
    }

    public void OnCheatHealUp()
    {
        ship.HealUp();
    }

    private void PrepareLevel(int levelIndex)
    {
        m_GameState = GameState.LevelTransition;
        m_LevelIndicatorDisplayTime = Constants.kLevelDisplayDuration;
        m_LevelIndex = levelIndex;
        m_ElapsedTime = 0.0f;

        if (m_Mode == Mode.Endless)
        {
            Leaderboard mainLeaderboard = LeaderboardsManager.sharedInstance.GetLeaderboardByName(Constants.kBrainCloudMainLeaderboardID);

            if (mainLeaderboard != null && mainLeaderboard.GetCount() > 0)
            {
                    m_LeaderboardEntryIndex = mainLeaderboard.GetCount() - 1;
                    hud.PushLeaderboardEntry(mainLeaderboard.GetLeaderboardEntryAtIndex(m_LeaderboardEntryIndex));
            }

            hud.ShowLevel(-1, "");
        }
        else if (m_Mode == Mode.Horde)
        {
            hud.ShowLevel(m_LevelIndex + 1, m_LevelDescriptions[m_LevelIndex]);
            hud.PushLevelGoal(m_LevelDescriptions[m_LevelIndex]);
        }
    }

    private void StartLevel()
    {
        m_GameState = GameState.Gameplay;

        hud.HideLevel();

        Spawner.sharedInstance.StartSpawning(m_LevelIndex);

        if (!ship.gameObject.activeInHierarchy)
        {
            ship.gameObject.SetActive(true);
            ship.Spawn();
        }

        return;
    }

    private void NextLevel()
    {
        Spawner.sharedInstance.StopSpawning();
        Spawner.sharedInstance.ExplodeAllActive();

        Achievement beatLevel1 = AchievementManager.sharedInstance.GetAchievementByID(Constants.kBrainCloudAchievementBeatLevel1);
        if (beatLevel1 != null && !beatLevel1.IsAwarded && m_LevelIndex == 0)
        {
            beatLevel1.Award();
            hud.PushAchievement(beatLevel1);
        }

        bool needsSync = m_UserData.SetLevelCompleted(m_LevelIndex);

        if (needsSync)
            Network.sharedInstance.UpdateUserEntityData(m_UserData.EntityID, m_UserData.EntityType, m_UserData.JsonData);

        if (m_LevelIndex + 1 < m_LevelCount)
        {
            ship.HealUp();
            PrepareLevel(m_LevelIndex + 1);
        }
    }

    private void DisplayNextHighScore()
    {
        Leaderboard leaderboard = LeaderboardsManager.sharedInstance.GetLeaderboardByName(Constants.kBrainCloudMainLeaderboardID);

        if (m_LeaderboardEntryIndex == -1 || leaderboard == null || leaderboard.GetCount() == 0)
            return;

        if (m_LeaderboardEntryIndex > 0)
        {
            m_LeaderboardEntryIndex--;
            hud.PushLeaderboardEntry(leaderboard.GetLeaderboardEntryAtIndex(m_LeaderboardEntryIndex));
        }
        else if (m_LeaderboardEntryIndex == 0)
        {
            hud.SetPlayerHasAllTimeHighScore();
        }
    }

    private void CheckEndlessModeTimeAchievements()
    {
        Achievement survive30 = AchievementManager.sharedInstance.GetAchievementByID(Constants.kBrainCloudAchievementSurvive30);
        if (survive30 != null && !survive30.IsAwarded && m_ElapsedTime >= 30.0f)
        {
            survive30.Award();
            hud.PushAchievement(survive30);
        }

        Achievement survive60 = AchievementManager.sharedInstance.GetAchievementByID(Constants.kBrainCloudAchievementSurvive60);
        if (survive60 != null && !survive60.IsAwarded && m_ElapsedTime >= 60.0f)
        {
            survive60.Award();
            hud.PushAchievement(survive60);
        }
    }

    private void OnShipHasExploded()
    {
        m_GameState = GameState.GameOver;
        m_EndOfGameDisplayTime = Constants.kEndOfGameDisplayDuration;
        hud.ShowGameOver();

        // Update the statistics
        Statistic gamesPlayed = StatisticsManager.sharedInstance.GetStatisticByName(Constants.kBrainCloudStatGamesPlayed);
        if (gamesPlayed != null)
            gamesPlayed.ApplyIncrement();

        // Send all the statistics increments to brainCloud now that the game has ended
        Dictionary<string, object> dictionary = StatisticsManager.sharedInstance.GetIncrementsDictionary();
        if(dictionary != null)
            Network.sharedInstance.IncrementUserStatistics(dictionary, OnIncrementUserStatisticsCompleted);
    }

    private void OnBossHasExploded()
    {
        m_GameState = GameState.Victory;
        m_EndOfGameDisplayTime = Constants.kEndOfGameDisplayDuration;
        hud.ShowGameWon();

        Spawner.sharedInstance.StopSpawning();
        Spawner.sharedInstance.ExplodeAllActive();

        // Update the statistics
        Statistic gamesPlayed = StatisticsManager.sharedInstance.GetStatisticByName(Constants.kBrainCloudStatGamesPlayed);
        if (gamesPlayed != null)
            gamesPlayed.ApplyIncrement();

        // Send all the statistics increments to brainCloud now that the game has ended
        Dictionary<string, object> dictionary = StatisticsManager.sharedInstance.GetIncrementsDictionary();
        if (dictionary != null)
            Network.sharedInstance.IncrementUserStatistics(dictionary, OnIncrementUserStatisticsCompleted);

        // The user beat the boss level, update the user's progress
        bool needsSync = m_UserData.SetLevelCompleted(m_LevelIndex);
        if (needsSync)
            Network.sharedInstance.UpdateUserEntityData(m_UserData.EntityID, m_UserData.EntityType, m_UserData.JsonData);
    }

    private void OnLeaderboardRequestCompleted(Leaderboard leaderboard)
    {
        LeaderboardsManager.sharedInstance.AddLeaderboard(leaderboard);

        if (IsGameOver())
        {
            if (m_Mode == Mode.Endless && leaderboard.Name == Constants.kBrainCloudMainLeaderboardID)
            {
                DialogManager.sharedInstance.ShowPlayAgainDialog();
                DialogManager.sharedInstance.ShowLeaderboardsDialog();
            }
        }
    }

    private void OnPostScoreRequestCompleted()
    {
        LeaderboardsManager.sharedInstance.SetUserTime(m_ElapsedTime);
        Network.sharedInstance.RequestLeaderboard(Constants.kBrainCloudMainLeaderboardID, OnLeaderboardRequestCompleted);
    }

    private void OnGlobalEntityLevelDataRequestCompleted(ref List<LevelData> levelData)
    {
        Spawner.sharedInstance.SetLevelData(ref levelData);

        m_LevelCount = levelData.Count;

        foreach (LevelData ld in levelData)
        {
            m_LevelDurations.Add(ld.Duration);
            m_LevelDescriptions.Add(ld.Description);
        }

        DialogManager.sharedInstance.HideConnectingDialog();
        DialogManager.sharedInstance.ShowMainMenuDialog();
    }

    private void OnUserStatisticsRequestCompleted(ref List<Statistic> statistics)
    {
        StatisticsManager.sharedInstance.SetStatistics(ref statistics);
    }

    private void OnIncrementUserStatisticsCompleted(ref List<Statistic> statistics)
    {
        StatisticsManager.sharedInstance.SetStatistics(ref statistics);
    }

    private void OnAchievementRequestCompleted(ref List<Achievement> achievements)
    {
        AchievementManager.sharedInstance.SetAchievements(ref achievements);
    }

    private void OnUserEntityDataRequestCompleted(UserData userData)
    {
        if (userData != null)
        {
            m_UserData = userData;
        }
        else
        {
            // User entity for User progress data doesn't exist, create one
            Network.sharedInstance.CreateUserEntityData();
        }
    }
}
