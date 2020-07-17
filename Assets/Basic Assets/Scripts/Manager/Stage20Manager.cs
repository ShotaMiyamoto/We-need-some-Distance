using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KanKikuchi.AudioManager;
using UnityEngine.Playables;
using UB.Simple2dWeatherEffects.Standard;
using Cinemachine;

public class Stage20Manager : MonoBehaviour
{
    //＝＝＝＝＝＝＝＝＝＝＝＝＝基本＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private GameObject buddyHeart = default;

    private Stage20HeartManager stage20HeartManager = default;

    [SerializeField] private GameObject mainHeart = default;


    //＝＝＝＝＝＝＝＝＝＝＝＝＝オブジェクト生成処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    [SerializeField] private GameObject[] obstacles = default;

    [SerializeField] private Color specialReadyColor = default;

    [SerializeField] private PlayableDirector playableDirector = default;

    [SerializeField] private AudioSource timelineAud = default;

    private bool isPlaying = true;

    private bool isStarted = false;

    [SerializeField] private GameObject enemyArea = default;
    [SerializeField] private GameObject panelArea = default;

    //＝＝＝＝＝＝＝＝＝＝＝＝＝霧処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] private D2FogsNoiseTexPE fogNoise = default;
    private bool isInFogArea = false;

    //＝＝＝＝＝＝＝＝＝＝＝＝＝チェックポイント処理＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private GameDataStorageManager gdsm = default;
    [SerializeField] private float[] checkPointTime = default;

    //＝＝＝＝＝＝＝＝＝＝＝＝＝＝カメラ制御＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    public CinemachineVirtualCamera secondCamera = default; //後半のカメラ


    private void Awake()
    {
        buddyHeart.GetComponent<SubHeartManager>().SetReadyColor = specialReadyColor;
    }

    private void Start()
    {
        stage20HeartManager = buddyHeart.GetComponent<Stage20HeartManager>();

        gdsm = GameDataStorageManager.Instance;

        timelineAud.volume = 0;

        if(gdsm.CurrentCheckPoint > 2)
        {
            secondCamera.Priority = 11;
        }
    }

    private void Update()
    {
        if(!isStarted && Input.GetMouseButtonDown(0))
        {
            isStarted = true;
            FogDebsityControll(0.5f, 6f);
            StartTimeline();
        }
    }

    public void StartTimeline()
    {

        switch (gdsm.CurrentCheckPoint)
        {
            case 0:
                //Debug.Log("はじめから");
                DOTween.To(() => timelineAud.volume, (x) => timelineAud.volume = x, 1f, 1f).SetEase(Ease.Linear);
                playableDirector.Play();
                break;

            case 1:
                //Debug.Log("第一チェックポイントから");
                DOTween.To(() => timelineAud.volume, (x) => timelineAud.volume = x, 1f, 1f).SetEase(Ease.Linear);
                playableDirector.time = checkPointTime[0];
                playableDirector.Play();
                break;

            case 2:
                //Debug.Log("第二チェックポイントから");
                DOTween.To(() => timelineAud.volume, (x) => timelineAud.volume = x, 1f, 1f).SetEase(Ease.Linear);
                playableDirector.time = checkPointTime[1];
                playableDirector.Play();
                break;

            case 3:
                //Debug.Log("第三チェックポイントから");
                DOTween.To(() => timelineAud.volume, (x) => timelineAud.volume = x, 1f, 1f).SetEase(Ease.Linear);
                playableDirector.time = checkPointTime[2];
                playableDirector.Play();
                break;

            case 4:
                //Debug.Log("第四チェックポイントから");
                DOTween.To(() => timelineAud.volume, (x) => timelineAud.volume = x, 1f, 1f).SetEase(Ease.Linear);
                playableDirector.time = checkPointTime[3];
                playableDirector.Play();
                break;

            default:
                //Debug.Log("そんな数値無い");
                break;
        }
    }

    private void FogDebsityControll(float target,float duration)
    {
        DOTween.To(() => fogNoise.Density, (x) => fogNoise.Density = x, target, duration).SetEase(Ease.InOutSine);
    }

    public void PDirPauser()
    {
        if (isPlaying)
        {
            playableDirector.Pause();
            isPlaying = false;
        }
        else
        {
            playableDirector.Resume();
            isPlaying = true;
        }
    }

