using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
/// <summary>
/// マネキンの処理
/// </summary>
public class MannequinEnamy : MonoBehaviour
{
    [Header("プレイヤー"), SerializeField]
    private GameObject target;
    [Header("ナブメッシュ"), SerializeField]
    private NavMeshAgent agent;
    [Header("動き出すまでの待機時間")]
    public float moveDelay = 2.0f;
    [Header("画面内にいるか")]
    public bool isInScreen = false;

    [Header("きしむ音")]
    public AudioSource audioSource;
    [Header("プレイヤを捕まえる位置"),SerializeField]
    private Transform playergrab;

    private Animator animator;

    //最後に見てた時刻
    private float lastLookedTime;
    //動いてるかどうか
    private bool Moving;
    //プレイヤー側でセンサーに移っているか
    public bool isLookedAT;
    // 音を一度だけ鳴らすためのフラグ
    private bool hasPlayedPreSound = false;
    //捕まったかどうか
    [SerializeField]private bool grab=false;
    //プレイやーが死んだどうか
    public bool playDie = false;
    //今ストップ状態か
    private bool isStop=false;

    private void Start()
    {
        //プレイヤーがないならタグで取る
        if (target==null)
        {
            target= GameObject.FindGameObjectWithTag("Player");
        }

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        //最初のポーズはランダム
        float randomStartPoint = Random.Range(0f, 5f);

        animator.Play("Move", 0, randomStartPoint);

        // 最初は止まっていてほしいので、スピードを0にする
        animator.speed = 0f;
    }


    /// <summary>
    /// マネキンの動きの処理
    /// </summary>
    private void Update()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            if (target == null) return;
        }
        if (grab) return;

        if (isLookedAT)
        {
            lastLookedTime = Time.time;
            hasPlayedPreSound = false;
        }
        float timeSinceLooked = Time.time - lastLookedTime;
        bool stopCondition = !isInScreen || isLookedAT || timeSinceLooked < moveDelay;

        //「今見られている」か「目を離してから2秒以内」か「カメラに移ってない」なら停止
        if (stopCondition)
        {
            isStop= true;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            //アニメーターの動きを停止
            animator.speed = 0f;

            // 見ていない時だけ実行
            if (isInScreen && !isLookedAT) 
            {
                // 残り時間を計算
                float timeLeft = moveDelay - timeSinceLooked;

                //動き出す0.5秒前に音を鳴らす
                if (timeLeft <= 0.5f && !hasPlayedPreSound)
                {
                    if (audioSource != null) audioSource.Play();
                    hasPlayedPreSound = true;
                }
            }
        }
        //sensorから外れたら追跡開始
        else
        {
            isStop= false;
            agent.isStopped = false;
            agent.destination = target.transform.position;

            //アニメーション開始
            animator.speed = 1f;
        }
        // 毎フレームリセット
        isLookedAT = false;
    }

    /// <summary>
    /// ヒットしたのがPlayerなら捕まえる
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerGrab();
        }
    }

    /// <summary>
    /// プレイヤを捕まえる処理
    /// </summary>
    /// <summary>
    /// プレイヤを捕まえる処理（修正版）
    /// </summary>
    private void PlayerGrab()
    {
        if (grab) return;

        grab = true;
        playDie = true;
        GameManager.instance.GameOver();

        // 1. エージェントを完全に止める
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        animator.speed = 1f;
        animator.Play("Grab", 0, 0f);

        // 2. プレイヤー側のスクリプト・物理・衝突判定を全てオフにする
        // ★ PlayerControllerを無効化しないとUpdate内で位置が上書きされて浮く
        PlayerController playerScript = target.GetComponent<PlayerController>();
        if (playerScript != null)
        {
            playerScript.enabled = false;
        }

        // ★ プレイヤーのAnimatorも無効化（アニメーションが位置を動かすのを防ぐ）
        Animator playerAnim = target.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.enabled = false;
        }

        CharacterController playerController = target.GetComponent<CharacterController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        Rigidbody playerRb = target.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.isKinematic = true;
            playerRb.detectCollisions = false;
        }

        // 3. 親子付け
        target.transform.SetParent(playergrab);

        // ローカル位置をゼロにして、playergrabの位置にぴったり合わせる
        // ※ もしプレイヤーのピボットが足元にある場合は、Yを少し下げて調整してください
        //   例: new Vector3(0, -0.5f, 0) など
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;

        Debug.Log("マネキン：プレイヤーを捕獲しました");
    }

    // カメラに映った時に呼ばれる
    private void OnBecameVisible()
    {
        isInScreen = true;
    }

    // カメラから外れた時に呼ばれる
    private void OnBecameInvisible()
    {
        isInScreen = false;
    }
}
