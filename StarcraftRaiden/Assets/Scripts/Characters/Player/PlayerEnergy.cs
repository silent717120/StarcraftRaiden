using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [Header("��q��")]
    [SerializeField]EnergyBar energyBar;
    [Header("��q�z�o���ӳt��")]
    [SerializeField] float overdriveInterval = 0.1f;
    [Header("��q��������")]
    [SerializeField] AudioData energyMaxSFX;
    //�O�_�i��o��q
    bool available = true;

    public const int MAX = 100; //��q�̤j��
    public const int PERCENT = 1; //�ʤ���
    public int ProjectlieEnenrgy = 0; //�C�g15�U�ֿn��q1�I

    int energy; //��e��q��

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

    //�����ֿn��q(15�U �� 1�I��q)
    public void ProjectlieObtain(int Value)
    {
        //��q����  / ��q�z�o�� /  ���a���` / �����i��o��q
        if (energy == MAX || !available || !gameObject.activeSelf) return;

        ProjectlieEnenrgy++;
        if(ProjectlieEnenrgy >= 15)
        {
            ProjectlieEnenrgy = 0;
            Obtain(PERCENT);
        }
    }

    //�����q
    public void Obtain(int Value)
    {
        //��q����  / ��q�z�o�� /  ���a���` / �����i��o��q
        if (energy == MAX || !available || !gameObject.activeSelf) return;

        energy += Value;
        energy = Mathf.Clamp(energy, 0, MAX);

        energyBar.UpdateStas(energy, MAX);

        //�I�sĥ��
        if (energy == MAX && !player.overdriveLocked)
        {
            AudioManager.Instance.PlaySFX(energyMaxSFX);
            player.BattlecruisersShowUI.fillAmount = 0;
        }
    }

    //�ϥί�q
    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStas(energy, MAX);

        //��q�z�o�κɮ�����
        if(energy == 0 && !available)
        {
            PlayerOverDrive.off.Invoke();
        }

        //�I�sĥ��
        if (!player.overdriveLocked)
        {
            player.BattlecruisersShowUI.fillAmount = 1;
        }
    }

    //�O�_��q��������
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

    //��q�z�o�v�����ӯ�q
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
