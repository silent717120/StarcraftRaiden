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

    //�o�g
    public void Launch(bool IsCoolDown, bool IsPlaySound, Transform muzzleTransform)
    {
        if (amount == 0 || !isReady) return;

        PoolManager.Release(missilePrefab, muzzleTransform.position);
        //�Ĥ@���u�o�g������O���A�ĤG���~�i�J�N�T
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
        //�Ĥ@���u�o�g������O���A�ĤG���~�i�J�N�T
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
