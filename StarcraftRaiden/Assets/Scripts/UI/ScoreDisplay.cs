using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    //分數是唯一
    static Text scoreText;

    void Awake()
    {
        scoreText = GetComponent<Text>();
    }

    void Start()
    {
        ScoreManager.Instance.ResetScore();
    }

    public static void UpdateText(int score) => scoreText.text = score.ToString();

    public static void ScaleText(Vector3 targetScale) => scoreText.rectTransform.localScale = targetScale;
}
