using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EventEnemyController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] sprites = default;
    [SerializeField] private float waitTime = default;
    private bool isMoved = false;
    private float timeElapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach(var val in sprites)
        {
            val.DOColor(new Color(val.color.r, val.color.g, val.color.b, 1f), 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoved)
        {
            timeElapsed += Time.deltaTime;

            if(timeElapsed > waitTime)
            {
                isMoved = true;
                var seq = DOTween.Sequence()
                    .Append(this.transform.DOMove(this.transform.parent.position, 1.5f).SetEase(Ease.InOutCubic))
                    .Join(this.transform.DOScale(Vector3.zero,1.8f).SetEase(Ease.InCubic))
                    .Play();
            }
        }
    }
}
