using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerFollowHeartController : MonoBehaviour
{
    [SerializeField] private GameObject glowHeart = default;
    private float originPos = default;
    [SerializeField] private float setPos = default;
    [SerializeField] private float followXPos = default;
    [SerializeField] private float followYPos = default;

    private bool isBacked = false;
    private float timeElapsed = 0f;

    private void Start()
    {
        originPos = this.gameObject.transform.position.x;
        ChangeFollowPos(setPos, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(followXPos, glowHeart.transform.position.y + followYPos, 0);
        timeElapsed += Time.deltaTime;
        if(!isBacked && timeElapsed > 60f)
        {
            isBacked = true;
            ChangeFollowPos(originPos, 4f);
        }
    }

    public void ChangeFollowPos(float pos,float duration)
    {
        DOTween.To(() => followXPos, x => followXPos = x, pos, duration);
    }

}
