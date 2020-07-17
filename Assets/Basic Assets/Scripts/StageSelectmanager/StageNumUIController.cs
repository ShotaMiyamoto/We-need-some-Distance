using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageNumUIController : MonoBehaviour
{

    [SerializeField] private Canvas canvas = default;
    [SerializeField] private RectTransform parentRect = default;
    [SerializeField] private Transform targetTfm = default;

    private RectTransform myRectTfm = default;

    [SerializeField] private Vector3 offset = default;

    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 pos;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetTfm.position + offset);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, canvas.worldCamera, out pos);

        myRectTfm.localPosition = pos;

    }
}
