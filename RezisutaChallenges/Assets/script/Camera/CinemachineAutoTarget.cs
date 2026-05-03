using UnityEngine;
using Cinemachine;
/// <summary>
/// CinemachineVirtualCameraにアタッチして、プレイヤーをTagで自動検索しFollow/LookAtに設定する
/// </summary>
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineAutoTarget : MonoBehaviour
{
    [Header("検索するTag")]
    [SerializeField] private string targetTag = "Player";

    private CinemachineVirtualCamera vcam;
    private bool isSet = false;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        // 既に設定済みならスキップ
        if (isSet) return;

        GameObject target = GameObject.FindGameObjectWithTag(targetTag);
        if (target != null)
        {
            vcam.Follow = target.transform;
            isSet = true;
            Debug.Log($"[CinemachineAutoTarget] {gameObject.name} にプレイヤーをアタッチしました");
        }
    }
}
