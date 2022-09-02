// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Ship ship;
    private InputMaster m_Input;

    private void Awake()
    {
        m_Input = new InputMaster();
    }

    private void OnEnable()
    {
        m_Input.Enable();

        m_Input.Ship.Fire.performed += context => ship.OnFire();
        m_Input.Ship.Hold.performed += context => ship.OnHold(context);

        m_Input.Ship.MovementUp.started += context => ship.OnMovementUpStart();
        m_Input.Ship.MovementUp.canceled += context => ship.OnMovementUpStop();

        m_Input.Ship.MovementDown.started += context => ship.OnMovementDownStart();
        m_Input.Ship.MovementDown.canceled += context => ship.OnMovementDownStop();

        m_Input.Ship.MovementLeft.started += context => ship.OnMovementLeftStart();
        m_Input.Ship.MovementLeft.canceled += context => ship.OnMovementLeftStop();

        m_Input.Ship.MovementRight.started += context => ship.OnMovementRightStart();
        m_Input.Ship.MovementRight.canceled += context => ship.OnMovementRightStop();

        m_Input.Dialogs.Escape.performed += context => DialogManager.sharedInstance.OnEscape();

        m_Input.CheatCodes.AddTime.performed += context => Game.sharedInstance.OnCheatAddTime();
        m_Input.CheatCodes.NextLevel.performed += context => Game.sharedInstance.OnCheatNextLevel();
        m_Input.CheatCodes.HealUp.performed += context => Game.sharedInstance.OnCheatHealUp();

    }

    private void OnDisable()
    {
        m_Input.Disable();

        m_Input.Ship.Fire.performed -= context => ship.OnFire();
        m_Input.Ship.Hold.performed -= context => ship.OnHold(context);

        m_Input.Ship.MovementUp.started -= context => ship.OnMovementUpStart();
        m_Input.Ship.MovementUp.canceled -= context => ship.OnMovementUpStop();

        m_Input.Ship.MovementDown.started -= context => ship.OnMovementDownStart();
        m_Input.Ship.MovementDown.canceled -= context => ship.OnMovementDownStop();

        m_Input.Ship.MovementLeft.started -= context => ship.OnMovementLeftStart();
        m_Input.Ship.MovementLeft.canceled -= context => ship.OnMovementLeftStop();

        m_Input.Ship.MovementRight.started -= context => ship.OnMovementRightStart();
        m_Input.Ship.MovementRight.canceled -= context => ship.OnMovementRightStop();

        m_Input.Dialogs.Escape.performed -= context => DialogManager.sharedInstance.OnEscape();

        m_Input.CheatCodes.AddTime.performed -= context => Game.sharedInstance.OnCheatAddTime();
        m_Input.CheatCodes.NextLevel.performed -= context => Game.sharedInstance.OnCheatNextLevel();
        m_Input.CheatCodes.HealUp.performed -= context => Game.sharedInstance.OnCheatHealUp();
    }
}
