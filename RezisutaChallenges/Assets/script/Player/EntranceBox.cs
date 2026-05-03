using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///　入口へのエリアに入ったかの処理
/// </summary>
public class EntranceBox : MonoBehaviour
{
    //エリアに入ったかどうか
    public bool isInAria=false;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInAria = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInAria = false;
        }
    }
}
