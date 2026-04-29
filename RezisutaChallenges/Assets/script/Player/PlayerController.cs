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

    [Header("Animator(自動)"), SerializeField]
    private Animator anim;

    [Header("歩く速度"), SerializeField]
    private float Walkspeed = 2f;

    [Header("走る速度"), SerializeField]
    private float runspeed = 4f;

    [Header("回転の滑らか"), SerializeField]
    private float turnSmoothTime = 0.1f;

    [Header("重力"), SerializeField]
    private float gravity = -9.81f;

    [Header("ClimbController(自動)"), SerializeField]
    private ClimbController climbController;

    //壁を登る際中か
    private bool isClimb = false;
    //保存用スピード
    private float speed;

    //今の回転速度の保存変数
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
        //スピードを代入
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

        //移動の方向を入力に与える
        Vector3 direction = new Vector3(verttical, 0f, -horisontal).normalized;

        if (anim != null)
        {
            // まず今動いているかを判定して変数に入れる
            bool isMoving = direction.magnitude >= 0.05f;

            //移動しているかつ今のスピードが走る速度なら走っていると判定
            bool isRunning = isMoving && speed == runspeed;

            // 走っていない、かつ移動している時だけ歩きにする
            anim.SetBool("isWalking", isMoving && !isRunning);
            anim.SetBool("isRunning", isMoving && isRunning);
            //anim.SetBool("isRunning", isRunning);
        }

        //走る処理
        //動いてるかつ,shiftを押してる場合
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = runspeed;
        }
        //Shiftを離したら
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = Walkspeed;
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
        //移動キーが押されているか
        if (direction.magnitude >= 0.1f)
        {
            // キャラクターを進行方向に向かせる
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 移動を実行
            controller.Move(direction * speed * Time.deltaTime);
        }

        //重力処理
        if (controller.isGrounded && velocity.y < 0)
        {
            // 地面に接しているときは少しだけ下向きに力をかける
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

        //落下防止
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        controller.enabled = false;

        //落下速度をリセット
        velocity = Vector3.zero;
        anim.SetTrigger("Climb");
    }

    /// <summary>
    /// アニメーションイベントで処理
    /// </summary>
    public void PlayerClimbAnimEnd()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // 重要：enabledを戻す前にvelocityを完全にゼロにする
        velocity = Vector3.zero;

        // --- ここを修正 ---
        Vector3 currentPos = transform.position;
        // 0.3f は大きすぎました。Root Motionが効いているなら、ここはもっと小さく。
        // ブロックの上に埋まらないように、わずかに（5cm）浮かせるだけにします。
        transform.position = new Vector3(currentPos.x, currentPos.y + 0.05f, currentPos.z);

        // 当たり判定を復活
        controller.enabled = true;

        // 接地判定を完全にリセット
        velocity.y = 0;

        isClimb = false;
        Debug.Log("Climb End: " + transform.position);
    }
}
