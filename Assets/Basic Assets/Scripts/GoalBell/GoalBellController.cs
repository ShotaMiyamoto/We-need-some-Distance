using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KanKikuchi.AudioManager;

public class GoalBellController : MonoBehaviour
{
    [SerializeField] private GameObject rotateRoot = default;
    [SerializeField] private GameObject bellBodySprite = default;
    [SerializeField] private GameObject bellBallSprite = default;

    [SerializeField] private SpriteRenderer emptyHeartSprite = default;
    [SerializeField] private GameObject waveEffect = default;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "MainHeart" && !GameManager.Instance.IsCreared && !GameManager.Instance.IsPlayerDied)
        {
            GameManager.Instance.SetIsArrivedGoal = true;
            GameManager.Instance.IsCreared = true;

            collision.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            collision.GetComponent<Rigidbody2D>().simulated = false;
            collision.transform.parent = rotateRoot.transform;

            SpriteRenderer bellBodySpriteRenederer = bellBodySprite.GetComponent<SpriteRenderer>();
            SpriteRenderer bellBallSpriteRenderer = bellBallSprite.GetComponent<SpriteRenderer>();
            SpriteRenderer waveSpriteRenderer = waveEffect.GetComponent<SpriteRenderer>();
            Vector3 originScale = waveEffect.transform.localScale;
            Color originColor = waveSpriteRenderer.color;

            //波紋アニメーション
            var wave = DOTween.Sequence()
                .SetLoops(4, LoopType.Restart)
                .Append(waveEffect.transform.DOScale(new Vector3(1f, 1f, 0), 1.5f).SetEase(Ease.OutExpo))
                .Join(waveSpriteRenderer.DOColor(new Color(0.7f, 0.7f, 0.7f, 0f), 1.5f).SetEase(Ease.OutExpo))
                .Pause();


            //ハートゲットアニメーション
            var getAnim = DOTween.Sequence()
                .OnStart(() => SEManager.Instance.Play(SEPath.ARRIVED,0.4f,1.2f)) //ハートが移動するSEを鳴らす)
                .Append(rotateRoot.transform.DORotate(new Vector3(0, 0, 720f), 1.5f).SetRelative().SetEase(Ease.InOutCubic))
                .Join(collision.transform.DOLocalMove(new Vector3(0, -0.5f, 0), 1.5f).SetEase(Ease.InOutCubic))
                .Join(collision.transform.DORotate(new Vector3(0, 0, 0), 1.5f).SetEase(Ease.InOutCubic))
                .Join(collision.transform.DOScale(Vector3.zero, 1.5f).SetEase(Ease.InQuart))
                .Pause();

            //色変更アニメーション
            var colorChange = DOTween.Sequence()
                .Append(bellBodySpriteRenederer.DOColor(ColorManager.Instance.GetGoalBellColor, 0.5f)).SetEase(Ease.InOutCirc)
                .Join(bellBallSpriteRenderer.DOColor(ColorManager.Instance.GetGoalBellColor, 0.5f)).SetEase(Ease.InOutCirc)
                .Join(emptyHeartSprite.DOColor(ColorManager.Instance.GetMainColor, 0.5f).SetEase(Ease.InOutCirc))
                .OnStart(() => wave.Play()) //波紋アニメーション再生開始

                .Pause();


            //鐘が揺れるアニメーション(本体)
            var bellBody = DOTween.Sequence()
                .OnStart(() => SEManager.Instance.Play(SEPath.GOAL_BELL, 1, 0.8f)) //鐘のSEを鳴らす

                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, 11.5f), 0.5f).SetEase(Ease.InOutSine))
                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, -11.5f), 1f).SetEase(Ease.InOutSine))

                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, 11.5f), 0.5f).SetEase(Ease.InOutSine))
                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, -11.5f), 1f).SetEase(Ease.InOutSine))

                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, 11.5f), 0.5f).SetEase(Ease.InOutSine))
                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, -11.5f), 1f).SetEase(Ease.InOutSine))

                .Append(bellBodySprite.transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.InOutSine))
                .Pause();

            //鐘が揺れるアニメーション(鐘の玉)
            var bellBall = DOTween.Sequence()
                .Append(bellBallSprite.transform.DOLocalMoveX(0.5f, 0.5f)).SetEase(Ease.InExpo)

                .Append(bellBallSprite.transform.DOLocalMoveX(-1f, 0.9f)).SetEase(Ease.InOutSine)
                .Append(bellBallSprite.transform.DOLocalMoveX(1f, 0.9f)).SetEase(Ease.InOutSine)

                .Append(bellBallSprite.transform.DOLocalMoveX(-1f, 0.9f)).SetEase(Ease.InOutSine)
                .Append(bellBallSprite.transform.DOLocalMoveX(1f, 0.9f)).SetEase(Ease.InOutSine)

                .Append(bellBallSprite.transform.DOLocalMoveX(-1f, 0.9f)).SetEase(Ease.InOutSine)
                .Append(bellBallSprite.transform.DOLocalMoveX(0.5f, 0.9f)).SetEase(Ease.InOutSine)

                .Append(bellBallSprite.transform.DOLocalMoveX(0, 1f)).SetEase(Ease.InOutSine)
                .Pause();


            //順番を決めて再生するシーケンス
             var playSeq = DOTween.Sequence()
                .Append(getAnim)
                .Append(colorChange) //ここでwaveシーケンスも再生する
                .Append(bellBody)
                .Join(bellBall)
                .Play();
        }
    }
}
