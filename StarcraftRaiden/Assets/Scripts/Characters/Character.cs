using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("���`�S��")]
    [SerializeField] GameObject deathVFX;
    [Header("���`����")]
    [SerializeField] protected AudioData[] deathSFX;
    [Header("���`�q�s����")]
    [SerializeField] protected AudioData[] deathHowelSFX;
    [Header("�ͩR��")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected bool showOnHeadHealthBar = true; //�O�_��ܥͩR��
    [SerializeField] protected StateBar onHeadHealthBar;
    [Header("���@�n")]
    [SerializeField] public bool IsNoHurt = false; //�O�_���L��
    [SerializeField] Renderer shieldRender;//���@�n
    [SerializeField] Material shieldMat;//���@�n
    [SerializeField] Color shieldColor;

    protected float health;

    //protected��ܤl���i�X�ݦ����  virtual��ܤl���i�мg�����
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

        //���m�@�����
        if (shieldRender != null)
        {
            shieldMat.SetColor("_BaseColor", new Color(shieldColor.r, shieldColor.g, shieldColor.b, 0));
            shieldRender.material = shieldMat;
        }
    }

    //�ҥΥͩR��
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

    //����
    public virtual void TakeDamage(float damage)
    {
        if (shieldRender != null)
        {
            StopAllCoroutines();
            StartCoroutine(nameof(StartMatCoroutine));
        }

        //�Y�O�L��
        if (IsNoHurt)
        {
            return;
        }

        health -= damage;

        if (showOnHeadHealthBar && gameObject.activeSelf) //�����s  �ݭn����s�b
        {
            onHeadHealthBar.UpdateStas(health, maxHealth);
        }

        if(health <= 0f) //���`
        {
            Die();
        }
    }

    //���`
    public virtual void Die()
    {
        health = 0f;
        AudioManager.Instance.PlayRandomSFX(deathSFX);
        if(deathHowelSFX.Length != 0) AudioManager.Instance.PlayRandomSFX(deathHowelSFX);
        PoolManager.Release(deathVFX,transform.position);
        gameObject.SetActive(false);
    }

    //�ɦ�
    public virtual void RestoreHealth(float value)
    {
        if (health == maxHealth) return;

        health = Mathf.Clamp(health + value, 0f, maxHealth);

        if (showOnHeadHealthBar) //�����s
        {
            onHeadHealthBar.UpdateStas(health, maxHealth);
        }
    }

    //�Q�ʦ^��  percent �^�_�ʤ���
    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime , float percent)
    {
        while(health < maxHealth)
        {
            yield return waitTime;

            RestoreHealth(maxHealth * percent);
        }
    }

    //�Q�ʦ���  percent ���ʤ���
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
