using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header("按鈕")]
    [SerializeField] Button StartGameButton;
    [SerializeField] Button RankingButton;
    [SerializeField] Button RankingBackButton;
    [SerializeField] Button TeachingButton;
    [SerializeField] Button TeachingBackButton;
    [SerializeField] Button ExitGameButton;

    [Header("UI畫布")]
    [SerializeField] Canvas MainCanvas;
    [SerializeField] Canvas RankingCanvas;
    [SerializeField] Canvas TeachingCanvas;

    [Header("操作語音")]
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

    //開始遊戲
    void OnStartGameButtonClick()
    {
        MainCanvas.enabled = false;
        UIInputModule.enabled = false;
        SceneLoader.Instance.LoadGameplayScene();
    }

    //排行榜
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

    //操作指南
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

    //離開遊戲
    void OnExitGameButtonClick()
    {
        Application.Quit();
    }
}
