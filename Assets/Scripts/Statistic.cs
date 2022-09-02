// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistic
{
    private string m_Name;
    private string m_Description;
    private long m_Value;
    private long m_Increment;

    public Statistic(string name, string description, long value)
    {
        m_Name = name;
        m_Description = description;
        m_Value = value;
        m_Increment = 0;
    }

    public void ApplyIncrement(int amount = 1)
    {
        m_Increment += amount;
        m_Value += amount;
    }

    public string Name
    {
        get { return m_Name; }
    }

    public string Description
    {
        get { return m_Description; }
    }

    public long Value
    {
        get { return m_Value; }
    }

    public long Increment
    {
        get { return m_Increment; }
    }
}