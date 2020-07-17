using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KanKikuchi.AudioManager;

public class StageSelectBell : MonoBehaviour
{
    [SerializeField] private int stageNum = default;

    [SerializeField] private GameObject rotateRoot = default;
    [SerializeField] private GameObject bellBodySprite = default;
    [SerializeField] private GameObject bellBallSprite = default;

    [SerializeField] private SpriteRenderer emptyHeartSprite = default;
    [SerializeField] private GameObject waveEffect = default;

    [SerializeField] private Color clearedBellColor = default;
    [SerializeField] private Color clearedHeartColor = default;

    [SerializeField] private bool canPlay = false;

    [SerializeField] private SubHeartManager subHeartManager = default;

    // Start is called before the first frame update
    void Start()
    {
        //クリアしていればクリア用の色に変更
        if (GameDataStorageManager.Instance.GetLatestClearedlevelNum >= stageNum)
        {
            canPlay = true;
            subHeartManager.SetAreaLength = 10f;

            SpriteRenderer bellBodySpriteRenederer = bellBodySprite.GetComponent<SpriteRenderer>();
            SpriteRenderer bellBallSpriteRenderer = bellBallSprite.GetComponent<SpriteRenderer>();

            bellBodySpriteRenederer.color = clearedBellColor;
            bellBallSpriteRenderer.color = clearedBellColor;

            Debug.Log("クリアしているので鐘の色を反映：" + name);

            //スキップした要素として登録されていなければクリアカラーを反映
            if (GameDataStorageManager.Instance.GetSkippedLevel != null)
            {
                if (GameDataStorageManager.Instance.GetSkippedLevel.IndexOf(stageNum) == -1)
                {
                    Debug.Log("スキップしていないのでハートの色を反映：" + name);
                    emptyHeartSprite.color = clearedHeartColor;
                }
            }
        }
        else 
        {
            if (GameDataStorageManager.Instance.GetLatestClearedlevelNum == (stageNum - 1))
            {
                canPlay = true;
                subHeartManager.SetAreaLength = 10f;
                Debug.Log("クリアしていないけど、次のステージなので有効");
            }
        }
        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MainHeart" && canPlay)
        {

            collision.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            collision.GetComponent<Rigidbody2D>().simulated = false;
            collision.transform.parent = rotateRoot.transform;

            SpriteRenderer bellBodySpriteRenederer = bellBodySprite.GetComponent<SpriteRenderer>();
            SpriteRenderer bellBallSpriteRenderer = bellBallSprite.GetComponent<SpriteRenderer>();
            SpriteRenderer waveSpriteRenderer = waveEffect.GetComponent<SpriteRenderer>();
            Vector3 originScale = waveEffect.transform.localScale;
            Color originColor = waveSpriteRenderer.color;

            //波紋アニメーション
            var wave = DOTween.Sequence()
                .SetLoops(2, LoopType.Restart)
                .Append(waveEffect.transform.DOScale(new Vector3(1f, 1f, 0), 1.5f).SetEase(Ease.OutExpo))
                .Join(waveSpriteRenderer.DOColor(new Color(0.7f, 0.7f, 0.7f, 0f), 1.5f).SetEase(Ease.OutExpo))
                .Pause();


            //ハートゲットアニメーション
            var getAnim = DOTween.Sequence()
                .OnStart(() => SEManager.Instance.Play(SEPath.ARRIVED, 0.4f, 1.2f)) //ハートが移動するSEを鳴らす)
                .Append(rotateRoot.transform.DORotate(new Vector3(0, 0, 720f), 1.5f).SetRelative().SetEase(Ease.InOutCubic))
                .Join(collision.transform.DOLocalMove(new Vector3(0, -0.5f, 0), 1.5f).SetEase(Ease.InOutCubic))
                .Join(collision.transform.DORotate(new Vector3(0, 0, 0), 1.5f).SetEase(Ease.InOutCubic))
                .Join(collision.transform.DOScale(Vector3.zero, 1.5f).SetEase(Ease.InQuart))
                .OnComplete(() => wave.Play()) //波紋アニメーション再生開始
                .Pause();

            //鐘が揺れるアニメーション(本体)
            var bellBody = DOTween.Sequence()
                .OnStart(() => SEManager.Instance.Play(SEPath.TITLE_BELL, 1, 0.8f)) //鐘のSEを鳴らす

                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, 11.5f), 0.5f).SetEase(Ease.InOutSine))
                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, -11.5f), 1f).SetEase(Ease.InOutSine))

                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.InOutSine))
                .Pause();

            //鐘が揺れるアニメーション(鐘の玉)
            var bellBall = DOTween.Sequence()
                .Append(bellBallSprite.transform.DOLocalMoveX(0.5f, 0.5f)).SetEase(Ease.InExpo)

                .Append(bellBallSprite.transform.DOLocalMoveX(-1f, 0.9f)).SetEase(Ease.InOutSine)
                .Append(bellBallSprite.transform.DOLocalMoveX(1f, 0.9f)).SetEase(Ease.InOutSine)

                .Append(bellBallSprite.transform.DOLocalMoveX(0, 1f)).SetEase(Ease.InOutSine)
                .Pause();


            //順番を決めて再生するシーケンス
            var playSeq = DOTween.Sequence()
               .Append(getAnim)//ここでwaveシーケンスも再生する
               .Append(bellBody)
               .Join(bellBall)
               .OnComplete(() => GameManager.Instance.LoadSceneFunc("Stage_" + stageNum.ToString()))
               .Play();
        }
    }
}
