using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
using System;
using UnityEngine.Analytics;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    //＝＝＝＝＝＝＝＝＝＝＝＝＝セーブデータ関連＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] private int stageNum = default;
    private GameDataStorageManager gdsm = default;
    private bool isArrivedGoal = false; //スキップせずゴールできたか
    public bool SetIsArrivedGoal { set { isArrivedGoal = value; } } //セッター

    //＝＝＝＝＝＝＝＝＝＝＝＝＝進行状況関連＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private bool isCleared = false;
    public bool IsCreared 
    {
        get { return isCleared; }
        set { isCleared = value; }
    }

    private bool isPlayerDied = false;
    public bool IsPlayerDied 
    {
        get { return isPlayerDied; }
        set { isPlayerDied = value; }
    }

    //＝＝＝＝＝＝＝＝＝＝＝＝＝Fading関連＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private GameObject fadePanel = default;

    private float timeElapsed = 0f;

    [SerializeField] private float fadeOutDelayTime = 8f;


    private bool isFading = false;

    [SerializeField] private float fadeOutTime = 3f;

    [SerializeField] private float fadeInTime = 1.5f;

    //＝＝＝＝＝＝＝＝＝＝＝＝＝MenuPanel処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private Animator menuPanelAnim = default;
    private bool isMenuPanelActivating = false;

    [SerializeField] private Animator stagePanelAnim = default;

    [SerializeField] private GameObject stopButton = default;
    [SerializeField] private GameObject stopButtonShadow = default;
    [SerializeField] private Sprite stopImage = default;
    [SerializeField] private Sprite playImage = default;
    [SerializeField] private GameObject skipRecomButton = default;
    [SerializeField] private Text stageNumText = default;
    [SerializeField] private string stegeNum = default;
  
    //＝＝＝＝＝＝＝＝＝＝＝＝＝シーン処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private bool isLoadingScene = false;
    [SerializeField] private string nextSceneName = default;
    public string SetNextSceneName { set { nextSceneName = value; } }

    //＝＝＝＝＝＝＝＝＝＝＝＝＝音量処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private bool isTurnOff = false;
    [SerializeField] private Image volumeImage = default;
    [SerializeField] private Sprite turnOn = default;
    [SerializeField] private Sprite turnOff = default;


    private void Start()
    {
        gdsm = GameDataStorageManager.Instance;

        stageNumText.text = stegeNum;

        //リトライ回数回数が3の倍数の場合はリトライボタン表示
        if (gdsm.RetryCount != 0 && gdsm.RetryCount % 3 == 0)
        {
            skipRecomButton.SetActive(true);
        }
        else
        {
            skipRecomButton.SetActive(false);
        }

        CheckVolumeSetting();
        FadeIn();
    }


    private void Update()
    {
        if (isCleared)
        {
            timeElapsed += Time.deltaTime;
            
            if ( (Input.GetMouseButtonDown(0) && !isFading && !isLoadingScene) || (timeElapsed > fadeOutDelayTime && !isFading && !isLoadingScene) )
            {
                ClearedStage();
            }

        }
    }

    private void FadeIn()
    {
        fadePanel.SetActive(true);
        var seq = DOTween.Sequence()
            .Append(fadePanel.GetComponent<Image>().DOColor(new Color(1,1,1,0) , fadeInTime)).SetEase(Ease.InQuint)
            .OnComplete(() => fadePanel.SetActive(false))
            .Play();
    }

    private void FadeOut()
    {
        isFading = true;
        fadePanel.SetActive(true);
        SEManager.Instance.FadeOut(SEPath.GOAL_BELL,fadeOutTime);　//音がなっていればフェードアウト

        var seq = DOTween.Sequence()
            .Append(fadePanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), fadeOutTime)).SetEase(Ease.InQuad)
            .Play();
    }

    public void SetActivateMenuPanel()
    {
        if (!isMenuPanelActivating)
        {
            Time.timeScale = 0f;
            isMenuPanelActivating = true;
            menuPanelAnim.SetBool("isTapped", true);
            BGMManager.Instance.Pause();
            SEManager.Instance.Pause();

            stopButton.GetComponent<Image>().sprite = playImage;
            stopButton.GetComponent<Image>().SetNativeSize();
            stopButtonShadow.GetComponent<Image>().sprite = playImage;
            stopButtonShadow.GetComponent<Image>().SetNativeSize();
        }
        else
        {
            Time.timeScale = 1f;
            isMenuPanelActivating = false;
            menuPanelAnim.SetBool("isTapped", false);
            BGMManager.Instance.UnPause();
            SEManager.Instance.UnPause();

            stopButton.GetComponent<Image>().sprite = stopImage;
            stopButton.GetComponent<Image>().SetNativeSize();
            stopButtonShadow.GetComponent<Image>().sprite = stopImage;
            stopButtonShadow.GetComponent<Image>().SetNativeSize();
        }
    }

    private void CheckVolumeSetting()
    {
        if(AudioListener.volume == 0)
        {
            isTurnOff = true;
            volumeImage.sprite = turnOff;
            volumeImage.SetNativeSize();
        }
        else
        {
            isTurnOff = false;
            volumeImage.sprite = turnOn;
            volumeImage.SetNativeSize();
        }
    }

    public void SetActivateVolume()
    {
        if (!isTurnOff)
        {
            isTurnOff = true;
            volumeImage.sprite = turnOff;
            volumeImage.SetNativeSize();
            AudioListener.volume = 0;
        }
        else
        {
            isTurnOff = false;
            volumeImage.sprite = turnOn;
            volumeImage.SetNativeSize();
            AudioListener.volume = 1;
        }
    }

    public void ActivateStagePanel()
    {
        stagePanelAnim.SetBool("isTapped", true);
        menuPanelAnim.SetBool("isTapped", false);
        stopButton.SetActive(false);
        stopButtonShadow.SetActive(false);
    }

    public void DeactivateStagePanel()
    {
        stagePanelAnim.SetBool("isTapped", false);
        menuPanelAnim.SetBool("isTapped", true);
        stopButton.SetActive(true);
        stopButtonShadow.SetActive(true);
    }

    public void ClearedStage()
    {
        gdsm.CurrentCheckPoint = 0; //中間経過リセット
        gdsm.RetryCount = 0; //リトライ回数リセット
        gdsm.ClearedLevel(stageNum);
        LoadSceneFunc(nextSceneName);

        if (isArrivedGoal)
        {
            gdsm.RemoveSkippedLevel(stageNum);
        }
    }


    public void RestartStage()
    {
        gdsm.RetryCount = gdsm.RetryCount + 1; //リトライ回数プラス
        //Debug.Log("リトライ回数プラス　：" + gdsm.RetryCount + "回目");

        Time.timeScale = 1f; //タイムスケールを戻す
        
        // 現在のScene名を取得する
        Scene loadScene = SceneManager.GetActiveScene();
        LoadSceneFunc(loadScene.name);
    }

    public void BackToTitle()
    {
        gdsm.RetryCount = 0; //リトライ回数リセット
        gdsm.CurrentCheckPoint = 0; //中間経過リセット
        Time.timeScale = 1f; //タイムスケールを戻す
        LoadSceneFunc("Title");
    }

    public void SkipLevel()
    {
        gdsm.CurrentCheckPoint = 0; //中間経過リセット
        Admob.Instance.ShowInterstitial(); //広告再生
        DeactivateStagePanel();
        SetActivateMenuPanel();
        gdsm.AddSkippedLevel(stageNum); //スキップレベルとして保存
        ClearedStage(); //クリアとして保存 & リトライ回数リセット
    }

    public void GotoSelectLevelScene()
    {
        gdsm.CurrentCheckPoint = 0; //中間経過リセット
        gdsm.RetryCount = 0; //リトライ回数リセット
        LoadSceneFunc("StageSelect");
    }

    public void LoadSceneFunc(string sceneName)
    {
        if (!isLoadingScene)
        {
            Time.timeScale = 1f; //タイムスケールを戻す
            isLoadingScene = true;
            FadeOut();
            SEManager.Instance.FadeOut(fadeOutTime); //音がなっていればフェードアウトする
            BGMManager.Instance.FadeOut(fadeOutTime);
            Admob.Instance.DestroyBanner();
            StartCoroutine(DelayMethod(fadeOutTime, () =>
            {
                // Sceneの読み直し
                SceneManager.LoadScene(sceneName);
            }));
        }
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
