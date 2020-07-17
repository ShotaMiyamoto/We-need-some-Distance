using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ObjectDestroyer : MonoBehaviour
{
    [SerializeField] private bool useDistanceDestroy = true; 
    private GameObject follower = default;
    [SerializeField] private float deadLine = 25f;

    [SerializeField] private bool useTimeDestroy = false;
    [SerializeField] private bool useFade = true;
    [SerializeField] private float destroyTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        follower = GameObject.FindGameObjectWithTag("MainHeart");

        if (useTimeDestroy)
        {
            StartCoroutine(DelayMethod(destroyTime,() =>
            {
                if (useFade)
                {
                    FadeOutDestory();   
                }
                else
                {
                    Destroy(this.gameObject);
                }
                    
                          
            }));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(useDistanceDestroy && this.transform.position.y + deadLine < follower.transform.position.y)
        {
            Destroy(this.gameObject);
        }
    }

    private void FadeOutDestory()
    {
        var sprites = transform.GetComponentsInChildren<SpriteRenderer>();
        //Debug.Log("コンポーネント数：" + sprites.Length);

        if(sprites != null)
        {
            var seq = DOTween.Sequence();

            foreach(var val in sprites)
            {
                seq.Join(val.DOColor(new Color(val.color.r, val.color.g, val.color.b, 0), 0.5f));
            }

            seq
               .Play()
               .OnComplete(() => Destroy(this.gameObject));
        }

    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
