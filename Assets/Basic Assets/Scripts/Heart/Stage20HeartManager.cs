using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stage20HeartManager : MonoBehaviour
{
    //＝＝＝＝＝＝＝＝＝＝＝＝＝出現関連＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private GameObject mainHeart = default;

    private bool isAppeared = true;

    [SerializeField] private SpriteRenderer[] heartImages = default;

    private SubHeartManager subHeartManager = default;

    private ParticleSystem particle = default;

    [SerializeField] private GameObject eventEnemy = default;

    private bool canFadeOut = true;

    private bool isEnded = false;

    [SerializeField] private Stage20Manager stage20Manager = default;

    [SerializeField] private SpriteRenderer shadow = default;

    private void Start()
    {
        subHeartManager = this.GetComponent<SubHeartManager>();
        particle = this.GetComponent<ParticleSystem>();

        eventEnemy.SetActive(false);
        shadow.color = new Color(0,0,0,0);
    }

    private void Update()
    {
        if (isAppeared && canFadeOut)
        {
            var dis = Vector2.Distance(mainHeart.transform.position, this.transform.position);

            if (dis < 1f)
            {
                Disappear();
            }
        }

        if (!canFadeOut && !isEnded)
        {
            var dis = Vector2.Distance(mainHeart.transform.position, this.transform.position);

            if (dis < 0.5f)
            {
                isEnded = true;
                stage20Manager.ClearStage();
            }
        }
    }

    public void Appear()
    {
        isAppeared = true;
        subHeartManager.SetTouch(true);
        particle.Play();
        foreach (var val in heartImages)
        {
            val.DOColor(new Color(val.color.r, val.color.g, val.color.b, 1), 1f);
        }
    }

    public void AppearAtAround()
    {
        float x;
        do { x = Random.Range(-2.2f, 2.2f); }
        while (x < 0.3f && x > -0.3f);

        float y;
        do { y = Random.Range(-3.6f, 3f); }
        while (y < 2f && y > -2f);

        this.transform.position = new Vector2(mainHeart.transform.position.x + x, mainHeart.transform.position.y + y);
        
        isAppeared = true;
        subHeartManager.SetTouch(true);
        particle.Play();
        foreach (var val in heartImages)
        {
            val.DOColor(new Color(val.color.r, val.color.g, val.color.b, 1), 1f);
        }
    }

    public void AppearAtAbove()
    {
        float x = Random.Range(-2.2f, 2.2f);

        float y = Random.Range(2.5f, 3.5f);

        this.transform.position = new Vector2(mainHeart.transform.position.x + x, mainHeart.transform.position.y + y);

        isAppeared = true;
        subHeartManager.SetTouch(true);
        particle.Play();
        foreach (var val in heartImages)
        {
            val.DOColor(new Color(val.color.r, val.color.g, val.color.b, 1), 1f);
        }
    }

    public void Disappear()
    {
        isAppeared = false;
        subHeartManager.PushUp();
        subHeartManager.SetTouch(false);
        particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        foreach (var val in heartImages)
        {
            val.DOColor(new Color(val.color.r, val.color.g, val.color.b, 0), 0.5f);
        }

    }

    public void FallDown()
    {
        this.transform.position = mainHeart.transform.position + new Vector3(0, 4f, 0);　//位置移動とフェードイン
        Appear();
        shadow.DOColor(new Color(0,0,0, 45 / 255f),1f);

        eventEnemy.transform.position = this.transform.position;　//敵の位置移動

        

        var seq = DOTween.Sequence()
            .OnStart(() =>
            {
                subHeartManager.SetTouch(false); //引き寄せ不可にする
                heartImages[1].enabled = false; //エフェクト無効化
                eventEnemy.SetActive(true);
            })
            .AppendInterval(6.5f)


            .Append(heartImages[0].DOColor(new Color(41f / 255f, 41f / 255f, 41f / 255f, 1f), 1f).SetEase(Ease.InQuart)) //黒くなる

            .AppendInterval(1.5f)

            .Append(this.transform.DOMoveY(this.transform.position.y - 0.8f, 3f).SetEase(Ease.InOutCubic))
            .Join(this.transform.DORotate(new Vector3(0, 0, 30f), 3f).SetEase(Ease.InOutSine))

            .OnComplete(() =>
            {
                Destroy(eventEnemy); //Enemy削除

                heartImages[1].enabled = true; //エフェクトを有効化する
                subHeartManager.SetReadyColor = new Color(41f / 255f, 41f / 255f, 41f / 255f, 1f); //色設定
                subHeartManager.SetLightningColor = new Color(41f / 255f, 41f / 255f, 41f / 255f, 1f); //色設定
                subHeartManager.SetTouch(true); //引き寄せ可にする

                subHeartManager.PushUp();　
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);　//パーティクルの生成を止める
                canFadeOut = false;
            })  
            .Play();
    }

    public void GoingUp(float distance, float duration)
    {
        this.transform.position = new Vector2(mainHeart.transform.position.x, mainHeart.transform.position.y + 3f);

        isAppeared = true;
        subHeartManager.SetTouch(true);
        particle.Play();
        foreach (var val in heartImages)
        {
            val.DOColor(new Color(val.color.r, val.color.g, val.color.b, 1), 1f);
        }

        this.transform.DOMoveY(distance, duration).SetEase(Ease.Linear).SetRelative();
    }

}
