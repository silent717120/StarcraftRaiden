using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateBar : MonoBehaviour
{
    [Header("�����")]
    [SerializeField] Image fillImageBack;
    [SerializeField] Image fillImageFront;
    [Header("�O�_����")]
    [SerializeField] bool delayFill = true;
    [Header("����ɶ�")]
    [SerializeField] float fillDelay = 0.5f;
    [Header("��R�t��")]
    [SerializeField] float fillSpeed = 0.1f;

    //�����ƶq
    public float currentFillAmount;

    protected float targetFillAmount;

    float previousFillAmount;

    float t;

    WaitForSeconds waitForDelayFill;
    Coroutine bufferedFillCoroutine; 

    Canvas canvas;

    private void Awake()
    {
        if(TryGetComponent<Canvas>(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void Initialized(float currentValue, float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateStas(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        //���ΤW����{
        if (bufferedFillCoroutine != null)
        {
            StopCoroutine(bufferedFillCoroutine);
        }

        //�����
        if(currentFillAmount > targetFillAmount)
        {
            fillImageFront.fillAmount = targetFillAmount;
            bufferedFillCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
        }
        //�ɦ��
        else if (currentFillAmount < targetFillAmount)
        {
            fillImageBack.fillAmount = targetFillAmount;

            bufferedFillCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }
    }

    protected virtual IEnumerator BufferedFillingCoroutine(Image image)
    {
        if (delayFill)
        {
            yield return waitForDelayFill;
        }

        previousFillAmount = currentFillAmount;
        t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }
    }
}
