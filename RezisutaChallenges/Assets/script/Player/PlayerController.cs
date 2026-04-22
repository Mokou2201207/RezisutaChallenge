using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// プレイヤー操作
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("CharacterController"), SerializeField]
    public CharacterController controller;

    [Header("Animator"), SerializeField]
    private Animator anim;

    [Header("歩く速度"), SerializeField]
    public float Walkspeed = 2f;

    [Header("走る速度"), SerializeField]
    public float runspeed = 4f;

    [Header("回転の滑らか"), SerializeField]
    public float turnSmoothTime = 0.1f;

    [Header("重力"), SerializeField]
    public float gravity = -9.81f;

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
        //スピードを代入
        speed = Walkspeed;
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
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
            anim.SetBool("isRunning", isMoving&&isRunning);
            //anim.SetBool("isRunning", isRunning);
        }

        //走る処理
        //動いてるかつ,shiftを押してる場合
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed=runspeed;
        }
        //Shiftを離したら
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = Walkspeed;
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
}
