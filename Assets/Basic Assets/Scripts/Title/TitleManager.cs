using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject titleText = default;
    [SerializeField] private float timeLeft = 0.3f;


    [SerializeField] private GameObject fadePanel = default;
    [SerializeField] private float fadeInTime = 1.5f;
    [SerializeField] private float fadeOutTime = 3f;

    [SerializeField] private GameObject newStageText = default;

    private bool isLoadingScene = false;


    private void FadeIn()
    {
        var seq = DOTween.Sequence()
            .Append(fadePanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), fadeInTime)).SetEase(Ease.InQuint)
            .OnComplete(() =>
                {
                    fadePanel.SetActive(false);
                    BGMManager.Instance.Play(BGMPath.SB_LIFEIS_SHORTVER, 1f, 1f, 1f, true);
                })
            .Play();
    }


    private void Start()
    {
        FadeIn();

        if(GameDataStorageManager.Instance.GetLatestClearedlevelNum == 10)
        {
            newStageText.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0.0f)
        {
            RandomizePosition();
            timeLeft = 0.2f;
        }
    }

    public void StartGame()
    {
        SEManager.Instance.Play(SEPath.TITLE_BELL);
        if (GameDataStorageManager.Instance.GetIsAllCleared)
        {
            LoadSceneFunc("Stage_1");
        }
        else
        {
            LoadSceneFunc("Stage_" + (GameDataStorageManager.Instance.GetLatestClearedlevelNum + 1).ToString());
        }

    }

    public void GoToStageSelect()
    {
        SEManager.Instance.Play(SEPath.TITLE_BELL);
        LoadSceneFunc("StageSelect");
    }

    private void RandomizePosition()
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-0.02f, 0.02f), UnityEngine.Random.Range(-0.02f, 0.02f), 0);
        titleText.transform.position = pos;

    }

    public void LoadSceneFunc(string sceneName)
    {
        if (!isLoadingScene)
        {
            Time.timeScale = 1f; //タイムスケールを戻す
            isLoadingScene = true;
            FadeOut();
            BGMManager.Instance.FadeOut(fadeOutTime); //BGMが鳴っていればフェードアウトする
            SEManager.Instance.FadeOut(fadeOutTime); //音がなっていればフェードアウトする
            Admob.Instance.DestroyBanner();
            StartCoroutine(DelayMethod(fadeOutTime, () =>
            {
                // Sceneの読み直し
                SceneManager.LoadScene(sceneName);
            }));
        }
    }

    private void FadeOut()
    {
        fadePanel.SetActive(true);

        var seq = DOTween.Sequence()
            .Append(fadePanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), fadeOutTime)).SetEase(Ease.InQuad)
            .Play();
    }


    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://gist.github.com/ShotaMiyamoto/402a1cf39e0b47a094aa9e23bdea47e9");
    }


    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

}
