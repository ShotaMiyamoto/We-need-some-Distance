using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class GameDataStorageManager : SingletonMonoBehaviour<GameDataStorageManager>
{
    //＝＝＝＝＝＝＝＝＝＝＝＝＝生成確認＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    /// <summary>
    /// 生成されているかどうかをstaticな変数に格納する。
    /// </summary>
    private static bool created = false;

    //＝＝＝＝＝＝＝＝＝＝＝＝＝保存するデータ類＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private int lastStageNum = 20;
    private bool isAllStageCleared = false;

    public bool GetIsAllCleared { get { return isAllStageCleared; } }

    /// <summary>
    /// 一番最後にクリアしたレベル。クリア進行度。　KEY:LatestClearedlevelNum
    /// </summary>
    private int latestClearedLevelNum = default;
    
    public int GetLatestClearedlevelNum { get { return latestClearedLevelNum; } }


    /// <summary>
    /// スキップしたレベル KEY:SkippedLevelNum
    /// </summary>
    private List<int> skippedLevelList = new List<int>();

    public List<int> GetSkippedLevel { get { return skippedLevelList; } }

    private string skippedLevelString = default;

    //＝＝＝＝＝＝＝＝＝＝＝＝＝リトライカウント＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private int retryCount = 0;
    public int RetryCount 
    {
        get { return retryCount; }
        set { retryCount = value; } 
    }

    //＝＝＝＝＝＝＝＝＝＝＝＝＝10，20ステージの中間地点を通過したかどうか＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private int currentCheckPoint = 0;
    public int CurrentCheckPoint
    {
        set { currentCheckPoint = value; }
        get { return currentCheckPoint; }
    }

    protected override void Awake()
    {
        //===================フレームレート設定=======================
        Application.targetFrameRate = 60; // ターゲットフレームレートを60に設定

        //===================アナリティクス設定=======================

        //===================生成確認=======================
        base.Awake();

        if (!created)
        {
            DontDestroyOnLoad(this.gameObject); //シーンを切り替えても指定オブジェクトを残す
            created = true; //生成
            //Debug.Log("作られてないので新しく作りますね" + name);
        }
        else
        {
            Destroy(this.gameObject);
        }


        //===================データ読み込み=======================

        //クリアしたレベルの同期
        if (PlayerPrefs.HasKey("LatestClearedlevelNum"))
        {
            //すでにキーがある
            latestClearedLevelNum = PlayerPrefs.GetInt("LatestClearedlevelNum");
            if(latestClearedLevelNum == lastStageNum)
            {
                //Debug.Log("すべてのステージクリア済み");
                isAllStageCleared = true;
            }
            else
            {
                //Debug.Log("未クリアのステージがある");
                isAllStageCleared = false;
            }
        }
        else
        {
            //キーがない
            latestClearedLevelNum = 0;
            PlayerPrefs.SetInt("LatestClearedlevelNum", latestClearedLevelNum);
            //Debug.Log("KEY:LatestClearedlevelNum キーがないので作成");
        }


        //スキップしたレベルの同期
        if (PlayerPrefs.HasKey("SkippedLevelNum"))
        {
            //すでにキーがある
            skippedLevelString = PlayerPrefs.GetString("SkippedLevelNum");　//データの同期。
            //Debug.Log("KEY:SkippedLevelNum キーがある。　ステージ番号：" + skippedLevelString);

            //文字列を「,」で分割して文字列を数値化する
            foreach (var value in skippedLevelString.Split(','))
            {
                skippedLevelList.Add(int.Parse(value));
                skippedLevelList.Sort(); //昇順にソート
            }
        }
        else
        {
            //キーがない
            skippedLevelList.Add(0);
            skippedLevelString = "0";
            PlayerPrefs.SetString("SkippedLevelNum", skippedLevelString);
            //Debug.Log("スキップしたステージはないのでキー作成");
        }
    }


    public void ClearedLevel(int stageNum)
    {
        if(stageNum > latestClearedLevelNum)
        {
            latestClearedLevelNum = stageNum;
            PlayerPrefs.SetInt("LatestClearedlevelNum", latestClearedLevelNum);
            //Debug.Log("未クリアステージをクリアしたので保存");
        }
        else
        {
            //Debug.Log("すでにクリア済みステージのため保存はしない");
        }

        if(stageNum == lastStageNum)
        {
            isAllStageCleared = true;
        }
    }

    public void AddSkippedLevel(int num)
    {
        skippedLevelList.Add(num); //リストに番号を追加
        skippedLevelList.Sort(); //昇順にソート

        bool isFirst = true; //フラグ
        foreach (var value in skippedLevelList) //文字列に変換
        {
            if (isFirst)
            {
                skippedLevelString = value.ToString();
                isFirst = false;
                continue;
            }
            skippedLevelString += ',' + value.ToString();
        }

        PlayerPrefs.SetString("SkippedLevelNum", skippedLevelString); //文字列をセット

        //Debug.Log("スキップしたレベルを追加。データをセット：" + skippedLevelString);
    }

    public void RemoveSkippedLevel(int num)
    {
        skippedLevelList.Remove(num);

        bool isFirst = true; //フラグ
        foreach (var value in skippedLevelList) //文字列に変換
        {
            if (isFirst)
            {
                skippedLevelString = value.ToString();
                isFirst = false;
                continue;
            }
            skippedLevelString += ',' + value.ToString();
        }

        PlayerPrefs.SetString("SkippedLevelNum", skippedLevelString); //文字列をセット

        //Debug.Log("スキップしたレベルを削除。データをセット：" + skippedLevelString);

    }

    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
