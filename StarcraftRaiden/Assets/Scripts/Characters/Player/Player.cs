using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [Header("生命條")]
    [SerializeField] StateBar_HUD statsBar_HUD;
    [Header("是否血量再生")]
    [SerializeField]  bool regenerateHealth = true;
    [Header("血量再生間隔")]
    [SerializeField] float healthRegenerateTime;
    [Header("血量再生百分比")]
    [SerializeField,Range(0f,1f)] float healthRegeneratePercent;

    [Header("輸入設定")]
    [SerializeField]PlayerInput input;

    [Header("移動速度")]
    [SerializeField]public float moveSpeed = 10f;
    [Header("加減速度")]
    [SerializeField] float acclerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;
    [Header("移動旋轉角度")]
    [SerializeField] float moveRotationAngle = 50f;

    [Header("子彈預製物")]
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;
    [SerializeField] GameObject projectile4;
    [SerializeField] GameObject projectile5;
    [SerializeField] GameObject projectileOverdrive;
    [Header("子彈發射位置")]
    [SerializeField] Transform muzzleMiddle;
    [SerializeField] Transform muzzleLeft;
    [SerializeField] Transform muzzleRight;
    [SerializeField] Transform muzzleLeft2;
    [SerializeField] Transform muzzleRight2;
    [Header("發射音效")]
    [SerializeField] AudioData projectileLaunchSFX;
    [Header("武器威力(等級)")]
    [SerializeField, Range(0,4)] public int weaponPower = 0;

    [Header("射擊速度")]
    public float FireInterval = 0.2f;


    [Header("閃避音效")]
    [SerializeField] AudioData dodgeSFX;
    [Header("閃避能量消耗值")]
    [SerializeField,Range(0,100)]public int dodgeEnergyCost = 25;
    [Header("閃避圈數  閃避速度")]
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f,0.5f,0.5f);

    [Header("能量爆發")]
    [SerializeField]public bool overdriveLocked = true;
    [SerializeField]int overdriveDodgeFactor = 2;
    [SerializeField]float overdriveSpeedFactor = 1.2f;
    [SerializeField]float overdriveFireFactor = 1.2f;
    [SerializeField] AudioData energyNoEnoughSFX;

    [Header("呼叫艦隊")]
    [SerializeField] GameObject BattlecruisersUI;
    [SerializeField] public Image BattlecruisersShowUI;
    [SerializeField] GameObject Battlecruisers;
    [SerializeField] Transform[] muzzleBattlecruisers;
    [SerializeField] AudioData BattlecruiserSFX;
    [SerializeField] AudioData[] BattlecruiserLaunchSFX;

    [Header("導彈功能")]
    [SerializeField] GameObject MissileUI;
    [SerializeField] float MissileTime;
    [SerializeField] GameObject[] MissileShip;
    [SerializeField] Transform[] muzzleMissile;
    [SerializeField] int Missilelevel;
    [SerializeField] bool IsMissileLocked = true;

    [Header("升級外觀")]
    [SerializeField] public GameObject Level2_Skin;

    [Header("作弊功能")]
    [SerializeField] bool IsCheat = true;
    [SerializeField] AudioData CheatAudio;

    bool isDodging = false; //正在閃避中

    bool isOverdriving = false; //正在能量爆發

    readonly float slowMotionDuration = 0.5f; //子彈緩速時間

    //飛機邊界範圍
    float paddingX;
    float paddingY;

    //當前旋轉角
    float currentRoll;

    //翻滾持續時間
    float dodgeDuration;

    float t;
    Vector2 previousVelocity;
    Quaternion previousRotation;

    //協成讀秒
    WaitForSeconds waitForFireInterval; //普通射擊時間
    WaitForSeconds waitForOverdriveFireInterval; //爆發射擊時間
    WaitForSeconds waitForBattlecruiserInterval; //艦隊射擊計時
    WaitForSeconds waitForBattlecruiserAudioInterval; //艦隊射擊計時聲音
    WaitForSeconds waitHealthRegenerateTime; //回血時間
    WaitForSeconds waitDecelerationTime; //等待減速時間
    WaitForSeconds waitMissileTime; //導彈發射時間

    //協成
    Coroutine moveCoroutine;
    Coroutine healthRegenerateCoroutine;

    //碰撞用
    new Rigidbody2D rigidbody;
    new Collider2D collider;

    //導彈系統
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

        waitForFireInterval = new WaitForSeconds(FireInterval); //普通射擊計時
        waitForOverdriveFireInterval = new WaitForSeconds(FireInterval / overdriveFireFactor); //能量爆發射擊計時
        waitForBattlecruiserInterval = new WaitForSeconds(0.025f); //艦隊射擊計時
        waitForBattlecruiserAudioInterval = new WaitForSeconds(0.2f); //艦隊射擊聲音計時
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime); //設定回血計時
        waitDecelerationTime = new WaitForSeconds(decelerationTime); //設定減速計時
        waitMissileTime = new WaitForSeconds(MissileTime);
    }
    protected override void OnEnable()
    {
        base.OnEnable(); //沿用基類的功能

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
        statsBar_HUD.Initialized(health, maxHealth); //初始化血條

        input.EnableGameplayInput();
    }

    //刷新射擊設定
    public void UpdateFireSpeed()
    {
        waitForFireInterval = new WaitForSeconds(FireInterval); //設定開火計時
        waitForOverdriveFireInterval = new WaitForSeconds(FireInterval / overdriveFireFactor); //設定開火計時
        waitMissileTime = new WaitForSeconds(MissileTime);
    }

    //獲得道具
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

    //受傷
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);  //沿用基類的功能
        statsBar_HUD.UpdateStas(health, maxHealth); //更新血條
        //緩速時間
        //TimeController.Instance.BulletTime(slowMotionDuration);
        //鏡頭震動
        CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);

        //受傷後開始執行被動回血
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

    //設定自動回復數值
    public void SetHealthRegenerate(float RTime,float RPercent)
    {
        healthRegenerateTime = RTime;
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime); //設定回血計時
        healthRegeneratePercent = RPercent;

        if (gameObject.activeSelf)
        {
            if (regenerateHealth)
            {
                if (healthRegenerateCoroutine != null) // 有出現表示正在執行，則刷新即可
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

    //死亡
    public override void Die()
    {
        GameManager.Instance.End();
        statsBar_HUD.UpdateStas(0f, maxHealth);
        base.Die();
    }

    #region 移動
    void Move(Vector2 moveInput)
    {
        if (GameManager.GameState == GameState.GameOver) return;
        //停止上一個呼叫
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        //輸入信號 * 移動速度 = 加速度方向
        Quaternion moveRotation = Quaternion.AngleAxis(moveRotationAngle * moveInput.x, Vector2.down);
        moveCoroutine = StartCoroutine(MoveCoroutine(acclerationTime,moveInput.normalized * moveSpeed, moveRotation));
        //只有在移動時才去呼叫判斷範圍
        StopCoroutine(nameof(DecelerationCoroutine));
        StartCoroutine(nameof(MoveRangeLimatationCoroutine));
    }

    void StopMove()
    {
        //停止上一個呼叫
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        //輸入信號 * 移動速度 = 加速度方向
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime,Vector2.zero,Quaternion.identity));
        //停止時關閉呼叫判斷範圍
        StartCoroutine(nameof(DecelerationCoroutine));
    }

    //移動協成
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

    //移動限制協成
    IEnumerator MoveRangeLimatationCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position,paddingX,paddingY);

            yield return null;
        }
    }

    //減速協成
    IEnumerator DecelerationCoroutine()
    {
        yield return waitDecelerationTime;

        //停止減速時關閉呼叫判斷範圍
        StopCoroutine(nameof(MoveRangeLimatationCoroutine));
    }
    #endregion

    #region 射擊
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
        //1號子彈平行飛  2 偏上  3偏下
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

            //是否能量爆發狀態，決定間隔時間
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

    #region 閃避
    void Dodge()
    {
        if (GameManager.GameState == GameState.GameOver) return;
        //判斷是否閃避中  能量是否足夠
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;
        StartCoroutine(nameof(DodgeCoroutine));
    }

    IEnumerator DodgeCoroutine()
    {
        isDodging = true;

        //播放音效
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);

        //能量消耗 (爆發時不消耗)
        if(!isOverdriving)PlayerEnergy.Instance.Use(dodgeEnergyCost);

        //閃避時玩家無敵
        collider.isTrigger = true;

        //重置旋轉角
        currentRoll = 0f;

        //緩速時間
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);

        /* 寫法1
        //閃避時沿著X軸旋轉
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

        /* 寫法2
        //閃避時沿著X軸旋轉
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

        //寫法3 使用貝茲曲線
        //閃避時沿著X軸旋轉
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

    #region 能量爆發
    void Overdrive()
    {
        if (GameManager.GameState == GameState.GameOver) return;
        if (overdriveLocked) return;
        //能量足夠才可觸發
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
        TimeController.Instance.BulletTime(slowMotionDuration, 0.2f, slowMotionDuration); //子彈時間

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

    //解鎖呼叫艦隊
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

    //呼叫艦隊
    IEnumerator StartBattlecruiser()
    {
        Battlecruisers.SetActive(true);
        Battlecruisers.GetComponent<Animator>().SetTrigger("Start");
        yield return new WaitForSeconds(1);

        AudioManager.Instance.PlaySFX(BattlecruiserSFX);

        while (isOverdriving)
        {
            //要有敵人才可發射
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
            //要有敵人才可發射
            if (EnemyManager.Instance.RandomEnemy != null)
            {
                AudioManager.Instance.PlayRandomSFX(BattlecruiserLaunchSFX);
            }
            yield return waitForBattlecruiserAudioInterval;
        }
    }

    //呼叫艦隊結束
    IEnumerator ExitBattlecruiser()
    {
        Battlecruisers.GetComponent<Animator>().SetTrigger("End");
        yield return new WaitForSeconds(5);
        Battlecruisers.SetActive(false);
    }
    #endregion

    #region 導彈
    //手動發射導彈
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

    //啟動導彈功能
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
            //要有敵人才可發射
            if(EnemyManager.Instance.RandomEnemy != null)
            {
                missile.Launch(true,true,muzzleMissile[FirePoint]);
            }
            yield return waitMissileTime;
        }

        yield break;
    }
    #endregion

    //作弊直接回復30HP
    public void AddHP()
    {
        if (!IsCheat) return;
        AudioManager.Instance.PlaySFX(CheatAudio);
        RestoreHealth(30);
    }

    //玩家無敵，在按一次恢復
    public void NoHurt()
    {
        if (!IsCheat) return;
        AudioManager.Instance.PlaySFX(CheatAudio);
        collider.isTrigger = !collider.isTrigger;
    }
}
