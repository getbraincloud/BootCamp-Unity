// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BrainCloud.LitJson;


public class Network : MonoBehaviour
{
    public delegate void AuthenticationRequestCompleted();
    public delegate void AuthenticationRequestFailed();
    public delegate void BrainCloudLogOutCompleted();
    public delegate void BrainCloudLogOutFailed();
    public delegate void UpdateUsernameRequestCompleted();
    public delegate void UpdateUsernameRequestFailed();
    public delegate void LeaderboardRequestCompleted(Leaderboard leaderboard);
    public delegate void LeaderboardRequestFailed();
    public delegate void PostScoreRequestCompleted();
    public delegate void PostScoreRequestFailed();
    public delegate void RequestGlobalEntityLevelDataCompleted(ref List<LevelData> levels);
    public delegate void RequestGlobalEntityLevelDataFailed();
    public delegate void UserStatisticsRequestCompleted(ref List<Statistic> statistics);
    public delegate void UserStatisticsRequestFailed();
    public delegate void IncrementUserStatisticsCompleted(ref List<Statistic> statistics);
    public delegate void IncrementUserStatisticsFailed();
    public delegate void ReadAchievementRequestCompleted(ref List<Achievement> achievements);
    public delegate void ReadAchievementRequestFailed();

    public static Network sharedInstance;

    private BrainCloudWrapper m_BrainCloud;
    private string m_Username;


    private void Awake()
    {
        sharedInstance = this;

        DontDestroyOnLoad(gameObject);

        // Create and initialize the BrainCloud wrapper
        m_BrainCloud = gameObject.AddComponent<BrainCloudWrapper>();
        m_BrainCloud.Init();

        // Log the BrainCloud client version
        Debug.Log("BrainCloud client version: " + m_BrainCloud.Client.BrainCloudClientVersion);
    }

    void Update()
    {
        // Make sure you invoke this method in Update, or else you won't get any callbacks
        m_BrainCloud.RunCallbacks();
    }

    public bool HasAuthenticatedPreviously()
    {
        return m_BrainCloud.GetStoredProfileId() != "" && m_BrainCloud.GetStoredAnonymousId() != "";
    }

    public bool IsAuthenticated()
    {
        return m_BrainCloud.Client.Authenticated;
    }

    public bool IsUsernameSaved()
    {
        return m_Username != "";
    }

    public void ResetStoredProfileId()
    {
        m_BrainCloud.ResetStoredProfileId();
    }

