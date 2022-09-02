// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private ConnectingDialog connectingDialog;
    [SerializeField] private MainMenuDialog mainMenuDialog;
    [SerializeField] private PauseDialog pauseDialog;
    [SerializeField] private PlayAgainDialog playAgainDialog;
    [SerializeField] private AchievementDialog achievementDialog;
    [SerializeField] private StatisticsDialog statisticsDialog;
    [SerializeField] private LeaderboardDialog leaderboardDialog;
    [SerializeField] private PostScoreDialog postScoreDialog;
    [SerializeField] private ChangeUsernameDialog changeUsernameDialog;
    [SerializeField] private UniversalLoginDialog universaleLoginDialog;
    [SerializeField] private EmailLoginDialog emailLoginDialog;
    [SerializeField] private BrainCloudDialog brainCloudDialog;
    [SerializeField] private InGameBrainCloudDialog inGameBrainCloudDialog;
    [SerializeField] private LevelSelectDialog levelSelectDialog;
    [SerializeField] private AttachEmailDialog attachEmailDialog;
    [SerializeField] private ErrorDialog errorDialog;


    public static DialogManager sharedInstance;

    private Stack<Dialog> m_ActiveDialogs = new Stack<Dialog>();


    private void Awake()
    {
        sharedInstance = this;

        connectingDialog.DialogShown = OnDialogShown;
        connectingDialog.DialogHidden = OnDialogHidden;
        mainMenuDialog.DialogShown = OnDialogShown;
        mainMenuDialog.DialogHidden = OnDialogHidden;
        pauseDialog.DialogShown = OnDialogShown;
        pauseDialog.DialogHidden = OnDialogHidden;
        playAgainDialog.DialogShown = OnDialogShown;
        playAgainDialog.DialogHidden = OnDialogHidden;
        achievementDialog.DialogShown = OnDialogShown;
        achievementDialog.DialogHidden = OnDialogHidden;
        statisticsDialog.DialogShown = OnDialogShown;
        statisticsDialog.DialogHidden = OnDialogHidden;
        leaderboardDialog.DialogShown = OnDialogShown;
        leaderboardDialog.DialogHidden = OnDialogHidden;
        postScoreDialog.DialogShown = OnDialogShown;
        postScoreDialog.DialogHidden = OnDialogHidden;
        changeUsernameDialog.DialogShown = OnDialogShown;
        changeUsernameDialog.DialogHidden = OnDialogHidden;
        universaleLoginDialog.DialogShown = OnDialogShown;
        universaleLoginDialog.DialogHidden = OnDialogHidden;
        emailLoginDialog.DialogShown = OnDialogShown;
        emailLoginDialog.DialogHidden = OnDialogHidden;
        brainCloudDialog.DialogShown = OnDialogShown;
        brainCloudDialog.DialogHidden = OnDialogHidden;
        inGameBrainCloudDialog.DialogShown = OnDialogShown;
        inGameBrainCloudDialog.DialogHidden = OnDialogHidden;
        levelSelectDialog.DialogShown = OnDialogShown;
        levelSelectDialog.DialogHidden = OnDialogHidden;
        attachEmailDialog.DialogShown = OnDialogShown;
        attachEmailDialog.DialogHidden = OnDialogHidden;
        errorDialog.DialogShown = OnDialogShown;
        errorDialog.DialogHidden = OnDialogHidden;
    }

    public void ShowConnectingDialog()
    {
        if (!connectingDialog.IsShowing())
            Show(connectingDialog);
    }

    public void HideConnectingDialog()
    {
        if (connectingDialog.IsShowing())
            connectingDialog.Hide();
    }

    public void ShowMainMenuDialog()
    {
        if (!mainMenuDialog.IsShowing())
            Show(mainMenuDialog);
    }

    public void ShowPauseDialog()
    {
        if (!pauseDialog.IsShowing())
            Show(pauseDialog);
    }

    public void ShowPlayAgainDialog()
    {
        if (!playAgainDialog.IsShowing())
            Show(playAgainDialog);
    }

    public void ShowAchievementDialog()
    {
        if (!achievementDialog.IsShowing())
            Show(achievementDialog);
    }

    public void ShowStatisticsDialog()
    {
        if (!statisticsDialog.IsShowing())
            Show(statisticsDialog);
    }

    public void ShowLeaderboardsDialog()
    {
        if (!leaderboardDialog.IsShowing())
            Show(leaderboardDialog);
    }

    public void ShowUniversalLoginDialog(Network.AuthenticationRequestCompleted authenticationRequestCompleted = null, Network.AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        if (!universaleLoginDialog.IsShowing())
        {
            universaleLoginDialog.Set(authenticationRequestCompleted, authenticationRequestFailed);
            Show(universaleLoginDialog);
        }
    }

    public void ShowEmailLoginDialog(Network.AuthenticationRequestCompleted authenticationRequestCompleted = null, Network.AuthenticationRequestFailed authenticationRequestFailed = null)
    {
        if (!emailLoginDialog.IsShowing())
        {
            emailLoginDialog.Set(authenticationRequestCompleted, authenticationRequestFailed);
            Show(emailLoginDialog);
        }
    }

    public void ShowPostScoreDialog(float time)
    {
        if (!postScoreDialog.IsShowing())
        {
            postScoreDialog.Set(time);
            Show(postScoreDialog);
        }
    }

    public void ShowChangeUsernameDialog()
    {
        if (!changeUsernameDialog.IsShowing())
            Show(changeUsernameDialog);
    }

    public void ShowAttachEmailDialog()
    {
        if (!attachEmailDialog.IsShowing())
        {
            Show(attachEmailDialog);
        }
    }

    public void ShowBrainCloudDialog(bool inGameDialog = false)
    {
        if (inGameDialog)
        {
            if (!inGameBrainCloudDialog.IsShowing())
                Show(inGameBrainCloudDialog);
        }
        else
        {
            if (!brainCloudDialog.IsShowing())
                Show(brainCloudDialog);
        }
    }

    public void ShowLevelSelectDialog()
    {
        if (!levelSelectDialog.IsShowing())
            Show(levelSelectDialog);
    }

    public void ShowErrorDialog(string message)
    {
        if (!errorDialog.IsShowing())
        {
            errorDialog.Set(message);
            Show(errorDialog);
        }
    }

    public bool AreAnyDialogsShowing()
    {
        return m_ActiveDialogs.Count > 0;
    }

    public int DialogStackCount()
    {
        return m_ActiveDialogs.Count;
    }

    public void OnEscape()
    {
        if (!AreAnyDialogsShowing())
        {
            if (Game.sharedInstance.IsGameOver() || Game.sharedInstance.IsGameWon())
                ShowPlayAgainDialog();
            else
                ShowPauseDialog();
        }
        else
        {
            if(m_ActiveDialogs.Peek() != mainMenuDialog)
                m_ActiveDialogs.Peek().GetComponent<Dialog>().Hide();
        }
    }

    private void Show(Dialog dialog)
    {
        if (m_ActiveDialogs.Count == 0)
            Time.timeScale = 0.0f;
        else
            m_ActiveDialogs.Peek().Hide(false);

        m_ActiveDialogs.Push(dialog);

        dialog.Show();
    }

    private void OnDialogShown(Dialog dialog)
    {

    }

    private void OnDialogHidden(Dialog dialog)
    {
        m_ActiveDialogs.Pop();

        if (m_ActiveDialogs.Count == 0)
            Time.timeScale = 1.0f;
        else
            m_ActiveDialogs.Peek().Show(false);
    }
}
