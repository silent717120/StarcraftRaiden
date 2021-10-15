using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml; //xml需使用到
using System.IO; //File需使用到

public class GameLog : Singleton<GameLog>
{
    [Header("遊戲時間")]
    [SerializeField] int TotalTime;
    [Header("殺敵數")]
    [SerializeField] public int KillNum;
    [Header("結束UI")]
    [SerializeField] GameObject GameOverUI;
    [SerializeField] Text ScoreText;
    [SerializeField] Text LevelText;
    [SerializeField] Image LevelImage;
    [SerializeField] Text KillText;
    [SerializeField] Text TimeText;
    [SerializeField] Canvas GameUI;
    [Header("排行榜UI")]
    [SerializeField] Canvas RankingUI;
    [SerializeField] GameObject RankingItemsUI;
    [SerializeField] Text PlayerScoreText;
    [Header("新高分UI")]
    [SerializeField] Canvas HighScoreUI;
    [SerializeField] InputField HighScoreNameInput;
    [SerializeField] AudioData NewScoreAudio;
    int NewRank = 0;

    //排行數據
    [SerializeField] string filepath; //讀取xml路徑
    private XmlDocument xmlDoc = new XmlDocument();
    [SerializeField] RankData[] RankList;

    WaitForSeconds waitForTimeInterval;

    protected override void Awake()
    {
        base.Awake();
        waitForTimeInterval = new WaitForSeconds(1);
    }

    //計算遊戲時間
    IEnumerator Start()
    {
        LoadRanking();

        while (GameManager.GameState != GameState.GameOver)
        {
            TotalTime++;
            yield return waitForTimeInterval;
        }
    }

    //結算刷新
    public void GameOverUpdate()
    {
        GameOverUI.SetActive(true);
        GameUI.enabled = false;
        ScoreText.text = ScoreManager.Instance.score.ToString();
        PlayerScoreText.text = ScoreManager.Instance.score.ToString();
        LevelText.text = PlayerLevel.Instance.level.ToString();
        LevelImage.sprite = PlayerLevel.Instance.LevelUp_NewImage.sprite;
        KillText.text = KillNum.ToString();
        TimeText.text = (TotalTime / 60).ToString() + "分" + (TotalTime % 60).ToString() + "秒";
    }

    //載入紀錄
    public void LoadRanking()
    {
        filepath = System.Environment.CurrentDirectory + "/RankLog.xml";
        xmlDoc.Load(filepath);
        XmlElement RankingInfoDoc = xmlDoc.DocumentElement;
        XmlNodeList XmlList = RankingInfoDoc.SelectNodes("Item"); //取得所有欄位
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
            //更新排行UI
            for (int i = 0; i < RankList.Length; i++)
            {
                RankingItemsUI.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = RankList[i].Ranking.ToString();
                RankingItemsUI.transform.GetChild(i).GetChild(1).GetComponent<Text>().text = RankList[i].Score.ToString();
                RankingItemsUI.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = RankList[i].Name;
                RankingItemsUI.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    //保存紀錄
    public void SaveRanking()
    {
        xmlDoc = new XmlDocument(); //建立一個XML
        XmlElement RankingInfoDoc = xmlDoc.CreateElement("RankingInfo");
        RankingInfoDoc.SetAttribute("Version", "0"); //設定版本號
        xmlDoc.AppendChild(RankingInfoDoc);

        XmlElement RankDoc = null; //當前
        XmlElement RankChildElem = null; //當前子項

        //紀錄排名
        for (int i = 0; i < RankList.Length; i++)
        {
            RankDoc = xmlDoc.CreateElement("Item");
            RankDoc.SetAttribute("Num", RankList[i].Ranking.ToString()); //設定排名
            RankingInfoDoc.AppendChild(RankDoc);

            RankChildElem = xmlDoc.CreateElement("Score");
            RankChildElem.InnerText = RankList[i].Score.ToString(); //設定分數
            RankDoc.AppendChild(RankChildElem);

            RankChildElem = xmlDoc.CreateElement("Name");
            RankChildElem.InnerText = RankList[i].Name; //設定名字
            RankDoc.AppendChild(RankChildElem);

            RankChildElem = xmlDoc.CreateElement("Level");
            RankChildElem.InnerText = RankList[i].Level.ToString(); //設定等級
            RankDoc.AppendChild(RankChildElem);

            RankChildElem = xmlDoc.CreateElement("Kill");
            RankChildElem.InnerText = RankList[i].Kill.ToString(); //設定擊殺
            RankDoc.AppendChild(RankChildElem);
        }

        filepath = System.Environment.CurrentDirectory + "/RankLog.xml";
        xmlDoc.Save(filepath);
    }

    //排行頁面開啟 (判斷是否新高分)
    public void CheckHighScore()
    {
        GameOverUI.SetActive(false);
        RankingUI.enabled = true;

        //移除所有敵人(避免特效無法消失)
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

    //取消輸入高分界面
    public void CancelHighScore()
    {
        NewRank = 99;

        //關閉輸入UI
        HighScoreUI.enabled = false;

        UpdateRanking();
    }

    public void UpdateRanking()
    {
        //更新排行
        if (NewRank != 99)
        {
            //重新排列
            for (int i = RankList.Length-1; i > NewRank; i--)
            {
                RankList[i].Ranking = RankList[i - 1].Ranking;
                RankList[i].Score = RankList[i - 1].Score;
                RankList[i].Name = RankList[i - 1].Name;
                RankList[i].Level = RankList[i - 1].Level;
                RankList[i].Kill = RankList[i - 1].Kill;
            }

            //新紀錄
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

            //關閉輸入UI
            HighScoreUI.enabled = false;
        }

        Debug.Log(NewRank);

        //更新排行UI
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

    //判斷是否上榜
    public void HighScoreTest()
    {
        NewRank= 99; //取得的名次
        for (int i = RankList.Length-1 ; i > 0; i--)
        {
            if (ScoreManager.Instance.score > RankList[i].Score)
            {
                NewRank = i;
            }
        }
    }

    //關卡資料
    [System.Serializable]
    public class RankData
    {
        public int Ranking; //名次
        public int Score; //分數
        public string Name; //名字
        public int Level; //等級
        public int Kill; //擊殺數
    }
}