    public void SetActiveFogArea()
    {
        if (!isInFogArea)
        {
            FogDebsityControll(1.3f, 15f);
            isInFogArea = true;
        }
        else
        {
            FogDebsityControll(0.5f, 3f);
            isInFogArea = false;
        }
    }

    public void GenerateObs()
    {
       
        int i = Random.Range(0, 2);

        switch (i)
        {
            case 0:
                GenerateFastPanelObs();
                GenerateSndwitchObs();
                //Debug.Log("FastPanel生成");
                break;

            case 1:
                GenerateSlowPanelObs();
                GenerateSndwitchObs();
                //Debug.Log("SlowPanel生成");
                break;

            default:
                //Debug.Log("そんな数値ない");
                break;
        }
    }

    public void GenerateSndwitchObs()
    {
        GameObject obj = Instantiate(obstacles[0], mainHeart.transform.position, Quaternion.Euler(0, 0, GetAngle(mainHeart.transform.position, buddyHeart.transform.position)));
        ObjectFader(obj, 0.5f);
    }

    public void GenerateGatherObs()
    {
        stage20HeartManager.AppearAtAround();
        Instantiate(obstacles[1], buddyHeart.transform.position, buddyHeart.transform.rotation);
    }

    public void GenerateFastPanelObs()
    {
        stage20HeartManager.AppearAtAbove();
        var pos = (buddyHeart.transform.position + mainHeart.transform.position) * 0.5f;
        var angle = GetAngle(mainHeart.transform.position, buddyHeart.transform.position);
        GameObject obj = Instantiate(obstacles[2], pos, Quaternion.Euler(0,0,angle));
        ObjectFader(obj, 0.5f);
    }

    public void GenerateSlowPanelObs()
    {
        stage20HeartManager.AppearAtAbove();
        var pos = (buddyHeart.transform.position + mainHeart.transform.position) * 0.5f;
        var angle = GetAngle(mainHeart.transform.position, buddyHeart.transform.position);
        GameObject obj = Instantiate(obstacles[3], pos, Quaternion.Euler(0, 0, angle));
        ObjectFader(obj, 0.5f);
    }

    public void GenerateEnemyArea()
    {
        stage20HeartManager.GoingUp(26f,35f);
        enemyArea.SetActive(true);
        enemyArea.transform.position = buddyHeart.transform.position + new Vector3(0, 8,0);
    }

    public void GeneratePanelArea()
    {
        stage20HeartManager.GoingUp(32f,41f);
        panelArea.SetActive(true);
        panelArea.transform.position = buddyHeart.transform.position + new Vector3(0, 8, 0);
    }


    public float GetAngle(Vector2 from, Vector2 to)
    {
        Vector2 dest = to - from; //ベクトル取得
        float radian = Mathf.Atan2(dest.x, dest.y); //ラジアンを求める
        float degree = radian * Mathf.Rad2Deg; //ラジアンを度に変換する

        if(degree < 0)　//角度の範囲を0-360になるよう修正
        {
            degree += 360;
        }

        degree = 360f - degree;

        return degree;
    }

    public void PassedCheckPoint()
    {
        gdsm.CurrentCheckPoint += 1;
        //Debug.Log("チェックポイント" + gdsm.CurrentCheckPoint + "通過");
    }

    private void ObjectFader(GameObject obj,float fadeTime)
    {
        List<SpriteRenderer> sprites = new List<SpriteRenderer>();

        if (obj.GetComponent<SpriteRenderer>())
        {
            sprites.Add(obj.GetComponent<SpriteRenderer>());
        }

        SpriteRenderer[] childSprites;

        if (obj.GetComponentsInChildren<SpriteRenderer>() != null)
        {
            childSprites = obj.GetComponentsInChildren<SpriteRenderer>();
        
            foreach (var val in childSprites)
            {
                sprites.Add(val);
            }
        }

        if (sprites.Count > 0)
        {
            foreach (var val in sprites)
            {
                val.DOColor(new Color(val.color.r, val.color.g, val.color.b, 1), fadeTime);
            }
        }
    }

    public void ClearStage()
    {
        FogDebsityControll(2f, 3f);
        GameManager.Instance.IsCreared = true;

        var seq = DOTween.Sequence()
            .AppendInterval(1.5f)
            .AppendCallback(() => SEManager.Instance.Play(SEPath.GOAL_BELL, 0.8f))
            .Play();
    }

}
