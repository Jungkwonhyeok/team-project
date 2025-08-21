using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFollow : MonoBehaviour
{
    RectTransform rect;


    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    void Update()
    {
        rect.position = Camera.main.WorldToScreenPoint(BossController.instance.player.transform.position);
    }
}
