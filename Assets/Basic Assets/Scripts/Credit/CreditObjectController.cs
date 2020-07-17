using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UB.Simple2dWeatherEffects.Standard;
using DG.Tweening;

public class CreditObjectController : MonoBehaviour
{
    [SerializeField] private LightningBolt2D lightning = default;
    [SerializeField] private GameObject pinkHeart = default;
    [SerializeField] private GameObject blueHeart = default;

    [SerializeField] private D2FogsNoiseTexPE fogNoise = default;

    private void Start()
    {
        Floating();
    }

    // Update is called once per frame
    void Update()
    {
        lightning.startPoint = pinkHeart.transform.position;
        lightning.endPoint = blueHeart.transform.position;
    }

    public void Floating()
    {
        var seq = DOTween.Sequence()
            .SetLoops(-1,LoopType.Yoyo)
            .Append(pinkHeart.transform.DOMoveY(Random.Range(-0.5f, 0.5f), 3f).SetEase(Ease.InOutQuad).SetRelative())
            .Join(blueHeart.transform.DOMoveY(Random.Range(-0.5f, 0.5f), 3f).SetEase(Ease.InOutQuad).SetRelative())
            .AppendCallback(() => 
            {
                DOTween.To(() => fogNoise.Density, (x) => fogNoise.Density = x, Random.Range(0.25f, 0.7f), 2f).SetEase(Ease.Linear);
            })
            .Play();
    }
}
