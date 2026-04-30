using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// アイテムの点滅ハイライトの処理
/// </summary>
public class ItemHighlighter : MonoBehaviour
{
    private Outline outline;

    [Header("点滅スピード")]
    public float speed = 2.0f;

    [Header("最大時の太さ")]
    public float maxWidth = 5.0f;

    void Start()
    {
        outline = GetComponent<Outline>();
    }

    void Update()
    {
        if (outline == null) return;

        float alpha = Mathf.PingPong(Time.time * speed, 1.0f);

        outline.OutlineWidth = alpha * maxWidth;
    }
}

