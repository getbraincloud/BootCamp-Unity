// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum DialogType
{
    Unknown = -1,
    Pause,
    Login,
    HighScores,
    PostScore
};

public enum DialogSize
{
    Unknown = -1,
    Big,
    Medium,
    Small,
    Narrow
};


public class Dialog : MonoBehaviour
{
    public delegate void DialogShownDelegate(Dialog dialog);
    public delegate void DialogHiddenDelegate(Dialog dialog);

    private bool m_IsShowing = false;
    private DialogShownDelegate m_DialogShown;
    private DialogHiddenDelegate m_DialogHidden;


    public DialogShownDelegate DialogShown
    {
        get { return m_DialogShown; }
        set { m_DialogShown = value; }
    }

    public DialogHiddenDelegate DialogHidden
    {
        get { return m_DialogHidden; }
        set { m_DialogHidden = value; }
    }

    public void Show(bool triggerCallback = true)
    {
        m_IsShowing = true;
        gameObject.SetActive(true);

        OnShow();

        if (triggerCallback && m_DialogShown != null)
            m_DialogShown(this);
    }

    public void Hide(bool triggerCallback = true)
    {
        m_IsShowing = false;
        gameObject.SetActive(false);

        OnHide();

        if(triggerCallback && m_DialogHidden != null)
            m_DialogHidden(this);
    }

    public bool IsShowing()
    {
        return m_IsShowing;
    }

    public void OnCloseButtonClicked()
    {
        Hide();
        OnClose();
    }

    protected virtual void OnShow() {}
    protected virtual void OnHide() {}
    protected virtual void OnClose() {}

}
