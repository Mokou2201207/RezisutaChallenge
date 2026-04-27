using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 箱などに登る処理
/// </summary>
public class ClimbController : MonoBehaviour
{
    [Header("前方への判定距離"), SerializeField]
    private float rayDistance = 0.7f;
    [Header("ビームを出す高さ"), SerializeField]
    public float rayHeight = 0.6f;
    [Header("判定したいレイヤー"), SerializeField]
    public LayerMask climbLayer;

    public bool isHit = false;

    void Update()
    {
        //判定用のビームを出す
        Vector3 origin = transform.position + Vector3.up * rayHeight;
        RaycastHit hit;
        Debug.DrawRay(origin, transform.forward * rayDistance, Color.red);

        if (Physics.Raycast(origin, transform.forward, out hit, rayDistance, climbLayer))
        {
            if (isHit) return;
            isHit = true;
        }
        else
        {
            if (!isHit) return;
            isHit = false;
        }
    }
}
