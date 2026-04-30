using System.Collections;
using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    [Header("手")]
    [SerializeField] private Transform hand;
    [Header("範囲")]
    [SerializeField] private float range = 1.5f;
    // 保持アイテム
    private GameObject item; 
    // アイテムを持っているか
    public bool isHaveItem = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (item != null) Drop(); else Pick();
        }
    }

    /// <summary>
    /// アイテム拾う処理
    /// </summary>
    void Pick()
    {
        isHaveItem = true;
        foreach (var col in Physics.OverlapSphere(transform.position, range))
        {
            if (col.CompareTag("Item"))
            {
                item = col.gameObject;
                item.transform.SetParent(hand);
                item.transform.localPosition = Vector3.zero;
                // 物理を止める
                if (item.TryGetComponent(out Rigidbody rb))
                {
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                // 持っている間はコライダーをトリガーにして物理干渉を防ぐ
                if (item.TryGetComponent(out Collider itemCol)) itemCol.isTrigger = true;

                if (TryGetComponent(out Animator anim)) anim.SetBool("isCarrying", true);

                break;
            }
        }
    }

    /// <summary>
    /// アイテムを離す処理
    /// </summary>
    void Drop()
    {
        isHaveItem = false;
        item.transform.SetParent(null);
        // 手の回転が蓄積しないようにリセット
        item.transform.rotation = Quaternion.identity;

        // --- ドロップ位置を安全に計算 ---
        Vector3 dropPos = transform.position + Vector3.up * 1.0f + transform.forward * 1.0f;

        // レイキャストで地面を見つける
        RaycastHit hit;
        if (Physics.Raycast(dropPos, Vector3.down, out hit, 5f, ~0, QueryTriggerInteraction.Ignore))
        {
            float itemHalfHeight = 0.3f;
            Collider itemCol = item.GetComponent<Collider>();
            if (itemCol != null)
            {
                itemHalfHeight = itemCol.bounds.extents.y;
            }
            item.transform.position = hit.point + Vector3.up * (itemHalfHeight + 0.05f);
        }
        else
        {
            item.transform.position = dropPos;
        }

        // コライダーを戻す
        if (item.TryGetComponent(out Collider col)) col.isTrigger = false;

        // プレイヤーとアイテムの衝突を一時的に無視する
        IgnorePlayerCollision(item, true);

        if (item.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (TryGetComponent(out Animator anim)) anim.SetBool("isCarrying", false);

        // 0.5秒後に衝突を戻す（アイテムがプレイヤーから離れた後）
        StartCoroutine(ReEnableCollision(item));

        item = null;
    }

    /// <summary>
    /// プレイヤーの全コライダーとアイテムの衝突を無視/復帰する
    /// </summary>
    private void IgnorePlayerCollision(GameObject targetItem, bool ignore)
    {
        if (targetItem == null) return;

        Collider itemCol = targetItem.GetComponent<Collider>();
        if (itemCol == null) return;

        // プレイヤーの全てのコライダーとの衝突を無視する
        foreach (Collider playerCol in GetComponents<Collider>())
        {
            Physics.IgnoreCollision(itemCol, playerCol, ignore);
        }

        // CharacterControllerもColliderなので個別に処理
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            Physics.IgnoreCollision(itemCol, cc, ignore);
        }
    }

    /// <summary>
    /// 一定時間後にプレイヤーとの衝突判定を復帰する
    /// </summary>
    private IEnumerator ReEnableCollision(GameObject droppedItem)
    {
        yield return new WaitForSeconds(0.5f);

        if (droppedItem != null)
        {
            IgnorePlayerCollision(droppedItem, false);
        }
    }
}