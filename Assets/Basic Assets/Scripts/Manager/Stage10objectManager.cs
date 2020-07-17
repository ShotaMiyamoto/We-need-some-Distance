using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using KanKikuchi.AudioManager;

public class Stage10objectManager : MonoBehaviour
{
    //＝＝＝＝＝＝＝＝＝＝＝＝＝データ取得＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private GameDataStorageManager gdsm = default;


    //＝＝＝＝＝＝＝＝＝＝＝＝＝最初の出現処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] private GameObject glowHeart = default;
    private bool isAppeared = false;

    private bool isStopped = false;
    [SerializeField] private float stopTime = 253f;

    [SerializeField] private Image stopButtonImage = default;

    //＝＝＝＝＝＝＝＝＝＝＝＝＝BGMの進行度に合わせて障害物を移動させる＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private bool isPlaying = false;

    private bool canCount = false;
    public bool SetCanCount { set { canCount = value; } }

    private float timeElapsed = 0f;

    private int clearObsCount = 0;

    [SerializeField] private List<GameObject> obstacles = default;

    [SerializeField] private List<float> obsAppearTime = default;

    [SerializeField] private List<float> obsAppearTimeLatterHalf = default;

    [SerializeField] private Text bgmTimeText = default;

    [SerializeField] private Color specialReadyColor = default;



    private void Awake()
    {
        glowHeart.GetComponent<SubHeartManager>().enabled = false;
    }

    private void Start()
    {
        gdsm = GameDataStorageManager.Instance;

        if (gdsm.CurrentCheckPoint > 0)
        {
            stopTime = 102f;
            obstacles.RemoveRange(0, 2);
            obsAppearTime.Clear();

            foreach(var val in obsAppearTimeLatterHalf)
            {
                obsAppearTime.Add(val);
            }
        }

        if(gdsm.GetLatestClearedlevelNum >= 10)
        {
            GameManager.Instance.SetNextSceneName = "Stage_11";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canCount)
        {
            timeElapsed += Time.deltaTime;
            bgmTimeText.text = ("Time :" + timeElapsed.ToString("f2"));

            if(!isStopped && timeElapsed > stopTime)
            {
                glowHeart.GetComponent<GlowHeartManager>().StopMove();
            }

            if (obstacles.Count > clearObsCount) 
            {
                CheckObsAppear();
            }
        }


        if (isAppeared) return;

        if (!isAppeared && Input.GetMouseButtonDown(0))
        {
            isAppeared = true;
            isPlaying = true;
            glowHeart.GetComponent<SubHeartManager>().enabled = true;
            glowHeart.GetComponent<SubHeartManager>().SetReadyColor = specialReadyColor;
            ApearStopButton();
        }
    }

    private void CheckObsAppear()
    {
        if(timeElapsed > obsAppearTime[clearObsCount])
        {
            //Debug.Log(timeElapsed + "秒経過：" + obstacles[clearObsCount].name + "生成");
            obstacles[clearObsCount].transform.position = glowHeart.transform.position + new Vector3(0,8f,0);
            obstacles[clearObsCount].SetActive(true);

            clearObsCount++;

            if(clearObsCount == 3)
            {
                gdsm.CurrentCheckPoint = 1;
                //Debug.Log("中間経過");
            }
        }
    }

    private void ApearStopButton()
    {
        var seq = DOTween.Sequence()
                    .Append(stopButtonImage.DOColor(new Color(99f / 255, 99f / 255f, 99f / 255, 1f), 1f))
                    .Play();
    }

    public void SetPlayBGM()
    {
        if (isPlaying)
        {
            isPlaying = false;
            BGMManager.Instance.Pause();
        }
        else
        {
            isPlaying = true;
            BGMManager.Instance.UnPause();
        }
    }

    public void StartPlay()
    {
        if(gdsm.CurrentCheckPoint < 1){
            BGMManager.Instance.Play(BGMPath.SB_LIFEIS_FULL, 1, 0, 1, false);
        }
        else
        {
            BGMManager.Instance.Play(BGMPath.SB_LIFEIS_HALF, 1, 0, 1, false);
        }
    }
}
