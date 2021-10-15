using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : Singleton<GameManager>
{
    public static GameState GameState { get => Instance.gameState; set => Instance.gameState = value; }

    [SerializeField]GameState gameState = GameState.Playing;

    [Header("是否生成敵人")]
    [SerializeField] bool spawnEnemy = true;
    [Header("波數UI")]
    [SerializeField] GameObject waveUI;
    [Header("BOSS_UI")]
    [SerializeField] GameObject bossUI;
    [SerializeField] public GameObject bossHP_UI;
    [SerializeField] public GameObject bossName_UI;
    [Header("字幕UI")]
    [SerializeField] GameObject CaptionsUI;
    [SerializeField] Text Captions_Text;
    [SerializeField] AudioData TextAudio;
    [SerializeField] VideoPlayer videoplayer;
    [Header("目前波數")]
    [SerializeField] public int WaveNumber = 1;
    [Header("每波間隔時間")]
    [SerializeField] public float TimeBetweenWaves = 2f;
    [Header("全部關卡")]
    [SerializeField] StageData[] AllStage; //普通模式 1~15   地獄模式16~30
    [Header("開場")]
    [SerializeField] string[] TitleContent;
    [SerializeField] AudioData TitleAudio;
    [SerializeField] VideoClip TitleVideo;
    [Header("敵人強度")]
    [SerializeField, Range(1f, 3f)] public float EnemyPower = 1f;
    [Header("結束音效")]
    [SerializeField] AudioData GameOverAudio;

    StageData currentState; //當前關卡

    //等到沒敵人時
    WaitUntil waitUntilNoEnemy;
    //等到字幕結束時
    WaitUntil waitCaption;
    public bool IsCaptionEnd = true;
    //等到生成結束時
    WaitUntil waitSpawn;
    public bool IsSpawnEnd = false;

    WaitForSeconds waitTimeBetweenWaves;
    WaitForSeconds waitTimeEnd; //每波結束休息時間
    WaitForSeconds waitBossTimeEnd; //Boss結束休息時間

    protected override void Awake()
    {
        base.Awake();
        waitTimeBetweenWaves = new WaitForSeconds(TimeBetweenWaves);
        waitTimeEnd = new WaitForSeconds(2f);
        waitBossTimeEnd = new WaitForSeconds(3f);
        waitUntilNoEnemy = new WaitUntil(() => EnemyManager.Instance.enemyList.Count == 0);
        waitCaption = new WaitUntil(() => IsCaptionEnd);
        waitSpawn = new WaitUntil(() => IsSpawnEnd);
    }

    private void Start()
    {
        if (!spawnEnemy) return;
        IsCaptionEnd = false;
        if (WaveNumber == 0) {
            AudioManager.Instance.PlaySFX(TitleAudio);
            StartCoroutine(nameof(GameStart));
        }
        else
        {
            StartCoroutine(nameof(StartCreatEnemy));
        }
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(ShowText(2f, TitleContent));
    }

    //文字介紹
    IEnumerator ShowText(float ShowTime,string[] TextConent)
    {
        //若不介紹文字，則直接生成怪物
        if(ShowTime == 0)
        {
            IsCaptionEnd = true;
            IsSpawnEnd = false;
            WaveCreatEnemy();
            yield break;
        }

        //時間暫停
        TimeController.Instance.Pause();
        //字幕開啟
        CaptionsUI.SetActive(true);
        Captions_Text.text = "";
        //播放頭像
        videoplayer.targetTexture.Release();
        if (WaveNumber == 0)
        {
            videoplayer.clip = TitleVideo;
        }else
        {
            videoplayer.clip = currentState.PhotoVideo;
        }

        for (int i = 0; i < TextConent.Length; i++)
        {
            for(int j = 0; j < TextConent[i].Length; j++)
            {
                Captions_Text.text += TextConent[i][j];
                AudioManager.Instance.PlayRandomSFX(TextAudio);
                yield return new WaitForSecondsRealtime(0.1f);
            }
            yield return new WaitForSecondsRealtime(ShowTime); //等待時間後播放下一段文字
            Captions_Text.text = "";
        }

        //等待時間後關閉文字開始遊戲
        CaptionsUI.SetActive(false);
        TimeController.Instance.Unpause();

        //只有開場需要進入
        if (WaveNumber == 0)
        {
            WaveNumber++;
            StartCoroutine(nameof(StartCreatEnemy));
        }
        else
        {
            //是否為boss戰
            if (currentState.IsBoss)
            {
                AudioManager.Instance.SetMusic(2);
            }
            WaveCreatEnemy();

            IsCaptionEnd = true;
            IsSpawnEnd = false;
        }
    }

    IEnumerator StartCreatEnemy()
    {
        //直到全破關才會結束 (或被強制中止)
        while (spawnEnemy && WaveNumber <= AllStage.Length)
        {
            //設定當前關卡內容
            currentState = AllStage[WaveNumber - 1];

            if (currentState.IsBoss)
            {
                bossUI.SetActive(true);
            }
            else
            {
                waveUI.SetActive(true);
            }

            yield return waitTimeBetweenWaves; //等待後開始生成敵人

            if (currentState.IsBoss)
            {
                bossUI.SetActive(false);
            }
            else
            {
                waveUI.SetActive(false);
            }

            if(currentState.PlayAudio != null)
            {
                AudioManager.Instance.PlaySFX(currentState.PlayAudio);
            }

            //介紹文字後產生敵人
            StartCoroutine(ShowText(currentState.TextTime, currentState.ShowTexts));

            //字幕結束時
            yield return waitCaption;
            Debug.Log("字幕結束");
            ///生成結束時
            yield return waitSpawn;
            Debug.Log("生成結束");
            //沒敵人時
            yield return waitUntilNoEnemy;
            Debug.Log("沒敵人了");

            if (GameManager.GameState == GameState.GameOver) yield break;

            if (currentState.StageAnimator != null)
            {
                currentState.StageAnimator.GetComponent<Animator>().SetTrigger("End");
            }

            //是否為boss戰
            if (currentState.IsBoss)
            {
                AudioManager.Instance.SetMusic(1);

                yield return waitBossTimeEnd;
            }

            WaveNumber++;
            IsCaptionEnd = false;

            yield return waitTimeEnd;
        }
        
        End();
    }

    //產生該波數敵人
    void WaveCreatEnemy()
    {
        //若使用動作呈現
        if (currentState.StageAnimator != null)
        {
            currentState.StageAnimator.SetActive(true);
            currentState.StageAnimator.GetComponent<Animator>().SetTrigger("Go");
            //將Animator中的敵人加入列表
            for (int i = 0; i < currentState.StageAnimator.transform.childCount; i++)
            {
                currentState.StageAnimator.transform.GetChild(i).gameObject.SetActive(true);
                //EnemyManager.Instance.enemyList.Add(currentState.StageAnimator.transform.GetChild(i).gameObject);
            }

            IsSpawnEnd = true;
        }
        //使用敵人生成器
        else
        {
            EnemyManager.Instance.minEnemyAmount = currentState.minEnemyAmount;
            EnemyManager.Instance.maxEnemyAmount = currentState.maxEnemyAmount;
            EnemyManager.Instance.SetBetweenTime(currentState.timeBetweenSpawns);
            EnemyManager.Instance.ememyNum = currentState.ememyNum;
            EnemyManager.Instance.ememyPrefabs = currentState.ememyPrefabs;
            if (currentState.IsRandom)
            {
                EnemyManager.Instance.StartRandomCreatEnemy();
            }
            else
            {
                EnemyManager.Instance.StartCreatEnemy();
            }
            if(currentState.SpecialEnemy != null)
            {
                EnemyManager.Instance.CreateSpecialEnemy(currentState.SpecialEnemy);
            }
            Debug.Log("產生敵人");
        }
    }

    public void End()
    {
        AudioManager.Instance.PlaySFX(GameOverAudio);
        GameState = GameState.GameOver;
        AudioManager.Instance.SetMusic(3);
        GameLog.Instance.GameOverUpdate();
        Debug.Log("遊戲結束 !");
    }

    public void Restart()
    {
        SceneLoader.Instance.LoadGameplayScene();
    }

    public void BackMainMenu()
    {
        SceneLoader.Instance.LoadMainMenuSence();
    }
}

//關卡資料
[System.Serializable]
public class StageData
{
    public bool IsBoss; //是否為boss關卡
    public float TextTime; //文字顯示時間(0則不顯示)
    public string[] ShowTexts; //顯示文本
    public VideoClip PhotoVideo; //顯示的大頭
    public AudioData PlayAudio; //是否播放聲音
    public GameObject StageAnimator; //該波數是否用Animator呈現

    public bool IsRandom; //是否為隨機生成
    public int minEnemyAmount; //該波怪物數量
    public int maxEnemyAmount; //該波怪物數量

    public int[] ememyNum; //該波敵人個別數量
    public GameObject[] ememyPrefabs; //該波敵人預製體
    public GameObject SpecialEnemy; //該波特殊敵人

    public float timeBetweenSpawns; //生成間隔時間
}

public enum GameState
{
    Playing,
    Paused,
    GameOver,
}