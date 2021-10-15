using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{
    [Header("時間變量")]
    [SerializeField,Range(0f,1f)] float bulletTimeScale = 0.1f;

    float defaultFixedDeltaTime;

    float timeScaleBeforePause; //暫停之前的時間刻度

    float t;

    protected override void Awake()
    {
        base.Awake();
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void Pause()
    {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        Time.timeScale = timeScaleBeforePause;
    }

    public void BulletTime(float duration)
    {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(SlowOutCoroutine(duration));
    }

    //逐漸開始跟結束
    public void BulletTime(float inDuration,float outDuration)
    {
        StartCoroutine(SlowInAndOutCoroutine(inDuration,outDuration));
    }

    public void BulletTime(float inDuration,float keepingDuration, float outDuration)
    {
        StartCoroutine(SlowInKeepAndOutDuration(inDuration, keepingDuration, outDuration));
    }

    public void SlowIn(float duration)
    {
        StartCoroutine(SlowInCoroutine(duration));
    }

    public void SlowOut(float duration)
    {
        StartCoroutine(SlowOutCoroutine(duration));
    }

    //緩速持續一段時間
    IEnumerator SlowInKeepAndOutDuration(float inDuration, float keppingDuration,float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        yield return new WaitForSecondsRealtime(keppingDuration); //WaitForSecondsRealtime 真實時間

        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    //緩速開始跟緩速結束
    IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));

        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    //逐漸開始
    IEnumerator SlowInCoroutine(float duration)
    {
        t = 0f;

        while (t < 1f)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration; //unscaledDeltaTime不受時間刻度影響
                Time.timeScale = Mathf.Lerp(1f, bulletTimeScale, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }

            yield return null;
        }
    }

    //逐漸結束
    IEnumerator SlowOutCoroutine(float duration)
    {
        t = 0f;

        while(t < 1f)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration; //unscaledDeltaTime不受時間刻度影響
                Time.timeScale = Mathf.Lerp(bulletTimeScale, 1f, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }

            yield return null;
        }
    }

}
