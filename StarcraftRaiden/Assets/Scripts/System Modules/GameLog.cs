using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml; //xml�ݨϥΨ�
using System.IO; //File�ݨϥΨ�

public class GameLog : Singleton<GameLog>
{
    [Header("�C���ɶ�")]
    [SerializeField] int TotalTime;
    [Header("���ļ�")]
    [SerializeField] public int KillNum;
    [Header("����UI")]
    [SerializeField] GameObject GameOverUI;
    [SerializeField] Text ScoreText;
    [SerializeField] Text LevelText;
    [SerializeField] Image LevelImage;
    [SerializeField] Text KillText;
    [SerializeField] Text TimeText;
    [SerializeField] Canvas GameUI;
    [Header("�Ʀ�]UI")]
    [SerializeField] Canvas RankingUI;
    [SerializeField] GameObject RankingItemsUI;
    [SerializeField] Text PlayerScoreText;
    [Header("�s����UI")]
    [SerializeField] Canvas HighScoreUI;
    [SerializeField] InputField HighScoreNameInput;
    [SerializeField] AudioData NewScoreAudio;
    int NewRank = 0;

    //�Ʀ�ƾ�
    [SerializeField] string filepath; //Ū��xml���|
    private XmlDocument xmlDoc = new XmlDocument();
    [SerializeField] RankData[] RankList;

    WaitForSeconds waitForTimeInterval;

    protected override void Awake()
    {
        base.Awake();
        waitForTimeInterval = new WaitForSeconds(1);
    }

    //�p��C���ɶ�
    IEnumerator Start()
    {
        LoadRanking();

        while (GameManager.GameState != GameState.GameOver)
        {
            TotalTime++;
            yield return waitForTimeInterval;
        }
    }

    //�����s
    public void GameOverUpdate()
    {
        GameOverUI.SetActive(true);
        GameUI.enabled = false;
        ScoreText.text = ScoreManager.Instance.score.ToString();
        PlayerScoreText.text = ScoreManager.Instance.score.ToString();
        LevelText.text = PlayerLevel.Instance.level.ToString();
        LevelImage.sprite = PlayerLevel.Instance.LevelUp_NewImage.sprite;
        KillText.text = KillNum.ToString();
        TimeText.text = (TotalTime / 60).ToString() + "��" + (TotalTime % 60).ToString() + "��";
    }

