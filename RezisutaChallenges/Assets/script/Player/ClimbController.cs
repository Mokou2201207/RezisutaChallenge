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
    public float rayDistance = 0.7f;   // 前方への判定距離
    public float rayHeight = 0.6f;     // ビームを出す高さ（キャラの胸あたり）
    public LayerMask climbLayer;       // Climbableレイヤーを指定
    public string triggerName = "ClimbUp"; // Animatorのトリガー名

    private CharacterController controller;
    private Animator anim;
    private bool isClimbing = false;
    private Vector3 endPosition;       // 登りきった後の位置

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isClimbing) return;

        // 1. 判定用のビームを出す
        Vector3 origin = transform.position + Vector3.up * rayHeight;
        RaycastHit hit;
        Debug.DrawRay(origin, transform.forward * rayDistance, Color.red); // シーンビューで確認用

        if (Physics.Raycast(origin, transform.forward, out hit, rayDistance, climbLayer))
        {
            Debug.Log("箱を検知中！"); // これを追加
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("スペースが押されました！"); // これを追加
                StartClimb(hit.collider);
            }
        }
    }

    void StartClimb(Collider box)
    {
        isClimbing = true;
        controller.enabled = false; // 物理判定を止める

        // ★追加：沈み込みを防ぐために、リジッドボディの物理を強制的に止める
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }

        anim.SetTrigger(triggerName);

        // ★追加：登りきった後の位置を計算（コードは以前と同じ）
        float topY = box.bounds.max.y;
        endPosition = transform.position + transform.forward * 0.8f;
        endPosition.y = topY;

        // ★実験：沈み込みを防ぐために、一瞬だけ今の位置を再設定して強制的に固定する
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // 4. アニメーションの最後にイベントで呼ぶ
    public void OnClimbFinish()
    {
        transform.position = endPosition; // ガバッと移動
        controller.enabled = true;        // 物理判定を戻す
        isClimbing = false;
    }
}
