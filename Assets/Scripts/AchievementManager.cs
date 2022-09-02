// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager sharedInstance;

    private List<Achievement> m_Achievements;

    private void Awake()
    {
        sharedInstance = this;
    }

    public Achievement GetAchievementByID(string id)
    {
        if (m_Achievements != null)
            for (int i = 0; i < m_Achievements.Count; i++)
                if (m_Achievements[i].ID == id)
                    return m_Achievements[i];
        return null;
    }

    public Achievement GetAchievementAtIndex(int index)
    {
        if (m_Achievements != null)
            if (index >= 0 && index < GetCount())
                return m_Achievements[index];
        return null;
    }

    public int GetCount()
    {
        return m_Achievements.Count;
    }

    public void SetAchievements(ref List<Achievement> achievements)
    {
        m_Achievements = achievements;
    }
}
