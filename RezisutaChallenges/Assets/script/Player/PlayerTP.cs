using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTP : MonoBehaviour
{
    [Header("目的地の行先"), SerializeField]
    private Transform tpTransform;

    [Header("画面が暗くなるパネル"), SerializeField]
    private Image darkPanel;

    [Header("画面が明るくなるパネル"), SerializeField]
    private Image brightlyPanal;

    [Header("エリア1のカメラ")]
    public CinemachineVirtualCamera cameraA;

    [Header("エリア2のカメラ")]
    public CinemachineVirtualCamera cameraB;

    /// <summary>
    /// PlayerがTPする処理
    /// </summary>
    public void PlayerTeleport()
    {
        StartCoroutine(TPCollection());
    }

    /// <summary>
    /// コルーチンで合わせる
    /// </summary>
    /// <returns></returns>
    private IEnumerator TPCollection()
    {
        //画面をゆっくり暗く
        darkPanel.gameObject.SetActive(true);
        brightlyPanal.gameObject.SetActive(false);
        yield return new WaitForSeconds(4f);

        //カメラの優先度を変更
        ResetAllPriorities();
        cameraB.Priority = 20;

        //Playerの位置を替える
        transform.position = tpTransform.position;

        //敵をスポーン
        SpawnManager.instance.SpawnByLabel("MainStageEnamy");

        yield return new WaitForSeconds(2f);

        //少しずつ明るく
        darkPanel.gameObject.SetActive(false);
        brightlyPanal.gameObject.SetActive(true);
    }

    /// <summary>
    /// カメラ優先度のリセット処理
    /// </summary>
    private void ResetAllPriorities()
    {
        cameraA.Priority = 10;
        cameraB.Priority = 10;
    }
}
