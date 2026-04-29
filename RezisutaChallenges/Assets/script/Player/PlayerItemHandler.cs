using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    [Header("持ち手")]
    [SerializeField] private Transform hand;
    [Header("届く距離")]
    [SerializeField] private float range = 1.5f;
    //今現在持っているもの
    private GameObject item; 
    //アイテムを持っているか
    public bool isHaveItem=false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (item != null) Drop(); else Pick();
        }
    }

    /// <summary>
    /// アイテムを取る処理
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
                if (item.TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;

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
        item.transform.position = transform.position + transform.forward * 0.8f + Vector3.up * 0.5f;

        if (item.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
        }
        if (item.TryGetComponent(out Collider col)) col.isTrigger = false;

        if (TryGetComponent(out Animator anim)) anim.SetBool("isCarrying", false);

        item = null;
    }
}