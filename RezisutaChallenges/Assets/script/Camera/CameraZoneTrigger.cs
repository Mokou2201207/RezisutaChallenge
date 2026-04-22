using UnityEngine;
using Cinemachine;

public class CameraZoneTrigger : MonoBehaviour
{
    [Header("エリア1のカメラ (左/手前)")]
    public CinemachineVirtualCamera cameraA;

    [Header("エリア2のカメラ (右/奥)")]
    public CinemachineVirtualCamera cameraB;


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーとこのトリガーの相対的な位置を計算
            // 今回のプロジェクトは X軸かZ軸で移動しているので、その差を見ます
            Vector3 relativePos = transform.InverseTransformPoint(other.transform.position);

            // 全カメラの優先度を一旦リセット（10にする）
            ResetAllPriorities();

            // プレイヤーがトリガーのどちら側に抜けたかで判定
            // ※方向（xにするかzにするか）は、ステージの向きに合わせて調整してください
            if (relativePos.x > 0)
            {
                // 右（エリア2）側に抜けた場合
                cameraB.Priority = 20;
                Debug.Log("エリア2のカメラを起動");
            }
            else
            {
                // 左（エリア1）側に抜けた場合
                cameraA.Priority = 20;
                Debug.Log("エリア1のカメラを起動");
            }
        }
    }

    private void ResetAllPriorities()
    {
        cameraA.Priority = 10;
        cameraB.Priority = 10;
    }
}