using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [Header("能量條")]
    [SerializeField]EnergyBar energyBar;
    [Header("能量爆發消耗速度")]
    [SerializeField] float overdriveInterval = 0.1f;
    [Header("能量全滿音效")]
    [SerializeField] AudioData energyMaxSFX;
    //是否可獲得能量
    bool available = true;

    public const int MAX = 100; //能量最大值
    public const int PERCENT = 1; //百分比
    public int ProjectlieEnenrgy = 0; //每射15下累積能量1點

    int energy; //當前能量值

    Player player;

    WaitForSeconds waitForOverdriveInterval;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        waitForOverdriveInterval = new WaitForSeconds(overdriveInterval);
    }

    private void OnEnable()
    {
        PlayerOverDrive.on += PlayerOverdriveOn;
        PlayerOverDrive.off += PlayerOverdriveOff;
    }

    private void OnDisable()
    {
        PlayerOverDrive.on -= PlayerOverdriveOn;
        PlayerOverDrive.off -= PlayerOverdriveOff;
    }

    private void Start()
    {
        energyBar.Initialized(energy, MAX);
        //Obtain(MAX);
    }

    //擊中累積能量(15下 換 1點能量)
    public void ProjectlieObtain(int Value)
    {
        //能量全滿  / 能量爆發中 /  玩家死亡 / 都不可獲得能量
        if (energy == MAX || !available || !gameObject.activeSelf) return;

        ProjectlieEnenrgy++;
        if(ProjectlieEnenrgy >= 15)
        {
            ProjectlieEnenrgy = 0;
            Obtain(PERCENT);
        }
    }

    //獲取能量
    public void Obtain(int Value)
    {
        //能量全滿  / 能量爆發中 /  玩家死亡 / 都不可獲得能量
        if (energy == MAX || !available || !gameObject.activeSelf) return;

        energy += Value;
        energy = Mathf.Clamp(energy, 0, MAX);

        energyBar.UpdateStas(energy, MAX);

        //呼叫艦隊
        if (energy == MAX && !player.overdriveLocked)
        {
            AudioManager.Instance.PlaySFX(energyMaxSFX);
            player.BattlecruisersShowUI.fillAmount = 0;
        }
    }

    //使用能量
    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStas(energy, MAX);

        //能量爆發用盡時關閉
        if(energy == 0 && !available)
        {
            PlayerOverDrive.off.Invoke();
        }

        //呼叫艦隊
        if (!player.overdriveLocked)
        {
            player.BattlecruisersShowUI.fillAmount = 1;
        }
    }

    //是否能量足夠消耗
    public bool IsEnough(int value) => energy >= value;

    void PlayerOverdriveOn()
    {
        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    void PlayerOverdriveOff()
    {
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }

    //能量爆發逐漸消耗能量
    IEnumerator KeepUsingCoroutine()
    {
        while(gameObject.activeSelf && energy > 0)
        {
            yield return waitForOverdriveInterval;

            Use(PERCENT);
        }
    }

    public void SetOverdriveInterval(float speed)
    {
        overdriveInterval = speed;
        waitForOverdriveInterval = new WaitForSeconds(overdriveInterval);
    }
}
