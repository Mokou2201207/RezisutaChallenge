using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //プレイヤーが入ってたら追跡する
        if (target)
        {
            agent.destination=target.transform.position;
        }
    }
}
