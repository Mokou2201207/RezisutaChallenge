using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// チュートリアルのドアの処理
/// </summary>
public class TutorialDoor : MonoBehaviour
{
    [Header("コンポーネント（自動）"), SerializeField]
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// ドアが空く処理
    /// </summary>
    public void OpenDoor()
    {
        if (anim!=null)
        {
            anim.SetBool("Open", true);
        }
    }
}
