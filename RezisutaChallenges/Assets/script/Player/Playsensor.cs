using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// プレイヤーの視角範囲
/// </summary>
public class Playsensor : MonoBehaviour
{
    [Header("sensorの距離"), SerializeField]
    private float range = 5f;
    [Header("sensorの広さ"),SerializeField]
    private float angle = 90f;
    [Header("Rayの数"), SerializeField]
    private int segments = 10;        
    [Header("当たってほしいレイヤー"), SerializeField]
    private LayerMask targetLayer;

    private void Update()
    {
        Scan();
    }

    //sensorの処理
    void Scan()
    {
        //扇の開始角度を計算
        float startAngle = -angle / 2f;

        for(int i=0; i <= segments; i++)
        {
            float currentAngle = startAngle + (angle / segments) * i;

            //正面ベクトルを回転させる
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            RaycastHit hit;
            // Rayの発射位置
            Vector3 origin = transform.position + Vector3.up * 1.0f;

            //特定のレイヤーにヒットしたら
            if (Physics.Raycast(origin, direction, out hit, range, targetLayer))
            {
                // 何かに当たった時：赤い線をヒットした場所まで引く
                Debug.DrawLine(origin, hit.point, Color.red);

                // マネキンのスクリプトを取得してフラグを書き換える
                var enemy = hit.collider.GetComponent<MannequinEnamy>();
                if (enemy != null)
                {
                    enemy.isLookedAT = true;
                }
            }
            else
            {
                // 何も当たっていない時
                Debug.DrawRay(origin, direction * range, Color.green);
            }
        }
    }
}
