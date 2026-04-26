using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("GameOverになる際のパネル")]
    [SerializeField] private Image gameOverPanel;
    [Header("プレイヤー死亡SE")]
    [SerializeField] private AudioClip playerDieSE;

    [SerializeField] private AudioSource audioSource;


    //プレイヤーが死んだかどうか
    public bool PlayerDie=false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // 重複防止
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ゲームオーバシーン
    /// </summary>
    public void GameOver()
    {
        if (PlayerDie) return;

        PlayerDie = true;

        audioSource.PlayOneShot(playerDieSE);

        StartCoroutine(RestartGame());
    }

    /// <summary>
    /// リスタートへ
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestartGame()
    {
        gameOverPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene("MainScene");
    }

}
