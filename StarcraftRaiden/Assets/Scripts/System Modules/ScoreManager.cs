using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    //真正分數
    public int score;
    //動態得分
    int currentScore;

    Vector3 scoreTextScale = new Vector3(1.2f, 1.2f, 1f);
    
    public void ResetScore()
    {
        score = 0;
        currentScore = 0;
        ScoreDisplay.UpdateText(score);
    }

    public void AddScore(int scorePoint)
    {
        currentScore += (int)Mathf.Round((float)scorePoint * GameManager.Instance.EnemyPower); //計算強度後四捨五入
        StartCoroutine(nameof(AddScoreCoroutine));
    }

    //動態增加分數
    IEnumerator AddScoreCoroutine()
    {
        ScoreDisplay.ScaleText(scoreTextScale);
        while(score < currentScore)
        {
            score += 1;
            ScoreDisplay.UpdateText(score);

            yield return null;
        }

        ScoreDisplay.ScaleText(Vector3.one);
    }
}
