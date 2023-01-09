// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HeadsUpDisplay : MonoBehaviour
{
    [SerializeField] private Text timer;
    [SerializeField] private GameObject scrollingElementPrefab;
    [SerializeField] private GameObject mainInformation;
    [SerializeField] private GameObject secondaryInformation;
    [SerializeField] private GameObject timeDivider;
    [SerializeField] private GameObject highScoreMask;

    private List<GameObject> m_HighScores;
    private Vector2 m_ScreenBounds;
    private bool m_PlayerHasAllTimeHighScore = false;
    private Fader m_InformationFader;


    private void Awake()
    {
        m_HighScores = new List<GameObject>();
        m_InformationFader = new Fader();
        m_ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    void Update()
    {
        if (m_HighScores.Count > 0 && m_HighScores[0].transform.localPosition.y == GetHighScoreLeavingPosition().y)
        {
            Destroy(m_HighScores[0]);
            m_HighScores.RemoveAt(0);
            TryPushingNextHighScore();
        }

        if (mainInformation.activeInHierarchy && m_InformationFader.IsFading())
        {
            m_InformationFader.Update();

            Color color = new Color(1.0f, 1.0f, 1.0f, m_InformationFader.Alpha);
            mainInformation.GetComponent<Text>().color = color;
            secondaryInformation.GetComponent<Text>().color = color;
        }
    }

    public void SetElapsedTime(float elapsedTime)
    {
        timer.text = "Time: " + TimeSpan.FromSeconds(elapsedTime).ToString(@"mm\:ss");
    }

    public void ShowGameOver()
    {
        ShowInformation("GAME OVER", "");
    }

    public void HideGameOver()
    {
        HideInformation(false);
    }

    public void ShowGameWon()
    {
        ShowInformation("YOU\'VE SURVIVED...", "JUST BARELY. ENJOY YOUR VICTORY.");
    }

    public void HideGameWon()
    {
        HideInformation(false);
    }

    public void ShowLevel(int number, string information = "")
    {
        if(number == -1)
            ShowInformation("SURVIVE", "As long as possible");
        else
            ShowInformation("LEVEL " + number.ToString(), information);
    }

    public void HideLevel()
    {
        HideInformation();
    }

    public void Reset()
    {
        timer.color = new Color32(255, 255, 255, 255);
        timer.text = "Time: 00:00";
        HideGameOver();

        for(int i = 0; i < m_HighScores.Count; i++)
            Destroy(m_HighScores[i]);

        m_HighScores.Clear();
    }

    public void PushLeaderboardEntry(LeaderboardEntry leaderboardEntry)
    { 
        GameObject temp;
        temp = Instantiate(scrollingElementPrefab, highScoreMask.transform);
        temp.GetComponent<HeadsUpDisplayScrollingElement>().Init(leaderboardEntry, GetHighScoreEnteringPosition());

        PushHighScoreElement(temp);
    }

    public void PushAchievement(Achievement achievement)
    {
        GameObject temp;
        temp = Instantiate(scrollingElementPrefab, highScoreMask.transform);
        temp.GetComponent<HeadsUpDisplayScrollingElement>().Init(achievement, GetHighScoreEnteringPosition(), OnHeadsUpDisplayScrollingElementHoldCompleted);

        PushHighScoreElement(temp);
    }

    public void PushLevelGoal(string levelGoal)
    {
        GameObject temp;
        temp = Instantiate(scrollingElementPrefab, highScoreMask.transform);
        temp.GetComponent<HeadsUpDisplayScrollingElement>().Init(levelGoal, GetHighScoreEnteringPosition());

        PushHighScoreElement(temp);
    }

    public void SetPlayerHasAllTimeHighScore()
    {
        if (!m_PlayerHasAllTimeHighScore)
        {
            m_PlayerHasAllTimeHighScore = true;
            timer.color = new Color32(207, 198, 0, 255);
            PushAllTimeHighScore();
        }
    }

    private void ShowInformation(string main, string secondary, bool fadeIn = true)
    {
        if (!mainInformation.activeInHierarchy)
        {
            mainInformation.SetActive(true);
            mainInformation.GetComponent<Text>().text = main;

            secondaryInformation.SetActive(true);
            secondaryInformation.GetComponent<Text>().text = secondary;

            if (fadeIn)
            {
                Color color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                mainInformation.GetComponent<Text>().color = color;
                secondaryInformation.GetComponent<Text>().color = color;

                m_InformationFader.StartFade(FadeType.In, 0.5f);
            }
            else
            {
                Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                mainInformation.GetComponent<Text>().color = color;
                secondaryInformation.GetComponent<Text>().color = color;
            }
        }
    }

    private void HideInformation(bool fadeOut = true)
    {
        if (fadeOut)
            m_InformationFader.StartFade(FadeType.Out, 0.5f, OnInformationFadeOutComplete);
        else
            OnInformationFadeOutComplete(m_InformationFader);
    }

    private void TryPushingNextHighScore()
    {
        if (m_HighScores.Count >= 2)
        {
            if (!m_HighScores[0].GetComponent<HeadsUpDisplayScrollingElement>().IsMoving() && m_HighScores[0].GetComponent<HeadsUpDisplayScrollingElement>().CanPush())
            {
                m_HighScores[0].GetComponent<HeadsUpDisplayScrollingElement>().MoveTo(GetHighScoreLeavingPosition());
                m_HighScores[1].GetComponent<HeadsUpDisplayScrollingElement>().MoveTo(GetHighScorePosition());
            }
        }
    }

    private void PushAllTimeHighScore()
    {
        GameObject temp;
        temp = Instantiate(scrollingElementPrefab, highScoreMask.transform);
        temp.GetComponent<HeadsUpDisplayScrollingElement>().Init("NEW #1 HIGH SCORE!", GetHighScoreEnteringPosition());

        PushHighScoreElement(temp);
    }

    private void PushHighScoreElement(GameObject go)
    {
        m_HighScores.Add(go);

        if (m_HighScores.Count == 1)  //If it's the first highscore push it
            m_HighScores[m_HighScores.Count - 1].GetComponent<HeadsUpDisplayScrollingElement>().MoveTo(GetHighScorePosition());
        else
            TryPushingNextHighScore();
    }

    private Vector2 GetHighScorePosition()
    {
        return Vector2.zero;
    }

    private Vector2 GetHighScoreEnteringPosition()
    {
        float x = GetHighScorePosition().x;
        float y = GetHighScorePosition().y - Constants.kHudHeight - Constants.kHudSmallBuffer;
        return new Vector2(x, y);
    }

    private Vector2 GetHighScoreLeavingPosition()
    {
        float x = GetHighScorePosition().x;
        float y = GetHighScorePosition().y + Constants.kHudHeight + Constants.kHudSmallBuffer;
        return new Vector2(x, y);
    }

    private void OnInformationFadeOutComplete(Fader fader)
    {
        if (fader == m_InformationFader)
        {
            mainInformation.SetActive(false);
            secondaryInformation.SetActive(false);
        }
    }

    private void OnHeadsUpDisplayScrollingElementHoldCompleted(HeadsUpDisplayScrollingElement headsUpDisplayScrollingElement)
    {
        TryPushingNextHighScore();
    }
}
      