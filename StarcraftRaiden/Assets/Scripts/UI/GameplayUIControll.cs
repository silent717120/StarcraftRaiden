using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIControll : MonoBehaviour
{
    [Header("��J����")]
    [SerializeField] PlayerInput playerInput;
    [Header("����")]
    [SerializeField] AudioData pauseSFX;
    [SerializeField] AudioData unpauseSFX;
    [SerializeField] AudioData pauseSFX2;
    [SerializeField] AudioData unpauseSFX2;
    [Header("UI�e��")]
    [SerializeField] Canvas HUDCanvas;
    [SerializeField] Canvas menusCanvas;
    [SerializeField] Canvas PauseMenu;
    [SerializeField] Canvas OptionMenu;
    [Header("���s")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button optionsBackButton;
    [Header("�n������")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider otherSlider;

    int buttonPressedParameterID = Animator.StringToHash("Pressed");

    void OnEnable()
    {
        playerInput.onPause += Pause;
        playerInput.onUnpause += Unpause;

        //���s�ƥ�
        //resumeButton.onClick.AddListener(OnResumeButtonClick);
        //optionsButton.onClick.AddListener(OnOptionButtonClick);
        //mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);

        //���s��ʵe������ƥ�
        ButtonPressedBehavior.buttonFunctionTable.Add(resumeButton.gameObject.name,OnResumeButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(optionsButton.gameObject.name, OnOptionButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(mainMenuButton.gameObject.name, OnMainMenuButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(optionsBackButton.gameObject.name, OnOptionBackButtonClick);

        PauseMenu.enabled = true;
        OptionMenu.enabled = false;
    }

    void OnDisable()
    {
        playerInput.onPause -= Pause;
        playerInput.onUnpause -= Unpause;

        //resumeButton.onClick.RemoveAllListeners();
        //optionsButton.onClick.RemoveAllListeners();
        //mainMenuButton.onClick.RemoveAllListeners();
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    void Start()
    {
        AudioManager.Instance.SetMusic(1);
    }

    void Pause()
    {
        Debug.Log(GameManager.Instance.IsCaptionEnd);
        if (!GameManager.Instance.IsCaptionEnd) return;  //�]�r���ɤ����
        HUDCanvas.enabled = false;
        menusCanvas.enabled = true;
        GameManager.GameState = GameState.Paused;
        TimeController.Instance.Pause();
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);
        AudioManager.Instance.PlaySFX(pauseSFX);
        AudioManager.Instance.PlaySFX(pauseSFX2);
    }

    //���U��_��
    public void Unpause()
    {
        resumeButton.Select();
        resumeButton.animator.SetTrigger(buttonPressedParameterID);
        AudioManager.Instance.PlaySFX(unpauseSFX);
        AudioManager.Instance.PlaySFX(unpauseSFX2);
    }

    //��^�C��(�����Ȱ�)
    void OnResumeButtonClick()
    {
        HUDCanvas.enabled = true;
        menusCanvas.enabled = false;
        GameManager.GameState = GameState.Playing;
        TimeController.Instance.Unpause();
        playerInput.EnableGameplayInput();
        playerInput.SwitchToFixedUpdateMode();
    }

    //�]�m
    void OnOptionButtonClick()
    {
        UIInput.Instance.SelectUI(optionsButton);
        playerInput.EnablePauseMenuInput();

        PauseMenu.enabled = false;
        OptionMenu.enabled = true;
    }

    //�]�m��^
    void OnOptionBackButtonClick()
    {
        UIInput.Instance.SelectUI(optionsBackButton);
        playerInput.EnablePauseMenuInput();

        PauseMenu.enabled = true;
        OptionMenu.enabled = false;
    }

    public void OnMusicValueChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value * 2f);
    }
    public void OnOtherValueChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value * 2f);
    }

    //�^��D���
    void OnMainMenuButtonClick()
    {
        menusCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuSence();
    }
}
