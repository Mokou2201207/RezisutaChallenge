using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 壁などに登る処理
/// </summary>
public class ClimbController : MonoBehaviour
{
    [Header("前方への判定距離"), SerializeField]
    private float rayDistance = 0.7f;
    [Header("レイの高さ"), SerializeField]
    private float rayHeight = 0.6f;
    [Header("登りレイヤー"), SerializeField]
    private LayerMask climbLayer;

    public bool isHit = false;

    /// <summary>
    /// 壁の上面のY座標（登り先の高さ）
    /// </summary>
    public float topY { get; private set; }

    /// <summary>
    /// 壁の表面にヒットした位置（プレイヤーを壁に寄せるために使う）
    /// </summary>
    public Vector3 wallHitPoint { get; private set; }

    /// <summary>
    /// 壁の法線方向（壁の表面の向き）
    /// </summary>
    public Vector3 wallNormal { get; private set; }

    void Update()
    {
        //前方のレイキャスト
        Vector3 origin = transform.position + Vector3.up * rayHeight;
        RaycastHit hit;
        Debug.DrawRay(origin, transform.forward * rayDistance, Color.red);

        if (Physics.Raycast(origin, transform.forward, out hit, rayDistance, climbLayer))
        {
            if (isHit) return;
            isHit = true;

            // 壁の表面の位置と法線を保存
            wallHitPoint = hit.point;
            wallNormal = hit.normal;

            // 壁の上面を検出する
            // 壁のヒットポイントの上方からレイを下に撃って、上面のYを取得する
            Vector3 aboveWall = hit.point + Vector3.up * 5f; // 壁の上から
            RaycastHit topHit;
            if (Physics.Raycast(aboveWall, Vector3.down, out topHit, 10f, climbLayer))
            {
                topY = topHit.point.y;
            }
            else
            {
                // 上面検出できない場合は、コライダーのboundsから推定
                topY = hit.collider.bounds.max.y;
            }
        }
        else
        {
            if (!isHit) return;
            isHit = false;
        }
    }
}
