using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevel : Singleton<PlayerLevel>
{
    [Header("���a")]
    [SerializeField] Player player;
    [Header("�g���")]
    [SerializeField] StateBar_HUD expBar;
    [Header("�g���k�s�t��")]
    [SerializeField] float expUpInterval = 0.01f;
    [Header("�g��Ȫ�")]
    [SerializeField] int[] expUpNum;

    [Header("����")]
    [SerializeField] public int level = 1;
    [Header("�g���")]
    [SerializeField] int exp = 0;
    int extraExp = 0; //�ɯŮɦh�X��exp
    [Header("�ɼu�¤O")]
    [SerializeField] public int MissilePower = 1;

    [Header("���Ŵ���UI")]
    [SerializeField] GameObject LevelUpUI;
    [SerializeField] Image LevelUp_OldImage;
    [SerializeField] public Image LevelUp_NewImage;
    [SerializeField] Text LevelUp_text;
    [SerializeField] Sprite[] LevelTextures;
    [Header("���괣��UI")]
    [SerializeField] GameObject UnLockUI;
    [SerializeField] Text UnLock_text;
    [SerializeField] string[] UnLock_Texts;

    WaitForSeconds waitForExpInterval;

    WaitForSeconds waitTimeLevelUp;
    WaitForSeconds waitTimeUnlock;

    protected override void Awake()
    {
        base.Awake();
        waitForExpInterval = new WaitForSeconds(expUpInterval);

        waitTimeLevelUp = new WaitForSeconds(4f);

        waitTimeUnlock = new WaitForSeconds(1.2f);
    }

    private void Start()
    {
        expBar.Initialized(exp, expUpNum[level-1]);

        LevelUpgrade();
    }

    //����g���
    public void GetExp(int Value)
    {
        if (!gameObject.activeSelf) return;

        exp += Value;

        extraExp = 0;
        if (exp > expUpNum[level - 1])
        {
            extraExp = exp - expUpNum[level - 1];
        }

        exp = Mathf.Clamp(exp, 0, expUpNum[level - 1]);

        if (exp >= expUpNum[level - 1])
        {
            UpLevel();
        }
        else
        {
            expBar.UpdateStas(exp, expUpNum[level - 1]);
        }
    }

    //�ɵ�
    public void UpLevel()
    {
        level++;

        StartCoroutine(nameof(UpResetCoroutine));

        player.RestoreHealth(30); //�^��

        LevelUpUI.SetActive(true);
        LevelUp_OldImage.sprite = LevelTextures[level - 2];
        LevelUp_NewImage.sprite = LevelTextures[level - 1];
        LevelUp_NewImage.GetComponentInChildren<Image>().sprite = LevelTextures[level - 1];
        LevelUp_text.text = level.ToString() + " ��";

        LevelUpgrade();
        StartCoroutine(nameof(LevelUpgradeCoroutine));
    }

    //�g���k0��
    IEnumerator UpResetCoroutine()
    {
        expBar.UpdateStas(exp, expUpNum[level - 2]);

        yield return new WaitForSeconds(0.5f);

        while (gameObject.activeSelf && exp > 0)
        {
            yield return waitForExpInterval;

            exp -= 10;
            exp = Mathf.Clamp(exp, 0, expUpNum[level - 2]);

            expBar.UpdateStas(exp, expUpNum[level - 2]);
        }

        //���B�~���g��ȥΧ�
        if(extraExp > 0)
        {
            GetExp(extraExp);
        }
    }

    //�ɯŸ��괣��
    IEnumerator LevelUpgradeCoroutine()
    {
        yield return waitTimeLevelUp;

        //���ܤ��e
        UnLockUI.SetActive(true);
        UnLock_text.text = UnLock_Texts[level-1];

        yield return waitTimeUnlock;

        //LevelUpgrade();
    }

    //�ɯŪ��]�w
    void LevelUpgrade()
    {
        switch (level)
        {
            case 1:
                //��l�Ƴ]�w
                player.weaponPower = 0;
                player.FireInterval = 0.25f;
                player.moveSpeed = 6;
                player.dodgeEnergyCost = 25;
                MissilePower = 1;
                PlayerEnergy.Instance.SetOverdriveInterval(0.12f);
                player.SetHealthRegenerate(12f, 0.02f);
                player.OpenMissileSystem(0);
                player.OpenBattlecruisers(0);
                break;
            case 2:
                //�g�t����
                player.FireInterval = 0.2f;
                break;
            case 3:
                //�¤O����
                player.weaponPower = 1;
                break;
            case 4:
                //�l�ܭ��u����
                player.OpenMissileSystem(1);
                MissilePower = 2;
                break;
            case 5:
                //�g�t����
                player.FireInterval = 0.18f;
                break;
            case 6:
                //�¤O����  //�~�[�ɯ�
                player.Level2_Skin.SetActive(true);
                player.weaponPower = 2;
                player.moveSpeed = 8; //�쥻��6
                break;
            case 7:
                //�I�sĥ������
                player.OpenBattlecruisers(1);
                break;
            case 8:
                //�l�ܭ��u�G��
                player.OpenMissileSystem(2);
                MissilePower = 3;
                break;
            case 9:
                //�¤O����
                player.weaponPower = 3;
                player.dodgeEnergyCost = 20;
                break;
            case 10:
                //�g�t���� //�۰ʦ^��W�[
                player.FireInterval = 0.15f;
                player.SetHealthRegenerate(11f,0.08f);
                player.OpenMissileSystem(3);
                MissilePower = 5;
                break;
            case 11:
                //�I�sĥ���G��
                PlayerEnergy.Instance.SetOverdriveInterval(0.24f); //�쥻��0.12f
                break;
            case 12:
                //�¤O����
                player.weaponPower = 4;
                player.dodgeEnergyCost = 15;
                break;
            case 13:
                //�g�t���� //�۰ʦ^��W�[
                player.FireInterval = 0.12f;
                player.SetHealthRegenerate(8f, 0.1f);
                break;
            case 14:
                //�{�פ����ӯ�q
                player.dodgeEnergyCost = 0;
                MissilePower = 10;
                break;
        }
        player.UpdateFireSpeed();
    }
}
