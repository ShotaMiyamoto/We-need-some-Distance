using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePosController : MonoBehaviour
{
    [SerializeField] private float timeLeft = 0.2f;
    private Vector3 originPos = default;

    private void Start()
    {
        originPos = this.transform.position;
    }

    private void RandomizePosition()
    {
        Vector3 pos = new Vector3(originPos.x + Random.Range(-0.02f, 0.02f), originPos.y + Random.Range(-0.02f, 0.02f), 0);
        this.transform.position = pos;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0.0f)
        {
            RandomizePosition();
            timeLeft = 0.2f;
        }
    }
}
