using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : Singleton<GameManager>
{
    public static GameState GameState { get => Instance.gameState; set => Instance.gameState = value; }

    [SerializeField]GameState gameState = GameState.Playing;

    [Header("�O�_�ͦ��ĤH")]
    [SerializeField] bool spawnEnemy = true;
    [Header("�i��UI")]
    [SerializeField] GameObject waveUI;
    [Header("BOSS_UI")]
    [SerializeField] GameObject bossUI;
    [SerializeField] public GameObject bossHP_UI;
    [SerializeField] public GameObject bossName_UI;
    [Header("�r��UI")]
    [SerializeField] GameObject CaptionsUI;
    [SerializeField] Text Captions_Text;
    [SerializeField] AudioData TextAudio;
    [SerializeField] VideoPlayer videoplayer;
    [Header("�ثe�i��")]
    [SerializeField] public int WaveNumber = 1;
    [Header("�C�i���j�ɶ�")]
    [SerializeField] public float TimeBetweenWaves = 2f;
    [Header("�������d")]
    [SerializeField] StageData[] AllStage; //���q�Ҧ� 1~15   �a���Ҧ�16~30
    [Header("�}��")]
    [SerializeField] string[] TitleContent;
    [SerializeField] AudioData TitleAudio;
    [SerializeField] VideoClip TitleVideo;
    [Header("�ĤH�j��")]
    [SerializeField, Range(1f, 3f)] public float EnemyPower = 1f;
    [Header("��������")]
    [SerializeField] AudioData GameOverAudio;

    StageData currentState; //��e���d

    //����S�ĤH��
    WaitUntil waitUntilNoEnemy;
    //����r��������
    WaitUntil waitCaption;
    public bool IsCaptionEnd = true;
    //����ͦ�������
    WaitUntil waitSpawn;
    public bool IsSpawnEnd = false;

    WaitForSeconds waitTimeBetweenWaves;
    WaitForSeconds waitTimeEnd; //�C�i�����𮧮ɶ�
    WaitForSeconds waitBossTimeEnd; //Boss�����𮧮ɶ�

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

    //��r����
    IEnumerator ShowText(float ShowTime,string[] TextConent)
    {
        //�Y�����Ф�r�A�h�����ͦ��Ǫ�
        if(ShowTime == 0)
        {
            IsCaptionEnd = true;
            IsSpawnEnd = false;
            WaveCreatEnemy();
            yield break;
        }

        //�ɶ��Ȱ�
        TimeController.Instance.Pause();
        //�r���}��
        CaptionsUI.SetActive(true);
        Captions_Text.text = "";
        //�����Y��
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
            yield return new WaitForSecondsRealtime(ShowTime); //���ݮɶ��Ἵ��U�@�q��r
            Captions_Text.text = "";
        }

        //���ݮɶ���������r�}�l�C��
        CaptionsUI.SetActive(false);
        TimeController.Instance.Unpause();

        //�u���}���ݭn�i�J
        if (WaveNumber == 0)
        {
            WaveNumber++;
            StartCoroutine(nameof(StartCreatEnemy));
        }
        else
        {
            //�O�_��boss��
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
        //������}���~�|���� (�γQ�j���)
        while (spawnEnemy && WaveNumber <= AllStage.Length)
        {
            //�]�w��e���d���e
            currentState = AllStage[WaveNumber - 1];

            if (currentState.IsBoss)
            {
                bossUI.SetActive(true);
            }
            else
            {
                waveUI.SetActive(true);
            }

            yield return waitTimeBetweenWaves; //���ݫ�}�l�ͦ��ĤH

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

            //���Ф�r�Უ�ͼĤH
            StartCoroutine(ShowText(currentState.TextTime, currentState.ShowTexts));

            //�r��������
            yield return waitCaption;
            Debug.Log("�r������");
            ///�ͦ�������
            yield return waitSpawn;
            Debug.Log("�ͦ�����");
            //�S�ĤH��
            yield return waitUntilNoEnemy;
            Debug.Log("�S�ĤH�F");

            if (GameManager.GameState == GameState.GameOver) yield break;

            if (currentState.StageAnimator != null)
            {
                currentState.StageAnimator.GetComponent<Animator>().SetTrigger("End");
            }

            //�O�_��boss��
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

    //���͸Ӫi�ƼĤH
    void WaveCreatEnemy()
    {
        //�Y�ϥΰʧ@�e�{
        if (currentState.StageAnimator != null)
        {
            currentState.StageAnimator.SetActive(true);
            currentState.StageAnimator.GetComponent<Animator>().SetTrigger("Go");
            //�NAnimator�����ĤH�[�J�C��
            for (int i = 0; i < currentState.StageAnimator.transform.childCount; i++)
            {
                currentState.StageAnimator.transform.GetChild(i).gameObject.SetActive(true);
                //EnemyManager.Instance.enemyList.Add(currentState.StageAnimator.transform.GetChild(i).gameObject);
            }

            IsSpawnEnd = true;
        }
        //�ϥμĤH�ͦ���
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
            Debug.Log("���ͼĤH");
        }
    }

    public void End()
    {
        AudioManager.Instance.PlaySFX(GameOverAudio);
        GameState = GameState.GameOver;
        AudioManager.Instance.SetMusic(3);
        GameLog.Instance.GameOverUpdate();
        Debug.Log("�C������ !");
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

//���d���
[System.Serializable]
public class StageData
{
    public bool IsBoss; //�O�_��boss���d
    public float TextTime; //��r��ܮɶ�(0�h�����)
    public string[] ShowTexts; //��ܤ奻
    public VideoClip PhotoVideo; //��ܪ��j�Y
    public AudioData PlayAudio; //�O�_�����n��
    public GameObject StageAnimator; //�Ӫi�ƬO�_��Animator�e�{

    public bool IsRandom; //�O�_���H���ͦ�
    public int minEnemyAmount; //�Ӫi�Ǫ��ƶq
    public int maxEnemyAmount; //�Ӫi�Ǫ��ƶq

    public int[] ememyNum; //�Ӫi�ĤH�ӧO�ƶq
    public GameObject[] ememyPrefabs; //�Ӫi�ĤH�w�s��
    public GameObject SpecialEnemy; //�Ӫi�S��ĤH

    public float timeBetweenSpawns; //�ͦ����j�ɶ�
}

public enum GameState
{
    Playing,
    Paused,
    GameOver,
}