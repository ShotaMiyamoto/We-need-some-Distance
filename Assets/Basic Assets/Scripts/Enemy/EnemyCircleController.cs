using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircleController : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies = default;
    private int lastActivatedNum = 0;
    private float timeElapsed = 0f;
    [SerializeField] private float updateTime = 1f;

    // Start is called before the first frame update
    private void Update()
    {
        if(lastActivatedNum < enemies.Length)
        timeElapsed += Time.deltaTime;
        if(timeElapsed > updateTime)
        {
            timeElapsed = 0;
            ActiveEnemies();
        }
    }

    private void ActiveEnemies()
    {
        enemies[lastActivatedNum].SetActive(true);
        lastActivatedNum++;
    }

}