    public void LogOut(BrainCloudLogOutCompleted brainCloudLogOutCompleted = null, BrainCloudLogOutFailed brainCloudLogOutFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("LogOut success: " + responseData);

                m_Username = "";

                // The user logged out, clear the persisted data related to their account
                m_BrainCloud.ResetStoredAnonymousId();
                m_BrainCloud.ResetStoredProfileId();

                if (brainCloudLogOutCompleted != null)
                    brainCloudLogOutCompleted();
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("BrainCloud Logout failed: " + statusMessage);

                if (brainCloudLogOutFailed != null)
                    brainCloudLogOutFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.PlayerStateService.Logout(successCallback, failureCallback);
        }
        else
        {
            Debug.Log("BrainCloud Logout failed: user is not authenticated");

            if (brainCloudLogOutFailed != null)
                brainCloudLogOutFailed();
        }
    }

    public void Reconnect(AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        // Success callback lambda
        BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
        {
            Debug.Log("Reconnect authentication success: " + responseData);
            HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
        };

        // Failure callback lambda
        BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
        {
            Debug.Log("Reconnect authentication failed. " + statusMessage);

            if (authenticationRequestFailed != null)
                authenticationRequestFailed();
        };

        // Make the BrainCloud request
        m_BrainCloud.Reconnect(successCallback, failureCallback);
    }

    public void RequestAnonymousAuthentication(AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        // Success callback lambda
        BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
        {
            Debug.Log("Anonymous authentication success: " + responseData);
            HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
        };

        // Failure callback lambda
        BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
        {
            Debug.Log("Anonymous authentication failed. " + statusMessage);

            if (authenticationRequestFailed != null)
                authenticationRequestFailed();
        };

        // Make the BrainCloud request
        m_BrainCloud.AuthenticateAnonymous(successCallback, failureCallback);
    }

    public void RequestAuthenticationEmail(string email, string password, AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        // Success callback lambda
        BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
        {
            Debug.Log("Email authentication success: " + responseData);
            HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
        };

        // Failure callback lambda
        BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
        {
            Debug.Log("Email authentication failed. " + statusMessage);

            if (authenticationRequestFailed != null)
                authenticationRequestFailed();
        };

        // Make the BrainCloud request
        m_BrainCloud.AuthenticateEmailPassword(email, password, true, successCallback, failureCallback);
    }

    public void RequestAuthenticationUniversal(string userID, string password, AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        // Success callback lambda
        BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
        {
            Debug.Log("Universal authentication success: " + responseData);
            HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
        };

        // Failure callback lambda
        BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
        {
            Debug.Log("Universal authentication failed. " + statusMessage);

            if (authenticationRequestFailed != null)
                authenticationRequestFailed();
        };

        // Make the BrainCloud request
        m_BrainCloud.AuthenticateUniversal(userID, password, true, successCallback, failureCallback);
    }

    public void RequestUpdateUsername(string username, UpdateUsernameRequestCompleted updateUsernameRequestCompleted = null, UpdateUsernameRequestFailed updateUsernameRequestFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("RequestUpdateUsername success: " + responseData);

                JsonData jsonData = JsonMapper.ToObject(responseData);
                m_Username = jsonData["data"]["playerName"].ToString();

                if (updateUsernameRequestCompleted != null)
                    updateUsernameRequestCompleted();
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("RequestUpdateUsername failed: " + statusMessage);

                if (updateUsernameRequestFailed != null)
                    updateUsernameRequestFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.PlayerStateService.UpdateName(username, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("RequestUpdateUsername failed: user is not authenticated");

            if (updateUsernameRequestFailed != null)
                updateUsernameRequestFailed();
        }
    }

    public void RequestLeaderboard(string leaderboardId, LeaderboardRequestCompleted leaderboardRequestCompleted = null, LeaderboardRequestFailed leaderboardRequestFailed = null)
    {
        RequestLeaderboard(leaderboardId, Constants.kBrainCloudDefaultMinHighScoreIndex, Constants.kBrainCloudDefaultMaxHighScoreIndex, leaderboardRequestCompleted, leaderboardRequestFailed);
    }

    public void RequestLeaderboard(string leaderboardId, int startIndex, int endIndex, LeaderboardRequestCompleted leaderboardRequestCompleted = null, LeaderboardRequestFailed leaderboardRequestFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("RequestMainHighScores success: " + responseData);

                // Read the json and update our values
                JsonData jsonData = JsonMapper.ToObject(responseData);
                JsonData leaderboard = jsonData["data"]["leaderboard"];

                List<LeaderboardEntry> leaderboardEntriesList = new List<LeaderboardEntry>();
                int rank = 0;
                string nickname;
                long ms = 0;
                float time = 0.0f;

                if (leaderboard.IsArray)
                {
                    for (int i = 0; i < leaderboard.Count; i++)
                    {
                        rank = int.Parse(leaderboard[i]["rank"].ToString());
                        nickname = leaderboard[i]["data"]["nickname"].ToString();
                        ms = long.Parse(leaderboard[i]["score"].ToString());
                        time = (float)ms / 1000.0f;

                        leaderboardEntriesList.Add(new LeaderboardEntry(nickname, rank, time));
                    }
                }

                Leaderboard lb = new Leaderboard(leaderboardId, leaderboardEntriesList);

                if (leaderboardRequestCompleted != null)
                    leaderboardRequestCompleted(lb);
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("RequestMainHighScores failed: " + statusMessage);

                if (leaderboardRequestFailed != null)
                    leaderboardRequestFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.LeaderboardService.GetGlobalLeaderboardPage(leaderboardId, BrainCloud.BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW, startIndex, endIndex, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("RequestMainHighScores failed: user is not authenticated");

            if (leaderboardRequestFailed != null)
                leaderboardRequestFailed();
        }
    }

    public void PostScoreToLeaderboard(string leaderboardID, float time, PostScoreRequestCompleted postScoreRequestCompleted = null, PostScoreRequestFailed postScoreRequestFailed = null)
    {
        PostScoreToLeaderboard(leaderboardID, time, m_Username, postScoreRequestCompleted, postScoreRequestFailed);
    }

    public void PostScoreToLeaderboard(string leaderboardID, float time, string nickname, PostScoreRequestCompleted postScoreRequestCompleted = null, PostScoreRequestFailed postScoreRequestFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("PostScoreToLeaderboard success: " + responseData);

                if (postScoreRequestCompleted != null)
                    postScoreRequestCompleted();
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("PostScoreToLeaderboard failed: " + statusMessage);

                if (postScoreRequestFailed != null)
                    postScoreRequestFailed();
            };

            // Make the BrainCloud request
            long score = (long)(time * 1000.0f);   // Convert the time from seconds to milleseconds
            string jsonOtherData = "{\"nickname\":\"" + nickname + "\"}";
            m_BrainCloud.LeaderboardService.PostScoreToLeaderboard(leaderboardID, score, jsonOtherData, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("PostScoreToLeaderboard failed: user is not authenticated");

            if (postScoreRequestFailed != null)
                postScoreRequestFailed();
        }
    }

    public void RequestGlobalEntityLevelData(RequestGlobalEntityLevelDataCompleted requestGlobalEntityLevelDataCompleted = null, RequestGlobalEntityLevelDataFailed requestGlobalEntityLevelDataFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("RequestGlobalEntityLevelData success: " + responseData);

                List<LevelData> levelData = new List<LevelData>();
                string entityType;
                string entityID;

                // Read the json and update our values
                JsonData jsonData = JsonMapper.ToObject(responseData);
                JsonData entityList = jsonData["data"]["entityList"];

                if (entityList.IsArray)
                {
                    for (int i = 0; i < entityList.Count; i++)
                    {
                        entityType = entityList[i]["entityType"].ToString();
                        entityID = entityList[i]["entityId"].ToString();

                        JsonData level = entityList[i]["data"]["level"];
                        levelData.Add(new LevelData(entityType, entityID, ref level));
                    }
                }

                if (requestGlobalEntityLevelDataCompleted != null)
                    requestGlobalEntityLevelDataCompleted(ref levelData);
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("RequestUserEntityData failed: " + statusMessage);

                if (requestGlobalEntityLevelDataFailed != null)
                    requestGlobalEntityLevelDataFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.GlobalEntityService.GetListByIndexedId(Constants.kBrainCloudGlobalEntityIndexedID, 5, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("RequestUserEntityData failed: user is not authenticated");

            if (requestGlobalEntityLevelDataFailed != null)
                requestGlobalEntityLevelDataFailed();
        }
    }

    public void RequestUserStatistics(UserStatisticsRequestCompleted userStatisticsRequestCompleted = null, UserStatisticsRequestFailed userStatisticsRequestFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("RequestUserStatistics success: " + responseData);

                // Read the json and update our values
                JsonData jsonData = JsonMapper.ToObject(responseData);
                JsonData statistics = jsonData["data"]["statistics"];

                List<Statistic> statisticsList = new List<Statistic>();

                long value = 0;
                string description;
                foreach (string key in statistics.Keys)
                {
                    value = long.Parse(statistics[key].ToString());
                    description = Constants.kBrainCloudStatDescriptions[key];
                    statisticsList.Add(new Statistic(key, description, value));
                }

                if (userStatisticsRequestCompleted != null)
                    userStatisticsRequestCompleted(ref statisticsList);
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("RequestUserStatistics failed: " + statusMessage);

                if (userStatisticsRequestFailed != null)
                    userStatisticsRequestFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.PlayerStatisticsService.ReadAllUserStats(successCallback, failureCallback);
        }
        else
        {
            Debug.Log("RequestUserStatistics failed: user is not authenticated");

            if (userStatisticsRequestFailed != null)
                userStatisticsRequestFailed();
        }
    }

    public void IncrementUserStatistics(Dictionary<string, object> data, IncrementUserStatisticsCompleted incrementUserStatisticsCompleted = null, IncrementUserStatisticsFailed incrementUserStatisticsFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("IncrementUserStatistics success: " + responseData);

                // Read the json and update our values
                JsonData jsonData = JsonMapper.ToObject(responseData);
                JsonData statistics = jsonData["data"]["statistics"];

                List<Statistic> statisticsList = new List<Statistic>();

                long value = 0;
                string description;
                foreach (string key in statistics.Keys)
                {
                    value = long.Parse(statistics[key].ToString());
                    description = Constants.kBrainCloudStatDescriptions[key];
                    statisticsList.Add(new Statistic(key, description, value));
                }

                if (incrementUserStatisticsCompleted != null)
                    incrementUserStatisticsCompleted(ref statisticsList);
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("IncrementUserStatistics failed: " + statusMessage);

                if (incrementUserStatisticsFailed != null)
                    incrementUserStatisticsFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.PlayerStatisticsService.IncrementUserStats(data, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("IncrementUserStatistics failed: user is not authenticated");

            if (incrementUserStatisticsFailed != null)
                incrementUserStatisticsFailed();
        }
    }

    public void RequestReadAchievements(ReadAchievementRequestCompleted readAchievementRequestCompleted = null, ReadAchievementRequestFailed readAchievementRequestFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("RequestReadAchievements success: " + responseData);

                // Read the json and update our values
                JsonData jsonData = JsonMapper.ToObject(responseData);
                JsonData achievements = jsonData["data"]["achievements"];

                List<Achievement> achievementsList = new List<Achievement>();
                string id;
                string title;
                string description;
                string status;

                if (achievements.IsArray)
                {
                    for (int i = 0; i < achievements.Count; i++)
                    {
                        id = achievements[i]["id"].ToString();
                        title = achievements[i]["title"].ToString();
                        description = achievements[i]["description"].ToString();
                        status = achievements[i]["status"].ToString();

                        achievementsList.Add(new Achievement(id, title, description, status));
                    }
                }

                if (readAchievementRequestCompleted != null)
                    readAchievementRequestCompleted(ref achievementsList);
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("RequestReadAchievements failed: " + statusMessage);

                if (readAchievementRequestFailed != null)
                    readAchievementRequestFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.GamificationService.ReadAchievements(true, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("RequestAchievements failed: user is not authenticated");

            if (readAchievementRequestFailed != null)
                readAchievementRequestFailed();
        }
    }

    private void HandleAuthenticationSuccess(string responseData, object cbObject, AuthenticationRequestCompleted authenticationRequestCompleted)
    {
        // Read the player name from the response data
        JsonData jsonData = JsonMapper.ToObject(responseData);
        m_Username = jsonData["data"]["playerName"].ToString();

        if (authenticationRequestCompleted != null)
            authenticationRequestCompleted();
    }
}
