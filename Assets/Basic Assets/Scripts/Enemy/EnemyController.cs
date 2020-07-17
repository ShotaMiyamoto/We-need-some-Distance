using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    //＝＝＝＝＝＝＝＝＝＝＝＝＝追跡用＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private GameManager gameManager = default;

    private Transform mainHeartTrans = default;
    private MainHeartManager mainHeartManager = default;

    [SerializeField] private float areaLength = 3f;
    private bool isInArea = false;
    private bool isMoving = false;

    private Rigidbody2D rb = default;
    [SerializeField] private float moveSpeed = 2f;
    //＝＝＝＝＝＝＝＝＝＝＝＝＝色変更用＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] private SpriteRenderer activeSprite = default;

    // Start is called before the first frame update
    void Start()
    {
        mainHeartTrans = GameObject.FindGameObjectWithTag("MainHeart").GetComponent<Transform>();
        mainHeartManager = GameObject.FindGameObjectWithTag("MainHeart").GetComponent<MainHeartManager>();

        rb = GetComponent<Rigidbody2D>();
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        var dis = Vector3.Distance(this.transform.position, mainHeartTrans.position);

        if (dis <= areaLength && !isInArea)
        {
            Activate();
        }
        else if (dis > areaLength && isInArea)
        {
            Deactivate();
        }
    }

    private void FixedUpdate()
    {
        if (isInArea)
        {
            if (mainHeartManager.CanAccel && !gameManager.IsCreared)
            {
                var dir = mainHeartTrans.position - this.transform.position;
                rb.velocity = dir.normalized * moveSpeed;
                if (!isMoving)
                {
                    isMoving = true;
                }
            }
            else
            {
                if (isMoving)
                {
                    isMoving = false;
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }

    private void Activate()
    {
        isInArea = true;
        activeSprite.DOColor(new Color(222f / 255f, 33f / 255f, 33f / 255f, 1f), 0.1f);
    }

    private void Deactivate()
    {
        isInArea = false;
        rb.velocity = Vector2.zero;
        activeSprite.DOColor(new Color(41f / 255f, 41f / 255f, 41f / 255f, 1f), 0.5f);
    }
}
