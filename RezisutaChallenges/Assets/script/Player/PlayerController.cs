using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// プレイヤー操作
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("自動コンポーネント")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private ClimbController climbController;
    [SerializeField] private PlayerItemHandler playerItemHandler;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerTP playerTPScript;

    [Header("EntranceBoxのscriptをアタッチ"), SerializeField]
    private EntranceBox entranceBoxScript;

    //速度
    [Header("歩く速度"), SerializeField]
    private float Walkspeed = 2f;
    [Header("走る速度"), SerializeField]
    private float runspeed = 4f;

    [Header("回転の滑らかさ"), SerializeField]
    private float turnSmoothTime = 0.1f;

    [Header("重力"), SerializeField]
    private float gravity = -9.81f;

    //登る欄
    [Header("登る時間"), SerializeField]
    private float climbDuration = 0.8f;
    [Header("登り切った後に前方へ移動する距離"), SerializeField]
    private float climbForwardDistance = 0.6f;
    [Header("壁との距離"), SerializeField]
    private float wallOffset = 0.3f;
    [Header("IKで手を壁に吸着させるレイヤー"), SerializeField]
    private LayerMask climbIKLayer;
    [Header("手のIK検出距離"), SerializeField]
    private float handIKRayDistance = 1.0f;

    //登り中フラグ
    private bool isClimb = false;
    //Tpのボタンを押したか
    private bool isTp = false;

    //保存用スピード
    private float speed;

    //アイテム保存用変数
    private GameObject carriedItem;
    //回転速度の保存変数
    private float turnSmoothVelocity;
    private Vector3 velocity;

    //IK用登り中の手のターゲット位置
    private Vector3 leftHandIKTarget;
    private Vector3 rightHandIKTarget;
    private Quaternion leftHandIKRotation;
    private Quaternion rightHandIKRotation;
    private float ikWeight = 0f;

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

        if (playerTPScript == null)
        {
            playerTPScript = GetComponent<PlayerTP>();
        }

        //デバックエラー用
        if (entranceBoxScript == null)
        {
            Debug.LogError("entranceBoxScriptがアタッチされていません");
        }

        // Root Motionを無効化
        if (anim != null)
        {
            anim.applyRootMotion = false;
        }

        // RigidbodyがあればKinematicにする
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // IKレイヤーが未設定ならClimbControllerのレイヤーを使う
        if (climbIKLayer == 0)
        {
            climbIKLayer = climbController.GetComponent<ClimbController>() != null
                ? LayerMask.GetMask("Default")
                : LayerMask.GetMask("Default");
        }

        //スピード初期化
        speed = Walkspeed;
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
        //登ってる最中なら処理しない
        if (isClimb) return;

        //WASDの入力
        float horisontal = Input.GetAxisRaw("Horizontal");
        float verttical = Input.GetAxisRaw("Vertical");

        //移動方向を入力に与える
        Vector3 direction = new Vector3(verttical, 0f, -horisontal).normalized;

        if (anim != null)

        {
            //動いているかを判定して変数に入れる
            bool isMoving = direction.magnitude >= 0.05f;
            //移動していてそのスピードが走る速度なら走っていると判定
            bool isRunning = isMoving && speed == runspeed;

            // 歩いているなら、移動している時にのみ
            anim.SetBool("isWalking", isMoving && !isRunning);
            anim.SetBool("isRunning", isMoving && isRunning);
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

        //登る判定
        if (climbController.isHit)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isClimb)
            {
                PlayerClimb();
                return;
            }
        }

        //入口のフラグがOnになっていたら
        if (entranceBoxScript.isInAria)
        {
            //二度おし対策
            if (isTp) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                isTp = true;
                playerTPScript.PlayerTeleport();
            }
        }

        //回転処理
        //移動キーを押している場合
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
            // 地面に接している時は下方向の力を保つ
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// UnityのIKコールバック
    /// </summary>
    private void OnAnimatorIK(int layerIndex)
    {
        if (anim == null) return;

        if (isClimb && ikWeight > 0f)
        {
            // --- 左手 IK ---
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKRotation);

            // --- 右手 IK ---
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTarget);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKRotation);
        }
        else
        {
            // IKを無効化
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
        }
    }

    /// <summary>
    /// 手のIKターゲットを壁の上面に設定する
    /// </summary>
    private void UpdateHandIKTargets()
    {
        float topY = climbController.topY;
        Vector3 wallNormal = climbController.wallNormal;

        // 壁の上面に手を置く位置を計算
        Vector3 right = Vector3.Cross(Vector3.up, -wallNormal).normalized;

        // 左手は少し左、右手は少し右にずらす
        float handSpread = 0.15f;

        // 壁の表面位置
        Vector3 wallSurface = climbController.wallHitPoint;

        //　壁の上面、少し左
        leftHandIKTarget = new Vector3(
            wallSurface.x - right.x * handSpread,
            topY,
            wallSurface.z - right.z * handSpread
        );

        // 右手ターゲット: 壁の上面、少し右
        rightHandIKTarget = new Vector3(
            wallSurface.x + right.x * handSpread,
            topY,
            wallSurface.z + right.z * handSpread
        );

        //手のひらを下に（壁の上面に置く向き）
        leftHandIKRotation = Quaternion.LookRotation(-wallNormal, Vector3.up);
        rightHandIKRotation = Quaternion.LookRotation(-wallNormal, Vector3.up);
    }

    /// <summary>
    /// ブロックへ登る処理
    /// </summary>
    void PlayerClimb()
    {
        if (isClimb) return;
        isClimb = true;

        // Root Motionは使わない
        if (anim != null)
        {
            anim.applyRootMotion = false;
        }

        // 物理・衝突判定を無効化
        controller.enabled = false;
        velocity = Vector3.zero;

        // 手のIKターゲットを計算
        UpdateHandIKTargets();

        anim.SetTrigger("Climb");

        // スクリプトで位置を補間して移動する
        StartCoroutine(ClimbMoveCoroutine());
    }

    /// <summary>
    /// 登りの移動をスクリプトで制御するコルーチン
    /// </summary>
    private IEnumerator ClimbMoveCoroutine()
    {
        Vector3 startPos = transform.position;

        // ClimbControllerが検出した壁の上面の高さ
        float targetY = climbController.topY;

        // 壁の表面位置と法線を取得
        Vector3 wallPoint = climbController.wallHitPoint;
        Vector3 wallNormal = climbController.wallNormal;

        Vector3 wallPos = wallPoint + wallNormal * wallOffset;
        wallPos.y = startPos.y; // 高さはそのまま

        float phase0Duration = climbDuration * 0.15f;
        float elapsed = 0f;

        while (elapsed < phase0Duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / phase0Duration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(startPos, wallPos, smooth);

            // IKを徐々に有効にする
            ikWeight = smooth;

            yield return null;
        }

        transform.position = wallPos;
        ikWeight = 1f;

        Vector3 topPos = wallPos;
        topPos.y = targetY + 0.05f;

        float phase1Duration = climbDuration * 0.5f;
        elapsed = 0f;

        while (elapsed < phase1Duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / phase1Duration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(wallPos, topPos, smooth);
            yield return null;
        }

        transform.position = topPos;

        Vector3 endPos = topPos + transform.forward * climbForwardDistance;
        endPos.y = targetY;

        float phase2Duration = climbDuration * 0.35f;
        elapsed = 0f;

        while (elapsed < phase2Duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / phase2Duration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(topPos, endPos, smooth);

            ikWeight = 1f - smooth;

            yield return null;
        }

        // 最終位置を確定
        transform.position = endPos;
        ikWeight = 0f;

        // 登り完了処理
        PlayerClimbAnimEnd();
    }

    /// <summary>
    /// 登り完了処理
    /// </summary>
    public void PlayerClimbAnimEnd()
    {
        // 重力や移動の速度を完全にリセット
        velocity = Vector3.zero;

        // IKを完全に無効化
        ikWeight = 0f;

        // 当たり判定を復帰
        controller.enabled = true;

        // 接地判定をリセットして重力を再開させる
        velocity.y = 0;

        isClimb = false;
    }
}
