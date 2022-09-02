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

    private void HandleAuthenticationSuccess(string responseData, object cbObject, AuthenticationRequestCompleted authenticationRequestCompleted)
    {
        // Read the player name from the response data
        JsonData jsonData = JsonMapper.ToObject(responseData);
        m_Username = jsonData["data"]["playerName"].ToString();

        if (authenticationRequestCompleted != null)
            authenticationRequestCompleted();
    }
}
