using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputControl
{
    public readonly static InputControl Instance;



    private PlayerInputControl inputActions;
    //GamePlayer
    public Vector2 MovePoint => inputActions.GamePlayer.Move.ReadValue<Vector2>();
    private Vector2 LookPoint;

    public InputAction LeftMouse => inputActions.GamePlayer.LeftMouse;
    public InputAction RightMouse => inputActions.GamePlayer.RightMouse;
    public InputAction LeftGoods => inputActions.GamePlayer.LeftGoods;
    public InputAction RightGoods => inputActions.GamePlayer.RightGoods;
    public InputAction ShiftButton => inputActions.GamePlayer.ShiftButton;
    public InputAction GButton => inputActions.GamePlayer.GButton;
    public InputAction EButton => inputActions.GamePlayer.EButton;
    public InputAction QButton => inputActions.GamePlayer.QButton;
    public InputAction EscZButton => inputActions.GamePlayer.Esc;    
    public InputAction TabButton => inputActions.GamePlayer.Tab;
    //UI
    public Vector2 NavigatePoint => inputActions.UI.Navigate.ReadValue<Vector2>();
    public InputAction LeftPage => inputActions.UI.LeftPage;
    public InputAction RightPage => inputActions.UI.RightPage;
    public InputAction Submit => inputActions.UI.Submit;
    public InputAction Cancel => inputActions.UI.Cancel;

    static InputControl()
    {
        Instance = new InputControl();
    }

    private InputControl()
    {
        inputActions = new PlayerInputControl();
        inputActions.Enable();
    }

    public void UIEnable()
    {
        inputActions.UI.Enable();
        inputActions.GamePlayer.Disable();
    }

    public void GamePlayerEnable()
    {
        inputActions.GamePlayer.Enable();
        inputActions.UI.Enable();
    }

    bool isMouse = true;
    public Vector2 GetLook()
    {
        if(Mouse.current != null && Mouse.current.delta.ReadValue() - Vector2.zero != Vector2.zero)
        {
            isMouse = true;
        }
        if(Gamepad.current != null && Gamepad.current.rightStick.ReadValue() - Vector2.zero != Vector2.zero)
        {
            isMouse = false;
        }
        if(isMouse)
        {
            Debug.Log("IsMouse!!!!!");
            LookPoint = inputActions.GamePlayer.Look.ReadValue<Vector2>();
        }
        else
        {
            Debug.Log("IsMouse11111!!!!!");
            LookPoint += inputActions.GamePlayer.Look.ReadValue<Vector2>();
        }
        return LookPoint;
    }

    



    ~InputControl()
    {
        if(inputActions != null)
        {
            inputActions.Disable();
        }
    }

}
