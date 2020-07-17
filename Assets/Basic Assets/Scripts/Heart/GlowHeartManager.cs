using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class GlowHeartManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    private bool isTapped = false;
    private Sequence seq = default;
    [SerializeField] private GameObject deadLine = default;
    [SerializeField] private Stage10objectManager stage10ObjectManager = default;

    public void StartToMove()
    {
        if (!isTapped)
        {
            stage10ObjectManager.StartPlay();
            stage10ObjectManager.SetCanCount = true;
            seq = DOTween.Sequence();
            isTapped = true;
            seq
                .SetLoops(-1, LoopType.Incremental)
                .Join(this.transform.DOMoveY(this.transform.position.y + moveSpeed, 1f).SetEase(Ease.Linear))
                .Play();
        }
    }

    private void Update()
    {
        deadLine.transform.position = this.transform.position + new Vector3(0, -8f, 0);
    }

    public void StopMove()
    {
        seq.Pause();
        deadLine.SetActive(false);
    }

}
