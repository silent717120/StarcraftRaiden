using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [Header("�ͩR��")]
    [SerializeField] StateBar_HUD statsBar_HUD;
    [Header("�O�_��q�A��")]
    [SerializeField]  bool regenerateHealth = true;
    [Header("��q�A�Ͷ��j")]
    [SerializeField] float healthRegenerateTime;
    [Header("��q�A�ͦʤ���")]
    [SerializeField,Range(0f,1f)] float healthRegeneratePercent;

    [Header("��J�]�w")]
    [SerializeField]PlayerInput input;

    [Header("���ʳt��")]
    [SerializeField]public float moveSpeed = 10f;
    [Header("�[��t��")]
    [SerializeField] float acclerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;
    [Header("���ʱ��ਤ��")]
    [SerializeField] float moveRotationAngle = 50f;

    [Header("�l�u�w�s��")]
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;
    [SerializeField] GameObject projectile4;
    [SerializeField] GameObject projectile5;
    [SerializeField] GameObject projectileOverdrive;
    [Header("�l�u�o�g��m")]
    [SerializeField] Transform muzzleMiddle;
    [SerializeField] Transform muzzleLeft;
    [SerializeField] Transform muzzleRight;
    [SerializeField] Transform muzzleLeft2;
    [SerializeField] Transform muzzleRight2;
    [Header("�o�g����")]
    [SerializeField] AudioData projectileLaunchSFX;
    [Header("�Z���¤O(����)")]
    [SerializeField, Range(0,4)] public int weaponPower = 0;

    [Header("�g���t��")]
    public float FireInterval = 0.2f;


    [Header("�{�׭���")]
    [SerializeField] AudioData dodgeSFX;
    [Header("�{�ׯ�q���ӭ�")]
    [SerializeField,Range(0,100)]public int dodgeEnergyCost = 25;
    [Header("�{�װ��  �{�׳t��")]
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f,0.5f,0.5f);

    [Header("��q�z�o")]
    [SerializeField]public bool overdriveLocked = true;
    [SerializeField]int overdriveDodgeFactor = 2;
    [SerializeField]float overdriveSpeedFactor = 1.2f;
    [SerializeField]float overdriveFireFactor = 1.2f;
    [SerializeField] AudioData energyNoEnoughSFX;

    [Header("�I�sĥ��")]
    [SerializeField] GameObject BattlecruisersUI;
    [SerializeField] public Image BattlecruisersShowUI;
    [SerializeField] GameObject Battlecruisers;
    [SerializeField] Transform[] muzzleBattlecruisers;
    [SerializeField] AudioData BattlecruiserSFX;
    [SerializeField] AudioData[] BattlecruiserLaunchSFX;

    [Header("�ɼu�\��")]
    [SerializeField] GameObject MissileUI;
    [SerializeField] float MissileTime;
    [SerializeField] GameObject[] MissileShip;
    [SerializeField] Transform[] muzzleMissile;
    [SerializeField] int Missilelevel;
    [SerializeField] bool IsMissileLocked = true;

    [Header("�ɯť~�[")]
    [SerializeField] public GameObject Level2_Skin;

    [Header("�@���\��")]
    [SerializeField] bool IsCheat = true;
    [SerializeField] AudioData CheatAudio;

    bool isDodging = false; //���b�{�פ�

    bool isOverdriving = false; //���b��q�z�o

    readonly float slowMotionDuration = 0.5f; //�l�u�w�t�ɶ�

    //������ɽd��
    float paddingX;
    float paddingY;

    //��e���ਤ
    float currentRoll;

    //½�u����ɶ�
    float dodgeDuration;

    float t;
    Vector2 previousVelocity;
    Quaternion previousRotation;

    //��Ū��
    WaitForSeconds waitForFireInterval; //���q�g���ɶ�
    WaitForSeconds waitForOverdriveFireInterval; //�z�o�g���ɶ�
    WaitForSeconds waitForBattlecruiserInterval; //ĥ���g���p��
    WaitForSeconds waitForBattlecruiserAudioInterval; //ĥ���g���p���n��
    WaitForSeconds waitHealthRegenerateTime; //�^��ɶ�
    WaitForSeconds waitDecelerationTime; //���ݴ�t�ɶ�
    WaitForSeconds waitMissileTime; //�ɼu�o�g�ɶ�

    //��
    Coroutine moveCoroutine;
    Coroutine healthRegenerateCoroutine;

    //�I����
    new Rigidbody2D rigidbody;
    new Collider2D collider;

    //�ɼu�t��
    MissileSystem missile;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();

        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;

        dodgeDuration = maxRoll / rollSpeed;

        rigidbody.gravityScale = 0f;

        waitForFireInterval = new WaitForSeconds(FireInterval); //���q�g���p��
        waitForOverdriveFireInterval = new WaitForSeconds(FireInterval / overdriveFireFactor); //��q�z�o�g���p��
        waitForBattlecruiserInterval = new WaitForSeconds(0.025f); //ĥ���g���p��
        waitForBattlecruiserAudioInterval = new WaitForSeconds(0.2f); //ĥ���g���n���p��
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime); //�]�w�^��p��
        waitDecelerationTime = new WaitForSeconds(decelerationTime); //�]�w��t�p��
        waitMissileTime = new WaitForSeconds(MissileTime);
    }
    protected override void OnEnable()
    {
        base.OnEnable(); //�u�ΰ������\��

        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;
        input.onLaunchMissile += LaunchMissile;

        input.onAddHP += AddHP;
        input.onNoHurt += NoHurt;

        PlayerOverDrive.on += OverdriveOn;
        PlayerOverDrive.off += OverdriveOff;
    }

    void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverdrive -= Overdrive;
        input.onLaunchMissile -= LaunchMissile;

        input.onAddHP += AddHP;
        input.onNoHurt += NoHurt;

        PlayerOverDrive.on -= OverdriveOn;
        PlayerOverDrive.off -= OverdriveOff;
    }

    void Start()
    {
        statsBar_HUD.Initialized(health, maxHealth); //��l�Ʀ��

        input.EnableGameplayInput();
    }

    //��s�g���]�w
    public void UpdateFireSpeed()
    {
        waitForFireInterval = new WaitForSeconds(FireInterval); //�]�w�}���p��
        waitForOverdriveFireInterval = new WaitForSeconds(FireInterval / overdriveFireFactor); //�]�w�}���p��
        waitMissileTime = new WaitForSeconds(MissileTime);
    }

    //��o�D��
    public void GetItem(int ItemType,int Num)
    {
        if(ItemType == 0)
        {
            RestoreHealth(Num);
        }
        else if(ItemType == 1)
        {
            PlayerEnergy.Instance.Obtain(Num);
        }
        else if (ItemType == 2)
        {
            missile.Reloading(Num);
        }
    }

    //����
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);  //�u�ΰ������\��
        statsBar_HUD.UpdateStas(health, maxHealth); //��s���
        //�w�t�ɶ�
        //TimeController.Instance.BulletTime(slowMotionDuration);
        //���Y�_��
        CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);

        //���˫�}�l����Q�ʦ^��
        if (gameObject.activeSelf)
        {
            if (regenerateHealth)
            {
                if(healthRegenerateCoroutine != null)
                {
                    StopCoroutine(healthRegenerateCoroutine);
                }
                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    //�]�w�۰ʦ^�_�ƭ�
    public void SetHealthRegenerate(float RTime,float RPercent)
    {
        healthRegenerateTime = RTime;
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime); //�]�w�^��p��
        healthRegeneratePercent = RPercent;

        if (gameObject.activeSelf)
        {
            if (regenerateHealth)
            {
                if (healthRegenerateCoroutine != null) // ���X�{��ܥ��b����A�h��s�Y�i
                {
                    StopCoroutine(healthRegenerateCoroutine);
                    healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
                }
            }
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        statsBar_HUD.UpdateStas(health, maxHealth);
    }

    //���`
    public override void Die()
    {
        GameManager.Instance.End();
        statsBar_HUD.UpdateStas(0f, maxHealth);
        base.Die();
    }

    #region ����
    void Move(Vector2 moveInput)
    {
        if (GameManager.GameState == GameState.GameOver) return;
        //����W�@�өI�s
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        //��J�H�� * ���ʳt�� = �[�t�פ�V
        Quaternion moveRotation = Quaternion.AngleAxis(moveRotationAngle * moveInput.x, Vector2.down);
        moveCoroutine = StartCoroutine(MoveCoroutine(acclerationTime,moveInput.normalized * moveSpeed, moveRotation));
        //�u���b���ʮɤ~�h�I�s�P�_�d��
        StopCoroutine(nameof(DecelerationCoroutine));
        StartCoroutine(nameof(MoveRangeLimatationCoroutine));
    }

    void StopMove()
    {
        //����W�@�өI�s
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        //��J�H�� * ���ʳt�� = �[�t�פ�V
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime,Vector2.zero,Quaternion.identity));
        //����������I�s�P�_�d��
        StartCoroutine(nameof(DecelerationCoroutine));
    }

    //���ʨ�
    IEnumerator MoveCoroutine(float time,Vector2 moveVelocity,Quaternion moveRotation)
    {
        t = 0f;
        previousVelocity = rigidbody.velocity;
        previousRotation = transform.rotation;

        while ( t < 1f)
        {
            t += Time.fixedDeltaTime / time;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t);
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t);

            yield return new WaitForFixedUpdate();
        }
    }

    //���ʭ����
    IEnumerator MoveRangeLimatationCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position,paddingX,paddingY);

            yield return null;
        }
    }

    //��t��
    IEnumerator DecelerationCoroutine()
    {
        yield return waitDecelerationTime;

        //�����t�������I�s�P�_�d��
        StopCoroutine(nameof(MoveRangeLimatationCoroutine));
    }
    #endregion

    #region �g��
    void Fire()
    {
        if (GameManager.GameState == GameState.GameOver) return;
        if (isDodging) return;
        StartCoroutine(nameof(FireCoroutine));
    }
    void StopFire()
    {
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine()
    {
        //1���l�u���歸  2 ���W  3���U
        while (true)
        {
            if (isDodging) yield break;
            switch (weaponPower)
            {
                case 0:
                    //PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    PoolManager.Release(projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(projectile1, muzzleLeft.position);
                    PoolManager.Release(projectile1, muzzleRight.position);
                    break;
                case 2:
                    PoolManager.Release(projectile1, muzzleMiddle.position);
                    PoolManager.Release(projectile2, muzzleLeft.position);
                    PoolManager.Release(projectile3, muzzleRight.position);
                    break;
                case 3:
                    PoolManager.Release(projectile1, muzzleMiddle.position);
                    PoolManager.Release(projectile2, muzzleLeft.position);
                    PoolManager.Release(projectile3, muzzleRight.position);
                    PoolManager.Release(projectile4, muzzleLeft2.position);
                    PoolManager.Release(projectile5, muzzleRight2.position);
                    break;
                case 4:
                    PoolManager.Release(projectile1, muzzleMiddle.position);
                    PoolManager.Release(projectile2, muzzleLeft.position);
                    PoolManager.Release(projectile3, muzzleRight.position);
                    PoolManager.Release(projectile4, muzzleLeft2.position);
                    PoolManager.Release(projectile5, muzzleRight2.position);
                    PoolManager.Release(projectileOverdrive, muzzleMiddle.position);
                    break;
                default:
                    break;
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);

            //�O�_��q�z�o���A�A�M�w���j�ɶ�
            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;
            /*
            if (isOverdriving)
            {
                yield return waitForOverdriveFireInterval;
            }
            else
            {
                yield return waitForFireInterval;
            }*/
        }
    }
    #endregion

    #region �{��
    void Dodge()
    {
        if (GameManager.GameState == GameState.GameOver) return;
        //�P�_�O�_�{�פ�  ��q�O�_����
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;
        StartCoroutine(nameof(DodgeCoroutine));
    }

    IEnumerator DodgeCoroutine()
    {
        isDodging = true;

        //���񭵮�
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);

        //��q���� (�z�o�ɤ�����)
        if(!isOverdriving)PlayerEnergy.Instance.Use(dodgeEnergyCost);

        //�{�׮ɪ��a�L��
        collider.isTrigger = true;

        //���m���ਤ
        currentRoll = 0f;

        //�w�t�ɶ�
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);

        /* �g�k1
        //�{�׮ɪu��X�b����
        var scale = transform.localScale;

        while(currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll,Vector3.right);

            if(currentRoll < maxRoll / 2f)
            {
                scale.x = Mathf.Clamp(scale.x - Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
                scale.y = Mathf.Clamp(scale.y - Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
                scale.z = Mathf.Clamp(scale.z - Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
            }
            else
            {
                scale.x = Mathf.Clamp(scale.x + Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
                scale.y = Mathf.Clamp(scale.y + Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
                scale.z = Mathf.Clamp(scale.z + Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
            }

            transform.localScale = scale;

            yield return null;
        }
        */

        /* �g�k2
        //�{�׮ɪu��X�b����
        var t1 = 0f;
        var t2 = 0f;
        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

            if (currentRoll < maxRoll / 2f)
            {
                t1 += Time.deltaTime / dodgeDuration;
                transform.localScale = Vector3.Lerp(transform.localScale,dodgeScale,t1);
            }
            else
            {
                t2 += Time.deltaTime / dodgeDuration;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t1);
            }

            transform.localScale = scale;

            yield return null;
        }
         */

        //�g�k3 �ϥΨ������u
        //�{�׮ɪu��X�b����
        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.up);
            transform.localScale = BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);

            yield return null;
        }

        collider.isTrigger = false;
        isDodging = false;
    }
    #endregion

    #region ��q�z�o
    void Overdrive()
    {
        if (GameManager.GameState == GameState.GameOver) return;
        if (overdriveLocked) return;
        //��q�����~�iĲ�o
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX))
        {
            AudioManager.Instance.PlaySFX(energyNoEnoughSFX);
            return;
        }

        PlayerOverDrive.on.Invoke();
    }

    void OverdriveOn()
    {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
        TimeController.Instance.BulletTime(slowMotionDuration, 0.2f, slowMotionDuration); //�l�u�ɶ�

        StartCoroutine(nameof(StartBattlecruiser));
        StartCoroutine(nameof(StartBattlecruiserAudio));
    }

    void OverdriveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;

        StartCoroutine(nameof(ExitBattlecruiser));
    }

    //����I�sĥ��
    public void OpenBattlecruisers(int level)
    {
        if(level == 0)
        {
            BattlecruisersUI.SetActive(false);
            overdriveLocked = true;
        }
        else if(level == 1)
        {
            BattlecruisersUI.SetActive(true);
            overdriveLocked = false;
            if (PlayerEnergy.Instance.IsEnough(100))
            {
                BattlecruisersShowUI.fillAmount = 0;
            }
            else
            {
                BattlecruisersShowUI.fillAmount = 1;
            }
        }
    }

    //�I�sĥ��
    IEnumerator StartBattlecruiser()
    {
        Battlecruisers.SetActive(true);
        Battlecruisers.GetComponent<Animator>().SetTrigger("Start");
        yield return new WaitForSeconds(1);

        AudioManager.Instance.PlaySFX(BattlecruiserSFX);

        while (isOverdriving)
        {
            //�n���ĤH�~�i�o�g
            if (EnemyManager.Instance.RandomEnemy != null)
            {
                PoolManager.Release(projectileOverdrive, muzzleBattlecruisers[Random.Range(0, muzzleBattlecruisers.Length)].position);
            }
            yield return waitForBattlecruiserInterval;
        }
    }

    IEnumerator StartBattlecruiserAudio()
    {
        yield return new WaitForSeconds(1);

        while (isOverdriving)
        {
            //�n���ĤH�~�i�o�g
            if (EnemyManager.Instance.RandomEnemy != null)
            {
                AudioManager.Instance.PlayRandomSFX(BattlecruiserLaunchSFX);
            }
            yield return waitForBattlecruiserAudioInterval;
        }
    }

    //�I�sĥ������
    IEnumerator ExitBattlecruiser()
    {
        Battlecruisers.GetComponent<Animator>().SetTrigger("End");
        yield return new WaitForSeconds(5);
        Battlecruisers.SetActive(false);
    }
    #endregion

    #region �ɼu
    //��ʵo�g�ɼu
    void LaunchMissile()
    {
        if (GameManager.GameState == GameState.GameOver) return;
        if (Missilelevel == 1)
        {
            missile.Launch(true,true,muzzleMissile[0]);
        }else if (Missilelevel == 2)
        {
            missile.Launch(true,true,muzzleMissile[0], muzzleMissile[1]);
        }else if(Missilelevel == 3)
        {
            StartCoroutine(nameof(LaunchMissileCoroutine));
        }
    }

    IEnumerator LaunchMissileCoroutine()
    {
        missile.Launch(false, true, muzzleMissile[0]);
        yield return new WaitForSeconds(0.1f);
        missile.Launch(false,false,muzzleMissile[1]);
        yield return new WaitForSeconds(0.1f);
        missile.Launch(false, false, muzzleMissile[2]);
        yield return new WaitForSeconds(0.1f);
        missile.Launch(true, false, muzzleMissile[3]);
    }

    //�Ұʾɼu�\��
    public void OpenMissileSystem(int level)
    {
        //IsMissileLocked = false;
        //StartCoroutine(FireMissile(Missilelevel));

        Missilelevel = level;

        MissileUI.SetActive(true);
        if (Missilelevel == 0)
        {
            MissileUI.SetActive(false);
        }
        else if (Missilelevel == 1)
        {
            MissileShip[0].SetActive(true);
        }
        else if(Missilelevel == 2)
        {
            MissileShip[1].SetActive(true);
        }
    }

    IEnumerator FireMissile(int FirePoint)
    {
        MissileShip[FirePoint].SetActive(true);

        while (!IsMissileLocked)
        {
            //�n���ĤH�~�i�o�g
            if(EnemyManager.Instance.RandomEnemy != null)
            {
                missile.Launch(true,true,muzzleMissile[FirePoint]);
            }
            yield return waitMissileTime;
        }

        yield break;
    }
    #endregion

    //�@�������^�_30HP
    public void AddHP()
    {
        if (!IsCheat) return;
        AudioManager.Instance.PlaySFX(CheatAudio);
        RestoreHealth(30);
    }

    //���a�L�ġA�b���@����_
    public void NoHurt()
    {
        if (!IsCheat) return;
        AudioManager.Instance.PlaySFX(CheatAudio);
        collider.isTrigger = !collider.isTrigger;
    }
}
