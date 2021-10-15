using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player Input")]

public class PlayerInput : ScriptableObject, InputActions.IGameplayActions,InputActions.IPauseMenuActions
{
    public event UnityAction<Vector2> onMove = delegate { };
    public event UnityAction onStopMove = delegate { };
    public event UnityAction onFire = delegate { };
    public event UnityAction onStopFire = delegate { };
    public event UnityAction onDodge = delegate { };
    public event UnityAction onOverdrive = delegate { };
    public event UnityAction onPause = delegate { };
    public event UnityAction onUnpause = delegate { };
    public event UnityAction onLaunchMissile = delegate { };

    public event UnityAction onAddHP = delegate { };
    public event UnityAction onNoHurt = delegate { };

    InputActions inputActions;

    void OnEnable()
    {
        inputActions = new InputActions();

        //機活動作表
        inputActions.Gameplay.SetCallbacks(this); //動作的回調函數
        inputActions.PauseMenu.SetCallbacks(this); //動作的回調函數
    }

    void OnDisable()
    {
        DisableAllInputs();
    }

    //切換動作表
    void SwitchActionMap(InputActionMap actionMap, bool isUIIput)
    {
        inputActions.Disable();
        actionMap.Enable();

        if (isUIIput)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SwitchToDynamicUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;

    public void SwitchToFixedUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

    //禁用輸入動作
    public void DisableAllInputs()
    {
        inputActions.Disable();
    }

    //啟動輸入遊戲動作
    public void EnableGameplayInput() => SwitchActionMap(inputActions.Gameplay, false);
    //啟動輸入暫停動作
    public void EnablePauseMenuInput() => SwitchActionMap(inputActions.PauseMenu, true);

    //增加按鍵方式
    //先去InputSystem新增Action  回到這邊新增方法即可
    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed) //按住按健
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }
        if (context.canceled) // 鬆開按健
        {
            onStopMove.Invoke();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed) //按住按健
        {
            onFire.Invoke();
        }
        if (context.canceled) // 鬆開按健
        {
            onStopFire.Invoke();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed) //按下按健
        {
            onDodge.Invoke();
        }
    }

    public void OnOverdrive(InputAction.CallbackContext context)
    {
        if (context.performed) //按下按健
        {
            onOverdrive.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) //按下按健
        {
            onPause.Invoke();
        }
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.performed) //按下按健
        {
            onUnpause.Invoke();
        }
    }

    public void OnLunchMissile(InputAction.CallbackContext context)
    {
        if (context.performed) //按下按健
        {
            onLaunchMissile.Invoke();
        }
    }

    public void OnAddHP(InputAction.CallbackContext context)
    {
        if (context.performed) //按下按健
        {
            onAddHP.Invoke();
        }
    }

    public void OnNoHurt(InputAction.CallbackContext context)
    {
        if (context.performed) //按下按健
        {
            onNoHurt.Invoke();
        }
    }
}
