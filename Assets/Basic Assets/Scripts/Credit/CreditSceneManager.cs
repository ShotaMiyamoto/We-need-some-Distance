using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KanKikuchi.AudioManager;

public class CreditSceneManager : SingletonMonoBehaviour<CreditSceneManager>
{
    [SerializeField] private GameObject fadePanel = default;
    [SerializeField] private float fadeInTime = 1.5f;
    [SerializeField] private float fadeOutTime = 3f;

    [SerializeField] private PostProcessVolume volume = default;
    private Bloom bloom = default;

    [SerializeField] private bool doChangeBloom = false;

    float fadeTime = 0;

    [SerializeField] private List<GameObject> creditTexts = default;
    private int currentTextNum = 0;


    [SerializeField] private bool showedAllText = false;
    public bool SetShowedAllText { set { showedAllText = value; } }

    private bool isFading = false;

    protected override void Awake()
    {
        Application.targetFrameRate = 60; // ターゲットフレームレートを60に設定
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (doChangeBloom)
        {
            bloom = ScriptableObject.CreateInstance<Bloom>();
            bloom.enabled.Override(true);
            fadeTime = fadeInTime;
        }
        
        FadeIn();
        BGMManager.Instance.Play(BGMPath.SB_LIFEIS_SHORTVER, 1f, 1f, 1f, true);
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeTime > 0 && doChangeBloom)
        {
            fadeTime -= Time.deltaTime;
            bloom.intensity.Override(fadeTime);
            volume = PostProcessManager.instance.QuickVolume(this.gameObject.layer, 0, bloom);
        }

        if(showedAllText && Input.GetMouseButtonDown(0) && !isFading)
        {
            isFading = true;
            BGMManager.Instance.FadeOut(BGMPath.SB_LIFEIS_SHORTVER);
            SEManager.Instance.Play(SEPath.TITLE_BELL, 0.4f);
            FadeOut();
        }

    }

    private void FadeIn()
    {
        var seq = DOTween.Sequence()
            .Append(fadePanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), fadeInTime)).SetEase(Ease.InQuint)
            .OnComplete(() =>
            {
                fadePanel.SetActive(false);
                ShowCreditText();
            })
            .Play();
    }

    private void FadeOut()
    {
        isFading = true;
        fadePanel.SetActive(true);

        var seq = DOTween.Sequence()
            .Append(fadePanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), fadeOutTime)).SetEase(Ease.InQuad)
            .OnComplete(() => 
            { 
                BackToTitle();
            })
            .Play();
    }

    public void ShowCreditText()
    {
        creditTexts[currentTextNum].SetActive(true);
        currentTextNum++;
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
