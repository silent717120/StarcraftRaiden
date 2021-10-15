using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    [Header("淡入淡出圖片")]
    [SerializeField] Image transitionImage;
    [Header("淡入淡出時間")]
    [SerializeField] float fadeTime = 3.5f;

    Color color;

    const string GAMEPLAY = "GamePlay";
    const string MAIN_MENU = "MainMenu";

   void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //轉場效果
    IEnumerator LoadingCoroutine(string sceneName)
    {
        //後台先進行場景加載
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        //設定加載好的場景是否激活
        loadingOperation.allowSceneActivation = false;

        transitionImage.gameObject.SetActive(true);

        while(color.a < 1f)
        {
            //Mathf.Clamp01 限制 alpha值在0~1之間
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime); //unscaledDeltaTime 不受timeScale影響
            transitionImage.color = color;

            yield return null;
        }

        //加載完成再載入
        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);

        loadingOperation.allowSceneActivation = true;

        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime); //unscaledDeltaTime 不受timeScale影響
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
