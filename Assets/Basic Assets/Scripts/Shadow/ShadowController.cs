using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShadowController : MonoBehaviour
{
    [SerializeField] private Vector3 shiftLength = new Vector3(0.025f,-0.025f,0);
    private GameObject parent = default;

    private void Awake()
    {
        parent = this.transform.parent.gameObject;
        this.GetComponent<SpriteRenderer>().color = new Color(0,0,0,45/255f);
        this.GetComponent<SpriteRenderer>().sprite = parent.GetComponent<SpriteRenderer>().sprite;
        this.transform.position = shiftLength;

    }

    private void Update()
    {
        this.transform.position = parent.transform.position + shiftLength;
    }
}
