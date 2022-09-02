// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard
{
    private string m_Name;
    private List<HighScore> m_HighScores;

    public string Name
    {
        get { return m_Name; }
    }

    public Leaderboard(string name, List<HighScore> highScores)
    {
        m_Name = name;
        m_HighScores = highScores;
    }

    public HighScore GetHighScoreAtIndex(int index)
    {
        if (index >= 0 && index < GetCount())
            return m_HighScores[index];
        return null;
    }

    public int GetCount()
    {
        return m_HighScores.Count;
    }
}
