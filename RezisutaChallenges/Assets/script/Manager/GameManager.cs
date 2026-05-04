using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("GameOverになる際のパネル")]
    [SerializeField] private Image gameOverPanel;
    [Header("プレイヤー死亡SE")]
    [SerializeField] private AudioClip playerDieSE;

    [SerializeField] private AudioSource audioSource;

    //リスタートする名前（場所がステージによって変わるため）
    public string restartPositionName= "TutorialStage";

    // シーンリロード後も記憶するための静的変数
    public static string savedRestartPosition = "";
    public static string activeCameraTag = "";
    public static string activeEnemyLabel = "";

    //プレイヤーが死んだかどうか
    public bool PlayerDie=false;

    private void Start()
    {
        // シーンロード時に、記憶しているカメラがあれば優先度を上げてメインカメラにする
        if (!string.IsNullOrEmpty(activeCameraTag))
        {
            GameObject targetCamObj = GameObject.FindGameObjectWithTag(activeCameraTag);
            if (targetCamObj != null)
            {
                CinemachineVirtualCamera vcam = targetCamObj.GetComponent<CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    vcam.Priority = 20;
                }
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (string.IsNullOrEmpty(savedRestartPosition))
            {
                // 初回のみInspectorの初期設定を入れる
                savedRestartPosition = restartPositionName;
            }
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
        if (gameOverPanel != null) gameOverPanel.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        string currentSceneName = SceneManager.GetActiveScene().name;

        // シーンをロード
        SceneManager.LoadScene(currentSceneName);

        // もしGameManagerが消えない設定なら、ここでフラグをリセット
        PlayerDie = false;
    }

}
