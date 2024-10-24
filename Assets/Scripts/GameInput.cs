using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    public event EventHandler Fire;
    public event EventHandler FireStop;
    public event EventHandler Pause;
    public event EventHandler Move;
    public event EventHandler Aim;

    private GameInputActions gameInputActions;

    private void Awake()
    {
        Instance = this;

        gameInputActions = new GameInputActions();
        gameInputActions.Player.Enable();

        gameInputActions.Player.Fire.performed += Fire_performed;
        gameInputActions.Player.Fire.canceled += Fire_canceled;
        gameInputActions.Player.Pause.performed += Pause_performed;
        gameInputActions.Player.Move.performed += Move_performed;
        gameInputActions.Player.Aim.performed += Aim_performed;
    }

    private void OnDestroy()
    {
        gameInputActions.Player.Fire.performed -= Fire_performed;
        gameInputActions.Player.Fire.canceled -= Fire_canceled;
        gameInputActions.Player.Pause.performed -= Pause_performed;
        gameInputActions.Player.Move.performed -= Move_performed;
        gameInputActions.Player.Aim.performed -= Aim_performed;

        gameInputActions.Dispose();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Pause?.Invoke(this, EventArgs.Empty);
    }

    private void Fire_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Fire?.Invoke(this, EventArgs.Empty);
    }

    private void Fire_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        FireStop?.Invoke(this, EventArgs.Empty);
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Move?.Invoke(this, EventArgs.Empty);
    }

    private void Aim_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Aim?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMoveVector()
    {
        Vector2 inputVector = gameInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector;
    }

    public Vector2 GetAimVector()
    {
        Vector2 inputVector = gameInputActions.Player.Aim.ReadValue<Vector2>();

        return inputVector;
    }
}
