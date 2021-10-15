using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileDisplay : MonoBehaviour
{
    //分數是唯一
    static Text scoreText;
    static Image cooldownImage;

    void Awake()
    {
        scoreText = transform.Find("Amount Text").GetComponent<Text>();
        cooldownImage = transform.Find("cool down image").GetComponent<Image>();
    }

    private void Start()
    {

    }

    public static void UpdateAmountText(int amount) => scoreText.text = amount.ToString();
    public static void UpdateCooldownImage(float FillAmount) => cooldownImage.fillAmount = FillAmount;
}
