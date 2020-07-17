using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectHeartController : MonoBehaviour
{
    [SerializeField] private GameObject mainHeart = default;
    [SerializeField] private float sidePos = default;
    private MainHeartManager mainHeartManager = default;

    private void Start()
    {
        mainHeartManager = mainHeart.GetComponent<MainHeartManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainHeartManager.GetIsArrived)
        {
            this.transform.position = new Vector3(mainHeart.transform.position.x + sidePos, 0, 0);
        }
    }
}
