using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    [Header("�H�J�H�X�Ϥ�")]
    [SerializeField] Image transitionImage;
    [Header("�H�J�H�X�ɶ�")]
    [SerializeField] float fadeTime = 3.5f;

    Color color;

    const string GAMEPLAY = "GamePlay";
    const string MAIN_MENU = "MainMenu";

   void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //����ĪG
    IEnumerator LoadingCoroutine(string sceneName)
    {
        //��x���i������[��
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        //�]�w�[���n�������O�_�E��
        loadingOperation.allowSceneActivation = false;

        transitionImage.gameObject.SetActive(true);

        while(color.a < 1f)
        {
            //Mathf.Clamp01 ���� alpha�Ȧb0~1����
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime); //unscaledDeltaTime ����timeScale�v�T
            transitionImage.color = color;

            yield return null;
        }

        //�[�������A���J
        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);

        loadingOperation.allowSceneActivation = true;

        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime); //unscaledDeltaTime ����timeScale�v�T
            transitionImage.color = color;

            yield return null;
        }
        transitionImage.gameObject.SetActive(false);
    }

    public void LoadGameplayScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(GAMEPLAY));
    }

    public void LoadMainMenuSence()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(MAIN_MENU));
    }
}
