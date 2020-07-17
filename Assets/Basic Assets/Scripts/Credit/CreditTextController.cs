using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CreditTextController : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float fadeOutTime = 3f;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float startDelay = 0f;
    [SerializeField] private bool neverFadeOut = false;
    [SerializeField] private bool isHost = false;
    [SerializeField] private bool isLastText = false;

    // Start is called before the first frame update
    void Start()
    {
        Text text = this.GetComponent<Text>();
        if (neverFadeOut)
        {
            var seq = DOTween.Sequence()
                .PrependInterval(startDelay)
                .Append(text.DOColor(new Color(50f / 255f, 50f / 255f, 50f / 255f, 1), fadeInTime).SetEase(Ease.InOutSine))
                .OnComplete(() =>
                {
                    if (isLastText)
                    {
                        CreditSceneManager.Instance.SetShowedAllText = true;
                    }
                })
                .Play();
        }
        else
        {
            var seq = DOTween.Sequence()
                .PrependInterval(startDelay)
                .Append(text.DOColor(new Color(50f / 255f, 50f / 255f, 50f / 255f, 1), fadeInTime).SetEase(Ease.InOutSine))
                .AppendInterval(displayTime)
                .Append(text.DOColor(new Color(50f / 255f, 50f / 255f, 50f / 255f, 0), fadeOutTime).SetEase(Ease.InOutSine))
                .OnComplete(() =>
                {
                    if (isHost)
                    {
                        CreditSceneManager.Instance.ShowCreditText();
                    }
                    this.gameObject.SetActive(false);
                })
                .Play();
        }
    }
}
