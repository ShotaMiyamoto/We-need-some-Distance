using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KanKikuchi.AudioManager;

public class SubHeartManager : MonoBehaviour
{
    //＝＝＝＝＝＝＝＝＝＝＝＝＝移動処理関連＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    private GameObject mainHeart = default;
    private MainHeartManager mainHeartManager = default;

    [SerializeField] private float areaLength = 2f;
    public float SetAreaLength { set { areaLength = value; } }

    private bool isInArea = false;

    private bool isPushing = false;

    private bool canPush = true;
    public bool SetCanPush { set { canPush = value;  } }
    //＝＝＝＝＝＝＝＝＝＝＝＝＝見た目処理関連＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    private Color defaultColor = default;
    private Color readyColor = default;
    public Color SetReadyColor { set { readyColor = value; } }

    [SerializeField] private bool doSpecialColor = false;

    private SpriteRenderer subHeartSpriteRenderer = default;

    [SerializeField] private GameObject touchEffectSprite = default;
    private SpriteRenderer touchEffectSpriteRenderer = default;

    private Sequence touchEffect = default; //タッチ中のエフェクトアニメーションシーケンス

    //＝＝＝＝＝＝＝＝＝＝＝＝＝Lightning関連＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private LightningBolt2D lightning = default;
    public Color SetLightningColor { set { lightning.glowColor = value; } }

    private Vector3 currentEndPos = default;

    //＝＝＝＝＝＝＝＝＝＝＝＝＝クリア判定関連＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private GameManager gameManager = default;

    //＝＝＝＝＝＝＝＝＝＝＝＝＝サウンド関連＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private BGMManager bgmManager = default;
    [SerializeField] private bool canMakeSound = true;

    private void Start()
    {
        gameManager = GameManager.Instance;
        subHeartSpriteRenderer = GetComponent<SpriteRenderer>();

        mainHeart = GameObject.FindGameObjectWithTag("MainHeart");
        mainHeartManager = mainHeart.GetComponent<MainHeartManager>();

        touchEffectSpriteRenderer = touchEffectSprite.GetComponent<SpriteRenderer>();

        bgmManager = BGMManager.Instance;
        //＝＝＝＝＝＝＝＝＝＝＝＝色同期＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
        ColorManager colorManager = ColorManager.Instance;

        defaultColor = colorManager.GetDefaultColor;

        if (!doSpecialColor)
        {
            readyColor = colorManager.GetSubColor;
            touchEffectSpriteRenderer.color = colorManager.GetSubColor;
        }

        lightning.glowColor = readyColor;

        //＝＝＝＝＝＝＝＝＝＝＝＝＝Lightning初期化＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
        lightning.startPoint = this.transform.position;
        lightning.isPlaying = false;

    }


    // Update is called once per frame
    void Update()
    {
        var dis = Vector3.Distance(this.transform.position, mainHeart.transform.position);

        if(dis <= areaLength && !isInArea)
        {
            Activate();
        }
        else if(dis > areaLength && isInArea)
        {
            Deactivate();
        }


        //加速開始＆エフェクト再生
        if (isInArea && isPushing)
        {
            mainHeartManager.CanAccel = true;
            mainHeartManager.SetDir = this.transform.position - mainHeart.transform.position;

            lightning.startPoint = this.transform.position;
            lightning.endPoint = Vector3.MoveTowards(currentEndPos, mainHeart.transform.position, Time.deltaTime * 10);
            currentEndPos = lightning.endPoint;
        }
    }

    public void PushDown()
    {
        if (canPush && isInArea && !isPushing && !gameManager.IsPlayerDied && !gameManager.IsCreared)
        {
            //Debug.Log("引き寄せ開始");
            mainHeartManager.CanAccel = true;
            lightning.isPlaying = true;
            lightning.EaseIn(0.25f);
            lightning.endPoint = this.transform.position;
            currentEndPos = this.transform.position;
            isPushing = true;


            //サウンド再生（すでに再生している場合はストップさせる）
            if (canMakeSound)
            {
                bgmManager.Stop(BGMPath.PULL_SOUND);
                bgmManager.FadeIn(BGMPath.PULL_SOUND, 1f);
                bgmManager.Play(BGMPath.PULL_SOUND, 0.4f, 0, 1, true);
            }

            //タッチ中のエフェクト再生
            SetActivateTouchEffect(true);
        }
    }

