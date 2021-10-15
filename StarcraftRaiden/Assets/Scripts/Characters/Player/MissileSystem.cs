using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] int defaultAmount = 5;
    [SerializeField] float cooldownTime = 1f;
    [SerializeField] GameObject missilePrefab = null;
    [SerializeField] AudioData laucnchSFX = null;

    bool isReady = true;

    int amount;

    private void Awake()
    {
        amount = defaultAmount;
    }

    private void Start()
    {
        MissileDisplay.UpdateAmountText(amount);
    }

    public void Reloading(int num)
    {
        amount += num;
        MissileDisplay.UpdateAmountText(amount);
        StartCoroutine(CooldownCoroutine());
    }

    //發射
    public void Launch(bool IsCoolDown, bool IsPlaySound, Transform muzzleTransform)
    {
        if (amount == 0 || !isReady) return;

        PoolManager.Release(missilePrefab, muzzleTransform.position);
        //第一次只發射不執行別的，第二次才進入冷確
        if (!IsCoolDown)
        {
            return;
        }

        if (IsPlaySound)
        {
            AudioManager.Instance.PlaySFX(laucnchSFX);
        }

        isReady = false;
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 0)
        {
            MissileDisplay.UpdateCooldownImage(1f);
        }
        else
        {
            StartCoroutine(CooldownCoroutine());
        }
    }
    public void Launch(bool IsCoolDown,bool IsPlaySound,Transform muzzleTransform, Transform muzzleTransform2)
    {
        if (amount == 0 || !isReady) return;

        PoolManager.Release(missilePrefab, muzzleTransform.position);
        PoolManager.Release(missilePrefab, muzzleTransform2.position);
        //第一次只發射不執行別的，第二次才進入冷確
        if (!IsCoolDown)
        {
            return;
        }

        if (IsPlaySound)
        {
            AudioManager.Instance.PlaySFX(laucnchSFX);
        }

        isReady = false;
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 0)
        {
            MissileDisplay.UpdateCooldownImage(1f);
        }
        else
        {
            StartCoroutine(CooldownCoroutine());
        }
    }

    IEnumerator CooldownCoroutine()
    {
        var cooldownValue = cooldownTime;
        while(cooldownValue > 0f)
        {
            MissileDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime,0f);
            yield return null;
        }

        isReady = true;
    }
}
