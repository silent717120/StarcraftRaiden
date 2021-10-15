using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateBar_HUD : StateBar
{
    [Header("百分比文字")]
    [SerializeField] Text percentText;

    //更新百分比數字
    void SetPercentText()
    {
        percentText.text = Mathf.RoundToInt(targetFillAmount * 100f) + "%";
    }

    public override void Initialized(float currentValue, float maxValue)
    {
        base.Initialized(currentValue, maxValue);
        SetPercentText();
    }

    protected override IEnumerator BufferedFillingCoroutine(Image image)
    {
        SetPercentText();
        return base.BufferedFillingCoroutine(image);
    }
}
