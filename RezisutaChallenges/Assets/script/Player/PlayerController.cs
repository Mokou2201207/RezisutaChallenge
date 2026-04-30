using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// プレイヤー操作
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("CharacterController(自動)"), SerializeField]
    private CharacterController controller;

    [Header("ClimbController(自動)"), SerializeField]
    private ClimbController climbController;

    [Header("PlayerItemHandler(自動)"), SerializeField]
    private PlayerItemHandler playerItemHandler;

    [Header("Animator(自動)"), SerializeField]
    private Animator anim;

    [Header("歩く速度"), SerializeField]
    private float Walkspeed = 2f;

    [Header("走る速度"), SerializeField]
    private float runspeed = 4f;

    [Header("回転の滑らかさ"), SerializeField]
    private float turnSmoothTime = 0.1f;

    [Header("重力"), SerializeField]
    private float gravity = -9.81f;



    //登り中フラグ
    private bool isClimb = false;

    //保存用スピード
    private float speed;

    //アイテム保存用変数
    private GameObject carriedItem;
    //回転速度の保存変数
    private float turnSmoothVelocity;
    private Vector3 velocity;

    /// <summary>
    /// 開始
    /// </summary>
    private void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        if (climbController == null)
        {
            climbController = GetComponent<ClimbController>();
        }

        if (playerItemHandler == null)
        {
            playerItemHandler = GetComponent<PlayerItemHandler>();
        }

        // --- 重要な初期化 ---
        // Root Motionを無効化（アニメーションがtransformを動かすのを防ぐ）
        if (anim != null)
        {
            anim.applyRootMotion = false;
        }

        // RigidbodyがあればKinematicにする（CharacterControllerと競合するため）
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        //スピード初期化
        speed = Walkspeed;
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
        if (isClimb) return;
        //WASDの入力
        float horisontal = Input.GetAxisRaw("Horizontal");
        float verttical = Input.GetAxisRaw("Vertical");

        //移動方向の入力に与える
        Vector3 direction = new Vector3(verttical, 0f, -horisontal).normalized;

        if (anim != null)

        {

            // 動いているかを判定して変数に入れる

            bool isMoving = direction.magnitude >= 0.05f;



            //移動していてそのスピードが走る速度なら走っていると判定

            bool isRunning = isMoving && speed == runspeed;



            // 歩いているなら、移動している時にのみ

            anim.SetBool("isWalking", isMoving && !isRunning);

            anim.SetBool("isRunning", isMoving && isRunning);

            //anim.SetBool("isRunning", isRunning);

        }

        //shiftを押すと速度を変える
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runspeed : Walkspeed;

        // アイテムを持っているかで最終的な速度を上書き
        if (playerItemHandler.isHaveItem)
        {
            speed = Walkspeed;
        }
        else
        {
            speed = targetSpeed;
        }

        //登る処理
        if (climbController.isHit)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isClimb)
            {
                PlayerClimb();
                return;
            }
        }

        //回転処理
        //移動キーを押しているか
        if (direction.magnitude >= 0.1f)
        {
            // キャラクターを進行方向に向ける
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 移動実行
            controller.Move(direction * speed * Time.deltaTime);
        }

        //重力処理
        if (controller.isGrounded && velocity.y < 0)
        {
            // 地面に接しているときは下方向の力を保つ
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// ブロックへ登る処理
    /// </summary>
    void PlayerClimb()
    {
        if (isClimb) return;
        isClimb = true;

        //物理防止
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        controller.enabled = false;

        //速度リセット
        velocity = Vector3.zero;
        anim.SetTrigger("Climb");
    }

    /// <summary>
    /// アニメーションイベントで呼ばれる
    /// </summary>
    public void PlayerClimbAnimEnd()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // CharacterController使用時は常にKinematic
        }

        // 重要：enabled戻す前にvelocity全てゼロにする
        velocity = Vector3.zero;

        // --- 修正 ---
        Vector3 currentPos = transform.position;
        // ブロックの上に埋まらないように、わずかに（5cm）上げる
        transform.position = new Vector3(currentPos.x, currentPos.y + 0.05f, currentPos.z);

        // 当たり判定を復帰
        controller.enabled = true;

        // 接地フラグをリセット
        velocity.y = 0;

        isClimb = false;
        Debug.Log("Climb End: " + transform.position);
    }
}
