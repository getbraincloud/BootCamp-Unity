// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BrainCloudDialog : Dialog
{
    [SerializeField] private Button attachEmailButton;
    [SerializeField] private Button setUsernameButton;
    [SerializeField] private Button highScoresButton;
    [SerializeField] private Button statisticsButton;
    [SerializeField] private Button achievementsButton;
    [SerializeField] private Button logOutButton;
    [SerializeField] private Button logInButton;


    protected override void OnShow()
    {
        Refresh();
    }

    public void OnAttachEmailClicked()
    {
        //TODO: Implement attaching an email identity
    }

    public void OnSetUsernameButtonClicked()
    {
        DialogManager.sharedInstance.ShowChangeUsernameDialog();
    }

    public void OnLeaderboardsButtonClicked()
    {
        DialogManager.sharedInstance.ShowLeaderboardsDialog();
    }

    public void OnStatisticsButtonClicked()
    {
        DialogManager.sharedInstance.ShowStatisticsDialog();
    }

    public void OnAchievementsButtonClicked()
    {
        DialogManager.sharedInstance.ShowAchievementDialog();
    }

    public void OnLogOutButtonClicked()
    {
        //TODO: Implement brainCloud log out
    }

    public void OnLogInButtonClicked()
    {
        Hide();
        Game.sharedInstance.HandleAuthentication();
    }

    private void OnBrainCloudLogOutCompleted()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (Network.sharedInstance.IsAuthenticated())
        {
            attachEmailButton.gameObject.SetActive(true);
            setUsernameButton.gameObject.SetActive(true);
            highScoresButton.gameObject.SetActive(true);
            statisticsButton.gameObject.SetActive(true);
            achievementsButton.gameObject.SetActive(true);
            logOutButton.gameObject.SetActive(true);
            logInButton.gameObject.SetActive(false);
        }
        else
        {
            attachEmailButton.gameObject.SetActive(false);
            setUsernameButton.gameObject.SetActive(false);
            highScoresButton.gameObject.SetActive(false);
            statisticsButton.gameObject.SetActive(false);
            achievementsButton.gameObject.SetActive(false);
            logOutButton.gameObject.SetActive(false);
            logInButton.gameObject.SetActive(true);
        }
    }
}
