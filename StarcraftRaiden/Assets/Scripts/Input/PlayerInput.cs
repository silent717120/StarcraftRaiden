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

        //�����ʧ@��
        inputActions.Gameplay.SetCallbacks(this); //�ʧ@���^�ը��
        inputActions.PauseMenu.SetCallbacks(this); //�ʧ@���^�ը��
    }

    void OnDisable()
    {
        DisableAllInputs();
    }

    //�����ʧ@��
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

    //�T�ο�J�ʧ@
    public void DisableAllInputs()
    {
        inputActions.Disable();
    }

    //�Ұʿ�J�C���ʧ@
    public void EnableGameplayInput() => SwitchActionMap(inputActions.Gameplay, false);
    //�Ұʿ�J�Ȱ��ʧ@
    public void EnablePauseMenuInput() => SwitchActionMap(inputActions.PauseMenu, true);

    //�W�[����覡
    //���hInputSystem�s�WAction  �^��o��s�W��k�Y�i
    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed) //�������
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }
        if (context.canceled) // �P�}����
        {
            onStopMove.Invoke();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed) //�������
        {
            onFire.Invoke();
        }
        if (context.canceled) // �P�}����
        {
            onStopFire.Invoke();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed) //���U����
        {
            onDodge.Invoke();
        }
    }

    public void OnOverdrive(InputAction.CallbackContext context)
    {
        if (context.performed) //���U����
        {
            onOverdrive.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) //���U����
        {
            onPause.Invoke();
        }
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.performed) //���U����
        {
            onUnpause.Invoke();
        }
    }

    public void OnLunchMissile(InputAction.CallbackContext context)
    {
        if (context.performed) //���U����
        {
            onLaunchMissile.Invoke();
        }
    }

    public void OnAddHP(InputAction.CallbackContext context)
    {
        if (context.performed) //���U����
        {
            onAddHP.Invoke();
        }
    }

    public void OnNoHurt(InputAction.CallbackContext context)
    {
        if (context.performed) //���U����
        {
            onNoHurt.Invoke();
        }
    }
}
