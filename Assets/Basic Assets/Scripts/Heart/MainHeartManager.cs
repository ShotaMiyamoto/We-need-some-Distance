using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KanKikuchi.AudioManager;

[RequireComponent(typeof(Rigidbody2D))]
public class MainHeartManager : MonoBehaviour
{
    private Rigidbody2D rb = default;

    /// <summary>
    /// 加速度
    /// </summary>
    [SerializeField] private float moveForce = 0.5f;

    [SerializeField] private float fastForce = 1f;

    [SerializeField] private float slowForce = 0.1f;

    /// <summary>
    /// 係数（減速する割合）
    /// </summary>
    private float coefficient = 0.98f;

    /// <summary>
    /// 加速可能かどうか
    /// </summary>
    private bool canAccel = false;

    public bool CanAccel 
    { 
        set { canAccel = value; }
        get { return canAccel; }
    }

    /// <summary>
    /// 加速する方向
    /// </summary>
    private Vector3 dir = default;

    public Vector3 SetDir { set { dir = value; } }

    [SerializeField] private float maxSpeed = 5f;


    /// <summary>
    /// ゲームオーバーかどうか
    /// </summary>
    private bool isDied = false;

    /// <summary>
    /// ステージセレクト用
    /// </summary>
    private bool isArrived = false;
    public bool GetIsArrived { get { return isArrived; } }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        this.GetComponent<SpriteRenderer>().color = ColorManager.Instance.GetMainColor;
    }

    private void FixedUpdate()
    {
        if (canAccel && !isDied)
        {
            //Debug.Log("加速中" + name);
            if(rb.velocity.magnitude < maxSpeed)
            rb.AddForce(dir * moveForce, ForceMode2D.Force);
        }
        else
        {
            if(rb.velocity.magnitude > 0.01f)
            {
                rb.velocity *= coefficient;
            }
        }

        if (rb.velocity.magnitude > maxSpeed || isDied)
        {
            rb.velocity *= coefficient;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle" && !isDied)
        {
            isDied = true;
            GameManager.Instance.IsPlayerDied = true;
            GameManager.Instance.RestartStage();
            SEManager.Instance.Play(SEPath.FAIL,0.6f);
            BGMManager.Instance.FadeOut();

            var seq1 = DOTween.Sequence()
                .Append(this.GetComponent<SpriteRenderer>().DOColor(new Color(0.2f,0.2f,0.2f,1), 0.3f))
                .Join(this.transform.DOScale(new Vector3(0.65f, 0.65f, 0), 0.3f))
                .Append(this.transform.DOScale(new Vector3(0.5f, 0.5f, 0), 1f))
                .Play();
        }

        if(collision.tag == "GoalBell")
        {
            isArrived = true; //ステージセレクト用
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "FastPanel")
        {
            moveForce = fastForce;
        }

        if(collision.tag == "SlowPanel")
        {
            moveForce = slowForce;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "FastPanel" || collision.tag == "SlowPanel")
        {
            moveForce = 0.5f;
        }
    }
}
