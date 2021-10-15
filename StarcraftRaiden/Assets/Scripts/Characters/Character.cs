using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("死亡特效")]
    [SerializeField] GameObject deathVFX;
    [Header("死亡音效")]
    [SerializeField] protected AudioData[] deathSFX;
    [Header("死亡吼叫音效")]
    [SerializeField] protected AudioData[] deathHowelSFX;
    [Header("生命值")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected bool showOnHeadHealthBar = true; //是否顯示生命條
    [SerializeField] protected StateBar onHeadHealthBar;
    [Header("防護罩")]
    [SerializeField] public bool IsNoHurt = false; //是否為無敵
    [SerializeField] Renderer shieldRender;//防護罩
    [SerializeField] Material shieldMat;//防護罩
    [SerializeField] Color shieldColor;

    protected float health;

    //protected表示子類可訪問此函數  virtual表示子類可覆寫此函數
    protected virtual void OnEnable()
    {
        health = maxHealth;

        if (showOnHeadHealthBar)
        {
            ShowOnHeadHealthBar();
        }
        else
        {
            HideOnHeadHealthBar();
        }

        //重置護盾顯示
        if (shieldRender != null)
        {
            shieldMat.SetColor("_BaseColor", new Color(shieldColor.r, shieldColor.g, shieldColor.b, 0));
            shieldRender.material = shieldMat;
        }
    }

    //啟用生命條
    public void ShowOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialized(health, maxHealth);
    }

    public void HideOnHeadHealthBar()
    {
        if (onHeadHealthBar == null) return;
        onHeadHealthBar.gameObject.SetActive(false);
    }

    //受傷
    public virtual void TakeDamage(float damage)
    {
        if (shieldRender != null)
        {
            StopAllCoroutines();
            StartCoroutine(nameof(StartMatCoroutine));
        }

        //若是無敵
        if (IsNoHurt)
        {
            return;
        }

        health -= damage;

        if (showOnHeadHealthBar && gameObject.activeSelf) //血條更新  需要角色存在
        {
            onHeadHealthBar.UpdateStas(health, maxHealth);
        }

        if(health <= 0f) //死亡
        {
            Die();
        }
    }

    //死亡
    public virtual void Die()
    {
        health = 0f;
        AudioManager.Instance.PlayRandomSFX(deathSFX);
        if(deathHowelSFX.Length != 0) AudioManager.Instance.PlayRandomSFX(deathHowelSFX);
        PoolManager.Release(deathVFX,transform.position);
        gameObject.SetActive(false);
    }

    //補血
    public virtual void RestoreHealth(float value)
    {
        if (health == maxHealth) return;

        health = Mathf.Clamp(health + value, 0f, maxHealth);

        if (showOnHeadHealthBar) //血條更新
        {
            onHeadHealthBar.UpdateStas(health, maxHealth);
        }
    }

    //被動回血  percent 回復百分比
    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime , float percent)
    {
        while(health < maxHealth)
        {
            yield return waitTime;

            RestoreHealth(maxHealth * percent);
        }
    }

    //被動扣血  percent 扣百分比
    protected IEnumerator DamageOverTimeCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (health > 0)
        {
            yield return waitTime;

            TakeDamage(maxHealth * percent);
        }
    }

    IEnumerator StartMatCoroutine()
    {
        Debug.Log(11111);
        float nowEMI = 0f;
        float add = 0.1f;
        Color nowColorEMI = shieldColor;

        while (nowEMI >= 0)
        {
            if (nowEMI >= 0.7f)
            {
                add = -0.1f;
                yield return new WaitForSeconds(0.3f);
            }
            nowEMI += add;

            nowColorEMI = new Color(nowColorEMI.r,nowColorEMI.g,nowColorEMI.b, nowEMI);

             shieldMat.SetColor("_BaseColor", nowColorEMI);
            shieldRender.material = shieldMat;

            yield return new WaitForSeconds(0.01f);
        }
    }
}
