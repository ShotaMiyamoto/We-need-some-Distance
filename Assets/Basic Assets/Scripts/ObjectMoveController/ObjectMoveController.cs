using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectMoveController : MonoBehaviour
{
    [SerializeField] private bool doRotate = false;
    [SerializeField] private bool doSlide = false;

    [SerializeField] private bool doLoop = true;

    [SerializeField] private float rotateAngle = 360f;
    [SerializeField] private Vector3 moveDir = default;

    [SerializeField] private float rotateDuration = 1.5f;
    [SerializeField] private float moveDuration = 1.5f;

    public Ease rotateEaseing = default;
    public LoopType rotateLoopType = default;

    public Ease moveEaseing = default;
    public LoopType moveLoopType = default;

    [SerializeField] private float waitTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (doRotate)
        {
            Rotate();
        }

        if (doSlide)
        {
            Slide();
        }
    }


    private void Slide()
    {
        if (doLoop)
        {
            var seq = DOTween.Sequence()
            .SetLoops(-1, moveLoopType)
            .Append(transform.DOLocalMove(moveDir, moveDuration).SetRelative().SetEase(moveEaseing))
            .PrependInterval(waitTime)
            .Append(transform.DOLocalMove(-moveDir, moveDuration).SetRelative().SetEase(moveEaseing))
            .PrependInterval(waitTime)
            .Play();
        }
        else
        {
            var seq = DOTween.Sequence()
            .Append(transform.DOLocalMove(moveDir, moveDuration).SetRelative().SetEase(moveEaseing))
            .PrependInterval(waitTime)
            .Play();
        }

    }


    private void Rotate()
    {
        if (doLoop)
        {
            var seq = DOTween.Sequence()
            .SetLoops(-1, rotateLoopType)
            .Append(transform.DORotate(new Vector3(0, 0, rotateAngle), rotateDuration).SetRelative().SetEase(rotateEaseing))
            .Play();
        }
        else
        {
            var seq = DOTween.Sequence()
            .Append(transform.DORotate(new Vector3(0, 0, rotateAngle), rotateDuration).SetRelative().SetEase(rotateEaseing))
            .Play();
        }

    }
}
