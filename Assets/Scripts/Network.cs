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
    public delegate void AuthenticationRequestFailed(string errorMessage);
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
    public delegate void AchievementAwardedRequestCompleted();
    public delegate void AchievementAwardRequestFailed();
    public delegate void RequestUserEntityDataCompleted(UserData userData);
    public delegate void RequestUserEntityDataFailed();
    public delegate void CreateUserEntityDataCompleted();
    public delegate void CreateUserEntityDataFailed();
    public delegate void UpdateUserEntityDataCompleted();
    public delegate void UpdateUserEntityDataFailed();
    public delegate void GetIdentitiesRequestCompleted();
    public delegate void GetIdentitiesRequestFailed();
    public delegate void AttachEmailIdentityCompleted();
    public delegate void AttachEmailIdentityFailed();

    public static Network sharedInstance;

    private BrainCloudWrapper m_BrainCloud;
    private string m_Username;
    private List<string> m_IdentityTypesList = new List<string>();


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

    public List<string> IdentityTypesList
    {
        get { return m_IdentityTypesList; }
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
            Debug.Log("Reconnect success: " + responseData);
            HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
        };

        // Failure callback lambda
        BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
        {
            string errorMessage = "Reconnect failed. " + ExtractStatusMessage(error);

            Debug.Log(errorMessage);

            if (authenticationRequestFailed != null)
                authenticationRequestFailed(errorMessage);
        };

        // Make the BrainCloud request
        m_BrainCloud.Reconnect(successCallback, failureCallback);
    }

    public void RequestAnonymousAuthentication(AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        // Success callback lambda
        BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
        {
            Debug.Log("RequestAnonymousAuthentication success: " + responseData);
            HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
        };

        // Failure callback lambda
        BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
        {
            string errorMessage = "RequestAnonymousAuthentication failed. " + ExtractStatusMessage(error);

            Debug.Log(errorMessage);

            if (authenticationRequestFailed != null)
                authenticationRequestFailed(errorMessage);
        };

        // Make the BrainCloud request
        m_BrainCloud.AuthenticateAnonymous(successCallback, failureCallback);
    }

    public void RequestAuthenticationEmail(string email, string password, AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        // Success callback lambda
        BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
        {
            Debug.Log("RequestAuthenticationEmail success: " + responseData);
            HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
        };

        // Failure callback lambda
        BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
        {
            string errorMessage = "RequestAuthenticationEmail failed. " + ExtractStatusMessage(error);

            Debug.Log(errorMessage);

            if (authenticationRequestFailed != null)
                authenticationRequestFailed(errorMessage);
        };

        // Make the BrainCloud request
        m_BrainCloud.AuthenticateEmailPassword(email, password, true, successCallback, failureCallback);
    }

    public void RequestAuthenticationUniversal(string userID, string password, AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        // Success callback lambda
        BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
        {
            Debug.Log("RequestAuthenticationUniversal success: " + responseData);
            HandleAuthenticationSuccess(responseData, cbObject, authenticationRequestCompleted);
        };

        // Failure callback lambda
        BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
        {
            string errorMessage = "RequestAuthenticationUniversal failed. " + ExtractStatusMessage(error);

            Debug.Log(errorMessage);

            if (authenticationRequestFailed != null)
                authenticationRequestFailed(errorMessage);
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

    public void AwardAchievementRequest(Achievement achievement, AchievementAwardedRequestCompleted achievementAwardedRequestCompleted = null, AchievementAwardRequestFailed achievementRequestFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("AwardAchievementRequest success: " + responseData);

                if (achievementAwardedRequestCompleted != null)
                    achievementAwardedRequestCompleted();
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("AwardAchievementRequest failed: " + statusMessage);

                if (achievementRequestFailed != null)
                    achievementRequestFailed();
            };

            // Make the BrainCloud request
            string[] achievements = { achievement.ID };
            m_BrainCloud.GamificationService.AwardAchievements(achievements, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("AwardAchievementRequest failed: user is not authenticated");

            if (achievementRequestFailed != null)
                achievementRequestFailed();
        }
    }

    public void RequestUserEntityData(RequestUserEntityDataCompleted requestUserEntityDataCompleted = null, RequestUserEntityDataFailed requestUserEntityDataFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("RequestUserEntityData success: " + responseData);

                JsonData jsonData = JsonMapper.ToObject(responseData);
                JsonData entities = jsonData["data"]["entities"];

                UserData userData = null;

                if (entities.IsArray && entities.Count > 0)
                {
                    string entityID = entities[0]["entityId"].ToString();
                    string entityType = entities[0]["entityType"].ToString();

                    userData = new UserData(entityID, entityType);
                    userData.LevelOneCompleted = bool.Parse(entities[0]["data"]["levelOneCompleted"].ToString());
                    userData.LevelTwoCompleted = bool.Parse(entities[0]["data"]["levelTwoCompleted"].ToString());
                    userData.LevelThreeCompleted = bool.Parse(entities[0]["data"]["levelThreeCompleted"].ToString());
                    userData.LevelBossCompleted = bool.Parse(entities[0]["data"]["levelBossCompleted"].ToString());
                }

                if (requestUserEntityDataCompleted != null)
                    requestUserEntityDataCompleted(userData);
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("RequestUserEntityData failed: " + statusMessage);

                if (requestUserEntityDataFailed != null)
                    requestUserEntityDataFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.EntityService.GetEntitiesByType(Constants.kBrainCloudUserProgressUserEntityType, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("RequestUserEntityData failed: user is not authenticated");

            if (requestUserEntityDataFailed != null)
                requestUserEntityDataFailed();
        }
    }

    public void CreateUserEntityData(CreateUserEntityDataCompleted createUserEntityDataCompleted = null, CreateUserEntityDataFailed createUserEntityDataFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("CreateUserEntityData success: " + responseData);

                if (createUserEntityDataCompleted != null)
                    createUserEntityDataCompleted();
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("CreateUserEntityData failed: " + statusMessage);

                if (createUserEntityDataFailed != null)
                    createUserEntityDataFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.EntityService.CreateEntity(Constants.kBrainCloudUserProgressUserEntityType,
                                                    Constants.kBrainCloudUserProgressUserEntityDefaultData,
                                                    Constants.kBrainCloudUserProgressUserEntityDefaultAcl,
                                                    successCallback, failureCallback);
        }
        else
        {
            Debug.Log("CreateUserEntityData failed: user is not authenticated");

            if (createUserEntityDataFailed != null)
                createUserEntityDataFailed();
        }
    }

    public void UpdateUserEntityData(string entityID, string entityType, string jsonData, UpdateUserEntityDataCompleted updateUserEntityDataCompleted = null, UpdateUserEntityDataFailed updateUserEntityDataFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("UpdateUserEntityData success: " + responseData);

                if (updateUserEntityDataCompleted != null)
                    updateUserEntityDataCompleted();
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("UpdateUserEntityData failed: " + statusMessage);

                if (updateUserEntityDataFailed != null)
                    updateUserEntityDataFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.EntityService.UpdateEntity(entityID, entityType, jsonData, Constants.kBrainCloudUserProgressUserEntityDefaultAcl, -1, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("UpdateUserEntityData failed: user is not authenticated");

            if (updateUserEntityDataFailed != null)
                updateUserEntityDataFailed();
        }
    }

    public void RequestGetIdentities(GetIdentitiesRequestCompleted getIdentitiesRequestCompleted = null, GetIdentitiesRequestFailed getIdentitiesRequestFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("RequestGetIdentities success: " + responseData);

                JsonData jsonData = JsonMapper.ToObject(responseData);
                JsonData identities = jsonData["data"]["identities"];

                // Clear the Identity types list before adding new identities to it
                m_IdentityTypesList.Clear();

                // Add the non-anonymous identities to the identity types list
                foreach (string key in identities.Keys)
                    m_IdentityTypesList.Add(key);

                if (getIdentitiesRequestCompleted != null)
                    getIdentitiesRequestCompleted();
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("RequestGetIdentities failed: " + statusMessage);

                if (getIdentitiesRequestFailed != null)
                    getIdentitiesRequestFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.IdentityService.GetIdentities(successCallback, failureCallback);
        }
        else
        {
            Debug.Log("RequestGetIdentities failed: user is not authenticated");

            if (getIdentitiesRequestFailed != null)
                getIdentitiesRequestFailed();
        }
    }

    public void AttachEmailIdentity(string email, string password, AttachEmailIdentityCompleted attachEmailIdentityCompleted = null, AttachEmailIdentityFailed attachEmailIdentityFailed = null)
    {
        if (IsAuthenticated())
        {
            // Success callback lambda
            BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
            {
                Debug.Log("AttachEmailIdentity success: " + responseData);

                // Ensure the Identity types list doesn't contain an email identity, if it doesn't add it
                if (!m_IdentityTypesList.Contains("Email"))
                    m_IdentityTypesList.Add("Email");

                if (attachEmailIdentityCompleted != null)
                    attachEmailIdentityCompleted();
            };

            // Failure callback lambda
            BrainCloud.FailureCallback failureCallback = (statusMessage, code, error, cbObject) =>
            {
                Debug.Log("AttachEmailIdentity failed: " + statusMessage);

                if (attachEmailIdentityFailed != null)
                    attachEmailIdentityFailed();
            };

            // Make the BrainCloud request
            m_BrainCloud.IdentityService.AttachEmailIdentity(email, password, successCallback, failureCallback);
        }
        else
        {
            Debug.Log("AttachEmailIdentity failed: user is not authenticated");

            if (attachEmailIdentityFailed != null)
                attachEmailIdentityFailed();
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

    private string ExtractStatusMessage(string error)
    {
        JsonData jsonData = JsonMapper.ToObject(error);
        string statusMessage = jsonData["status_message"].ToString();
        return statusMessage;
    }
}
