using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTP : MonoBehaviour
{
    private Transform tpTransform;
    private Image darkPanel;
    private Image brightlyPanal;
    public CinemachineVirtualCamera cameraA;
    public CinemachineVirtualCamera cameraB;

    private void Start()
    {
        // TPの行先をTagで自動取得
        if (tpTransform == null)
        {
            GameObject tpObj = GameObject.FindGameObjectWithTag("TPDestination");
            if (tpObj != null) tpTransform = tpObj.transform;
        }

        // 暗くなるパネルをTagで自動取得
        if (darkPanel == null)
        {
            GameObject darkObj = GameObject.FindGameObjectWithTag("DarkPanel");
            if (darkObj != null) darkPanel = darkObj.GetComponent<Image>();
        }

        // 明るくなるパネルをTagで自動取得
        if (brightlyPanal == null)
        {
            GameObject brightObj = GameObject.FindGameObjectWithTag("BrightlyPanel");
            if (brightObj != null) brightlyPanal = brightObj.GetComponent<Image>();
        }

        // カメラをTagで自動取得
        if (cameraA == null)
        {
            GameObject camAObj = GameObject.FindGameObjectWithTag("CameraA");
            if (camAObj != null) cameraA = camAObj.GetComponent<CinemachineVirtualCamera>();
        }

        if (cameraB == null)
        {
            GameObject camBObj = GameObject.FindGameObjectWithTag("CameraB");
            if (camBObj != null) cameraB = camBObj.GetComponent<CinemachineVirtualCamera>();
        }

        //画像を非表示
        darkPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// PlayerがTPする処理
    /// </summary>
    public void PlayerTeleport()
    {
        Debug.Log("[PlayerTP] PlayerTeleportが呼ばれました");
        StartCoroutine(TPCollection());
    }

    /// <summary>
    /// コルーチンで合わせる
    /// </summary>
    /// <returns></returns>
    private IEnumerator TPCollection()
    {
        //画面をゆっくり暗く
        if (darkPanel != null) darkPanel.gameObject.SetActive(true);
        else Debug.LogWarning("[PlayerTP] darkPanelがnullです");

        if (brightlyPanal != null) brightlyPanal.gameObject.SetActive(false);
        yield return new WaitForSeconds(4f);

        //カメラの優先度を変更
        ResetAllPriorities();
        if (cameraB != null) cameraB.Priority = 20;

        //Playerの位置を替える
        if (tpTransform != null)
        {
            // CharacterControllerを一時的に無効にして位置を変更
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            transform.position = tpTransform.position;

            if (cc != null) cc.enabled = true;

            Debug.Log("[PlayerTP] プレイヤーをTPしました: " + tpTransform.position);
        }
        else
        {
            Debug.LogWarning("[PlayerTP] tpTransformがnullでTPできません");
        }

        //敵をスポーン
        SpawnManager.instance.SpawnByLabel("MainStageEnamy");

        yield return new WaitForSeconds(2f);

        //少しずつ明るく
        if (darkPanel != null) darkPanel.gameObject.SetActive(false);
        if (brightlyPanal != null) brightlyPanal.gameObject.SetActive(true);
    }

    /// <summary>
    /// カメラ優先度のリセット処理
    /// </summary>
    private void ResetAllPriorities()
    {
        if (cameraA != null) cameraA.Priority = 10;
        if (cameraB != null) cameraB.Priority = 10;
    }
}