    //���J����
    public void LoadRanking()
    {
        filepath = System.Environment.CurrentDirectory + "/RankLog.xml";
        xmlDoc.Load(filepath);
        XmlElement RankingInfoDoc = xmlDoc.DocumentElement;
        XmlNodeList XmlList = RankingInfoDoc.SelectNodes("Item"); //���o�Ҧ����
        for (int i = 0; i < XmlList.Count; i++)
        {
            RankList[i].Ranking = int.Parse(XmlList[i].Attributes["Num"].Value);
            RankList[i].Score = int.Parse(XmlList[i].SelectSingleNode("Score").InnerText);
            RankList[i].Name = XmlList[i].SelectSingleNode("Name").InnerText;
            RankList[i].Level = int.Parse(XmlList[i].SelectSingleNode("Level").InnerText);
            RankList[i].Kill = int.Parse(XmlList[i].SelectSingleNode("Kill").InnerText);
        }

        if(RankingItemsUI != null)
        {
            //��s�Ʀ�UI
            for (int i = 0; i < RankList.Length; i++)
            {
                RankingItemsUI.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = RankList[i].Ranking.ToString();
                RankingItemsUI.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = RankList[i].Score.ToString();
                RankingItemsUI.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = RankList[i].Name;
                RankingItemsUI.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    //�O�s����
    public void SaveRanking()
    {
        xmlDoc = new XmlDocument(); //�إߤ@��XML
        XmlElement RankingInfoDoc = xmlDoc.CreateElement("RankingInfo");
        RankingInfoDoc.SetAttribute("Version", "0"); //�]�w������
        xmlDoc.AppendChild(RankingInfoDoc);

        XmlElement RankDoc = null; //��e
        XmlElement RankChildElem = null; //��e�l��

        //�����ƦW
        for (int i = 0; i < RankList.Length; i++)
        {
            RankDoc = xmlDoc.CreateElement("Item");
            RankDoc.SetAttribute("Num", RankList[i].Ranking.ToString()); //�]�w�ƦW
            RankingInfoDoc.AppendChild(RankDoc);

            RankChildElem = xmlDoc.CreateElement("Score");
            RankChildElem.InnerText = RankList[i].Score.ToString(); //�]�w����
            RankDoc.AppendChild(RankChildElem);

            RankChildElem = xmlDoc.CreateElement("Name");
            RankChildElem.InnerText = RankList[i].Name; //�]�w�W�r
            RankDoc.AppendChild(RankChildElem);

            RankChildElem = xmlDoc.CreateElement("Level");
            RankChildElem.InnerText = RankList[i].Level.ToString(); //�]�w����
            RankDoc.AppendChild(RankChildElem);

            RankChildElem = xmlDoc.CreateElement("Kill");
            RankChildElem.InnerText = RankList[i].Kill.ToString(); //�]�w����
            RankDoc.AppendChild(RankChildElem);
        }

        filepath = System.Environment.CurrentDirectory + "/RankLog.xml";
        xmlDoc.Save(filepath);
    }

    //�Ʀ歶���}�� (�P�_�O�_�s����)
    public void CheckHighScore()
    {
        GameOverUI.SetActive(false);
        RankingUI.enabled = true;

        //�����Ҧ��ĤH(�קK�S�ĵL�k����)
        EnemyManager.Instance.RemoveAllFromList();

        HighScoreTest();

        if (NewRank != 99)
        {
            AudioManager.Instance.PlaySFX(NewScoreAudio);
            HighScoreUI.enabled = true;
        }
        else
        {
            UpdateRanking();
        }
    }

    //������J�����ɭ�
    public void CancelHighScore()
    {
        NewRank = 99;

        //������JUI
        HighScoreUI.enabled = false;

        UpdateRanking();
    }

    public void UpdateRanking()
    {
        //��s�Ʀ�
        if (NewRank != 99)
        {
            //���s�ƦC
            for (int i = RankList.Length-1; i > NewRank; i--)
            {
                RankList[i].Ranking = RankList[i - 1].Ranking;
                RankList[i].Score = RankList[i - 1].Score;
                RankList[i].Name = RankList[i - 1].Name;
                RankList[i].Level = RankList[i - 1].Level;
                RankList[i].Kill = RankList[i - 1].Kill;
            }

            //�s����
            RankList[NewRank].Ranking = NewRank + 1;
            RankList[NewRank].Score = ScoreManager.Instance.score;

            if (HighScoreNameInput.text.Length == 0)
            {
                RankList[NewRank].Name = "No Name";
            }
            else
            {
                RankList[NewRank].Name = HighScoreNameInput.text;
            }
            RankList[NewRank].Level = PlayerLevel.Instance.level;
            RankList[NewRank].Kill = KillNum;

            //������JUI
            HighScoreUI.enabled = false;
        }

        Debug.Log(NewRank);

        //��s�Ʀ�UI
        for(int i = 0; i < RankList.Length; i++)
        {
            RankingItemsUI.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = RankList[i].Ranking.ToString();
            RankingItemsUI.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = RankList[i].Score.ToString();
            RankingItemsUI.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = RankList[i].Name;

            if(i == NewRank)
            {
                RankingItemsUI.transform.GetChild(i).GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                RankingItemsUI.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
            }
        }

        SaveRanking();
    }

    //�P�_�O�_�W�]
    public void HighScoreTest()
    {
        NewRank= 99; //���o���W��
        for (int i = RankList.Length-1 ; i > 0; i--)
        {
            if (ScoreManager.Instance.score > RankList[i].Score)
            {
                NewRank = i;
            }
        }
    }

    //���d���
    [System.Serializable]
    public class RankData
    {
        public int Ranking; //�W��
        public int Score; //����
        public string Name; //�W�r
        public int Level; //����
        public int Kill; //������
    }
}
