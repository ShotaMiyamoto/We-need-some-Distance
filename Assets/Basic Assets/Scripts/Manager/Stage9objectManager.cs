using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class Stage9objectManager : MonoBehaviour
{
    private Bloom bloom = default;
    private bool canActivateBloom = false;
    private float fadeTime = 0f;
    [SerializeField] private PostProcessVolume volume = default;

    private float timeElapsed = 0f;

    [SerializeField] private GameObject glowHeart = default;
    [SerializeField] private Vector3 moveDir = default;
    [SerializeField] private float delayTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        bloom = ScriptableObject.CreateInstance<Bloom>();
        bloom.enabled.Override(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(!canActivateBloom && GameManager.Instance.IsCreared)
        {
            canActivateBloom = true;
            DoMoveHeart();
        }

        if (fadeTime < 1f && canActivateBloom)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > 6f)
            {
                fadeTime += Time.deltaTime;
                bloom.intensity.Override(fadeTime);
                volume = PostProcessManager.instance.QuickVolume(this.gameObject.layer, 0, bloom);
            }
        }
    }

    private void DoMoveHeart()
    {
        var seq = DOTween.Sequence()
            .PrependInterval(delayTime)
            .Append(glowHeart.transform.DOMove(moveDir, 3f).SetEase(Ease.InOutSine))
            .Play();
    }
}
