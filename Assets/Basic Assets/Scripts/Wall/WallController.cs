using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WallController : MonoBehaviour
{
    [SerializeField] private float timeLimit = 1f;
    private float timeElapsed = 0;
    private bool isTouched = false;
    [SerializeField] private bool canBounce = true;
    [SerializeField] private bool canRotate = false;
    [SerializeField] private float rotateSpeed = -0.1f;
    [SerializeField] private Vector3 reactScale = new Vector3(0.01f, 0.01f, 0);

    private void Start()
    {
        if (canBounce)
        {
            this.GetComponent<SpriteRenderer>().color = ColorManager.Instance.GetWallColor;
        }
        else
        {
            this.GetComponent<Collider2D>().sharedMaterial = null;
        }
    }

    private void Update()
    {
        if (isTouched)
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed > timeLimit)
            {
                timeElapsed = 0f;
                isTouched = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (canRotate)
        {
            this.transform.Rotate(new Vector3(this.transform.rotation.x, this.transform.rotation.y, rotateSpeed));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "MainHeart" && !isTouched && canBounce)
        {
            //PopUpアニメーション
            var originScale = this.transform.localScale;

            var seq1 = DOTween.Sequence()
                .Append(transform.DOScale(originScale + reactScale, 0.15f)).SetEase(Ease.OutExpo)
                .Append(transform.DOScale(originScale, 0.15f).SetEase(Ease.OutExpo))
                .Play();

            isTouched = true;
        }
    }
}
