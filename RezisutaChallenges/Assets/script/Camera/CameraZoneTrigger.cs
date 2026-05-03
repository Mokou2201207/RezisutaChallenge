using UnityEngine;
using Cinemachine;

public class CameraZoneTrigger : MonoBehaviour
{
    [Header("エリア1のカメラ")]
    public CinemachineVirtualCamera cameraA;

    [Header("エリア2のカメラ")]
    public CinemachineVirtualCamera cameraB;


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 relativePos = transform.InverseTransformPoint(other.transform.position);

            // 全カメラの優先度を一旦リセット
            ResetAllPriorities();

            // プレイヤーがトリガーのどちら側に抜けたかで判定
            if (relativePos.x > 0)
            {
                // 右側に抜けた場合
                cameraB.Priority = 20;
                Debug.Log("エリア2のカメラを起動");
            }
            else
            {
                // 左側に抜けた場合
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