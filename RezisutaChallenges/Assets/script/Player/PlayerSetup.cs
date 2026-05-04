using UnityEngine;
using Cinemachine;
/// <summary>
/// プレイヤースポーン時に必要なスクリプトを自動アタッチする
/// SpawnManagerから呼ばれる
/// </summary>
public class PlayerSetup : MonoBehaviour
{
    /// <summary>
    /// プレイヤーオブジェクトに必要なスクリプトを自動でアタッチし、初期設定を行う
    /// </summary>
    public static void Setup(GameObject player)
    {
        // Tagを設定
        player.tag = "Player";

        // 必要なスクリプトを自動アタッチ
        if (player.GetComponent<PlayerController>() == null)
            player.AddComponent<PlayerController>();

        if (player.GetComponent<Playsensor>() == null)
            player.AddComponent<Playsensor>();

        if (player.GetComponent<ClimbController>() == null)
            player.AddComponent<ClimbController>();

        if (player.GetComponent<PlayerItemHandler>() == null)
            player.AddComponent<PlayerItemHandler>();

        if (player.GetComponent<PlayerTP>() == null)
            player.AddComponent<PlayerTP>();

        // シネマシーンのカメラにプレイヤーを自動アタッチ
        SetupCinemachineCameras(player.transform);
    }

    /// <summary>
    /// シーン内の全CinemachineVirtualCameraのFollow/LookAtにプレイヤーを設定する
    /// </summary>
    private static void SetupCinemachineCameras(Transform playerTransform)
    {
        CinemachineVirtualCamera[] allCameras = Object.FindObjectsOfType<CinemachineVirtualCamera>();

        foreach (var cam in allCameras)
        {
            // Follow（追従）をプレイヤーに設定
            if (cam.Follow == null)
            {
                cam.Follow = playerTransform;
            }
        }

        Debug.Log($"Cinemachineカメラ {allCameras.Length}台 にプレイヤーをアタッチしました");
    }
}
