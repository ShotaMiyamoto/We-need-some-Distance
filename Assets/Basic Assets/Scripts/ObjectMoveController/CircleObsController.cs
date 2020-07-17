using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleObsController : MonoBehaviour
{
    private Transform parentTrans = default;
    [SerializeField] private List<SpriteRenderer> sprites = default;
    [SerializeField] private bool isHost = false;
    private float gatherTime = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        parentTrans = transform.parent.transform;
        ChangeSpriteAlpha(1f, 1f);
        Gather();

        if (isHost)
        {
            Invoke("DestroyParent", gatherTime + 5f);
        }
    }

    private void Gather()
    {
        //Debug.Log("フェードイン開始"+name);
        var seq = DOTween.Sequence()
            .OnStart(() => this.GetComponent<CircleCollider2D>().enabled = true)
            .PrependInterval(1.5f)
            .Append(this.transform.DOMove(parentTrans.position + (this.transform.localPosition * 0.3f), gatherTime)).SetEase(Ease.InOutCubic)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            { 
                ChangeSpriteAlpha(0f, 0.25f);
                this.GetComponent<CircleCollider2D>().enabled = false;
            });
    }

    private void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }

    private void ChangeSpriteAlpha(float endValue,float time)
    {

        foreach (var val in sprites)
        {
            val.DOColor(new Color(val.color.r, val.color.g, val.color.b, endValue), time);
        }
    }
}