    public void PushUp()
    {
        if (canPush)
        {
            //Debug.Log("タップ終了");
            mainHeartManager.CanAccel = false;
            lightning.EaseOut(0.5f);
            lightning.isPlaying = false;
            isPushing = false;
            SetActivateTouchEffect(false);

            if (canMakeSound)
            {
                bgmManager.FadeOut(BGMPath.PULL_SOUND, 0.5f);
            }
        }
    }

    /// <summary>
    /// タッチエフェクトのループを続けるかどうか
    /// </summary>
    private void CheckContinueTween()
    {
        if (!isPushing || gameManager.IsPlayerDied || gameManager.IsCreared)
        {
            SetActivateTouchEffect(false);
            PushUp();
        }
        else
        {
            SetActivateTouchEffect(true); //再度エフェクト再生
            //Debug.Log("まだ触ってるからエフェクト継続" + name);
        }
    }

    private void Activate()
    {
        isInArea = true;

        //PopUpアニメーション
        var seq1 = DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(0.4f, 0.4f, 0), 0.1f)).SetEase(Ease.InOutCirc)
            .Append(transform.DOScale(new Vector3(0.6f, 0.6f, 0), 0.25f)).SetEase(Ease.InOutCirc)
            .Append(transform.DOScale(new Vector3(0.5f, 0.5f, 0), 0.25f)).SetEase(Ease.InOutCirc);

        //色変更アニメーション
        var seq2 = DOTween.Sequence()
            .Append(subHeartSpriteRenderer.DOColor(readyColor, 0.6f)).SetEase(Ease.InOutCirc);

        //seq1再生と同時にseq2も再生
        seq1
            .OnStart(() => seq2.Play())
            .Play();
    }

    private void Deactivate()
    {
        isInArea = false;
        mainHeartManager.CanAccel = false;

        //色変更アニメーション
        var seq1 = DOTween.Sequence()
            .Append(subHeartSpriteRenderer.DOColor(defaultColor, 0.5f)).SetEase(Ease.InOutCirc)
            .Play();

        //タッチ中のエフェクトが再生されているなら停止させる
        SetActivateTouchEffect(false);
    }

    private void SetActivateTouchEffect(bool val)
    {
        if (val)
        {
            if(touchEffect != null)
            {
                touchEffect.Kill();
            }

            touchEffect = DOTween.Sequence();
            touchEffect
                .OnStart(() => 
                {
                    touchEffectSpriteRenderer.color = readyColor;
                    touchEffectSprite.transform.localScale = new Vector3(1f, 1f, 0);
                })
                .Append(touchEffectSprite.transform.DOScale(new Vector3(3f, 3f, 0), 1f).SetRelative().SetEase(Ease.OutExpo))
                .Join(touchEffectSpriteRenderer.DOColor(readyColor - new Color(0, 0, 0, 1), 1f))
                .OnStepComplete(() => CheckContinueTween())
                .Play();
        }
        else
        {
            if (touchEffect != null)
            {
                if (touchEffect.IsPlaying())
                {
                    touchEffect.Pause();
                    touchEffectSpriteRenderer.color = readyColor;
                    touchEffectSprite.transform.localScale = new Vector3(1f,1f, 0);
                }
            }
        }

    }

    public void SetTouch(bool val)
    {
        canPush = val;
        this.GetComponent<CircleCollider2D>().enabled = val;
        this.transform.Find("TapCollider").GetComponent<CircleCollider2D>().enabled = val;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "MainHeart")
        {
            PushUp();
            touchEffect.Pause();
            touchEffectSprite.transform.localScale = new Vector3(1f, 1f, 0);
        }
    }
}
