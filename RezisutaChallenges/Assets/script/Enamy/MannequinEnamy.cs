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
    private bool grab=false;
    //プレイやーが死んだどうか
    public bool playDie = false;

    private void Start()
    {
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
        if (target == null||grab) return;

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
    private void PlayerGrab()
    {
        //二度つかめられないように
        if (grab) return;

        animator.Play("Grab");

        playDie = true;
        grab = true;
        agent.isStopped=true;

        Rigidbody playerRb = target.GetComponent<Rigidbody>();
    if (playerRb != null)
    {
        playerRb.isKinematic = true; // 物理演算を止める
        playerRb.useGravity = false; // 重力を切る
    }

        //プレイヤーが移動をできないように
        CharacterController playerController=target.GetComponent<CharacterController>();
        if (playerController!=null)
        {
            playerController.enabled = false;
        }

        PlayerController pc = target.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = false;
        }

        //敵がプレイヤーの手に
        target.transform.SetParent(playergrab);

        //位置と回転をリセットして正面に
        target.transform.localPosition= new Vector3(0, 0f, 0);
        target.transform.localRotation = Quaternion.identity;
        Debug.Log("プレイヤーを捕獲");
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
