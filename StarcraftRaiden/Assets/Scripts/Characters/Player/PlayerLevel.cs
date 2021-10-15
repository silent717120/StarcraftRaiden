using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevel : Singleton<PlayerLevel>
{
    [Header("玩家")]
    [SerializeField] Player player;
    [Header("經驗條")]
    [SerializeField] StateBar_HUD expBar;
    [Header("經驗歸零速度")]
    [SerializeField] float expUpInterval = 0.01f;
    [Header("經驗值表")]
    [SerializeField] int[] expUpNum;

    [Header("等級")]
    [SerializeField] public int level = 1;
    [Header("經驗值")]
    [SerializeField] int exp = 0;
    int extraExp = 0; //升級時多出的exp
    [Header("導彈威力")]
    [SerializeField] public int MissilePower = 1;

    [Header("等級提升UI")]
    [SerializeField] GameObject LevelUpUI;
    [SerializeField] Image LevelUp_OldImage;
    [SerializeField] public Image LevelUp_NewImage;
    [SerializeField] Text LevelUp_text;
    [SerializeField] Sprite[] LevelTextures;
    [Header("解鎖提示UI")]
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

    //獲取經驗值
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

    //升等
    public void UpLevel()
    {
        level++;

        StartCoroutine(nameof(UpResetCoroutine));

        player.RestoreHealth(30); //回血

        LevelUpUI.SetActive(true);
        LevelUp_OldImage.sprite = LevelTextures[level - 2];
        LevelUp_NewImage.sprite = LevelTextures[level - 1];
        LevelUp_NewImage.GetComponentInChildren<Image>().sprite = LevelTextures[level - 1];
        LevelUp_text.text = level.ToString() + " 等";

        LevelUpgrade();
        StartCoroutine(nameof(LevelUpgradeCoroutine));
    }

    //經驗歸0時
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

        //把額外的經驗值用完
        if(extraExp > 0)
        {
            GetExp(extraExp);
        }
    }

    //升級解鎖提示
    IEnumerator LevelUpgradeCoroutine()
    {
        yield return waitTimeLevelUp;

        //提示內容
        UnLockUI.SetActive(true);
        UnLock_text.text = UnLock_Texts[level-1];

        yield return waitTimeUnlock;

        //LevelUpgrade();
    }

    //升級的設定
    void LevelUpgrade()
    {
        switch (level)
        {
            case 1:
                //初始化設定
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
                //射速提升
                player.FireInterval = 0.2f;
                break;
            case 3:
                //威力提升
                player.weaponPower = 1;
                break;
            case 4:
                //追蹤飛彈解鎖
                player.OpenMissileSystem(1);
                MissilePower = 2;
                break;
            case 5:
                //射速提升
                player.FireInterval = 0.18f;
                break;
            case 6:
                //威力提升  //外觀升級
                player.Level2_Skin.SetActive(true);
                player.weaponPower = 2;
                player.moveSpeed = 8; //原本為6
                break;
            case 7:
                //呼叫艦隊解鎖
                player.OpenBattlecruisers(1);
                break;
            case 8:
                //追蹤飛彈二階
                player.OpenMissileSystem(2);
                MissilePower = 3;
                break;
            case 9:
                //威力提升
                player.weaponPower = 3;
                player.dodgeEnergyCost = 20;
                break;
            case 10:
                //射速提升 //自動回血增加
                player.FireInterval = 0.15f;
                player.SetHealthRegenerate(11f,0.08f);
                player.OpenMissileSystem(3);
                MissilePower = 5;
                break;
            case 11:
                //呼叫艦隊二階
                PlayerEnergy.Instance.SetOverdriveInterval(0.24f); //原本為0.12f
                break;
            case 12:
                //威力提升
                player.weaponPower = 4;
                player.dodgeEnergyCost = 15;
                break;
            case 13:
                //射速提升 //自動回血增加
                player.FireInterval = 0.12f;
                player.SetHealthRegenerate(8f, 0.1f);
                break;
            case 14:
                //閃避不消耗能量
                player.dodgeEnergyCost = 0;
                MissilePower = 10;
                break;
        }
        player.UpdateFireSpeed();
    }
}
