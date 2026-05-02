using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// アイテムの受け皿
/// </summary>
public class ItemSocket : MonoBehaviour
{
    [Header("必要なアイテムの名前")]
    [SerializeField] private string requiredItemName;

    [Header("正解した時に起こすイベント")]
    public UnityEvent OnSuccess;

    //アイテム保存用
    private ItemData currentItem;

    private void Update()
    {
        //エリアにアイテムがありEキーが押されたか
        if (currentItem != null && Input.GetKeyDown(KeyCode.E))
        {
            //名前が合っているかチェック
            if (currentItem.itemName == requiredItemName)
            {
                Debug.Log("ギミックを発動します");

                OnSuccess.Invoke();

                Outline outlineScript = GetComponent<Outline>();
                //このオブジェクトのハイライトを消す
                outlineScript.enabled = false;

                // 使ったアイテムを消す
                Destroy(currentItem.gameObject);
                currentItem = null;
            }
            else
            {
                Debug.Log("アイテムが違います：" + currentItem.itemName);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // エリアに入ったものがアイテムなら記録する
        if (other.TryGetComponent(out ItemData itemData))
        {
            currentItem = itemData;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // エリアから出たら記録を消す
        if (other.TryGetComponent(out ItemData itemData))
        {
            if (currentItem == itemData)
            {
                currentItem = null;
            }
        }
    }
}
