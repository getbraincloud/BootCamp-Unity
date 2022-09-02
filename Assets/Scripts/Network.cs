// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Network : MonoBehaviour
{
    public delegate void AuthenticationRequestCompleted();
    public delegate void AuthenticationRequestFailed();

    public static Network sharedInstance;

    private BrainCloudWrapper m_BrainCloud;


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

    public bool IsAuthenticated()
    {
        return m_BrainCloud.Client.Authenticated;
    }

    public void RequestAnonymousAuthentication(AuthenticationRequestCompleted authenticationRequestCompleted = null, AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        // Success callback lambda
        BrainCloud.SuccessCallback successCallback = (responseData, cbObject) =>
        {
            Debug.Log("Anonymous authentication success: " + responseData);

            if (authenticationRequestCompleted != null)
                authenticationRequestCompleted();
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
}
