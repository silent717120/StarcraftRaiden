using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header("���s")]
    [SerializeField] Button StartGameButton;
    [SerializeField] Button RankingButton;
    [SerializeField] Button RankingBackButton;
    [SerializeField] Button TeachingButton;
    [SerializeField] Button TeachingBackButton;
    [SerializeField] Button ExitGameButton;

    [Header("UI�e��")]
    [SerializeField] Canvas MainCanvas;
    [SerializeField] Canvas RankingCanvas;
    [SerializeField] Canvas TeachingCanvas;

    [Header("�ާ@�y��")]
    [SerializeField] AudioData TeacgingAudio;

    [SerializeField] InputSystemUIInputModule UIInputModule;

    private void OnEnable()
    {
        UIInputModule.enabled = true;
        ButtonPressedBehavior.buttonFunctionTable.Add(StartGameButton.gameObject.name, OnStartGameButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(RankingButton.gameObject.name, OnRankingButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(RankingBackButton.gameObject.name, OutRankingButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(TeachingButton.gameObject.name, OnTeachingButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(TeachingBackButton.gameObject.name, OutTeachingButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(ExitGameButton.gameObject.name, OnExitGameButtonClick);
    }

    private void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    private void Start()
    {
        Time.timeScale = 1f;

        AudioManager.Instance.SetMusic(0);
    }

    //�}�l�C��
    void OnStartGameButtonClick()
    {
        MainCanvas.enabled = false;
        UIInputModule.enabled = false;
        SceneLoader.Instance.LoadGameplayScene();
    }

    //�Ʀ�]
    void OnRankingButtonClick()
    {
        MainCanvas.enabled = false;
        RankingCanvas.enabled = true;

        UIInputModule.enabled = true;
    }

    void OutRankingButtonClick()
    {
        MainCanvas.enabled = true;
        RankingCanvas.enabled = false;

        UIInputModule.enabled = true;
    }

    //�ާ@���n
    void OnTeachingButtonClick()
    {
        MainCanvas.enabled = false;
        TeachingCanvas.enabled = true;

        UIInputModule.enabled = true;

        AudioManager.Instance.PlayAudioStatic(TeacgingAudio);
    }

    void OutTeachingButtonClick()
    {
        MainCanvas.enabled = true;
        TeachingCanvas.enabled = false;

        UIInputModule.enabled = true;

        AudioManager.Instance.StopAllAudio();
    }

    //���}�C��
    void OnExitGameButtonClick()
    {
        Application.Quit();
    }
}
