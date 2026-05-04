using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
    [System.Serializable]
    public class SpawnSettings
    {
        [Header("名前"), SerializeField]
        public string label;

        [Header("prefab"), SerializeField]
        public GameObject prefab;

        [Header("出現場所"), SerializeField]
        public Transform[] spawnPoints;

        [Header("開始にスポーンするか")]
        public bool isAutoSpawn = false;

        [Header("Playerスポーンか（自動セットアップする）")]
        public bool isPlayer = false;
    }

    [Header("スポーン設定リスト"), SerializeField]
    public List<SpawnSettings> spawnList;

    private void Start()
    {
        SpawnAllGroups();
    }

    /// <summary>
    /// ゲーム開始時に呼び出すものの処理
    /// </summary>
    public void SpawnAllGroups()
    {
        foreach (var group in spawnList)
        {
            if (group.prefab == null) continue;

            // Playerの場合は、GameManagerが記憶している復活地点とラベルが一致するものだけを出す
            if (group.isPlayer)
            {
                if (group.label != GameManager.savedRestartPosition)
                {
                    continue;
                }
            }
            else
            {
                // GameManagerに「今のステージの敵」が記録されていて、それと名前が一致すればスポーンさせる
                if (!string.IsNullOrEmpty(GameManager.activeEnemyLabel) && group.label == GameManager.activeEnemyLabel)
                {
                    // スポーンさせるのでそのまま下へ進む
                }
                // それ以外は今まで通り isAutoSpawn で判定
                else if (!group.isAutoSpawn) 
                {
                    continue;
                }
            }

            foreach (Transform p in group.spawnPoints)
            {
                if (p != null)
                {
                    GameObject spawned = Instantiate(group.prefab, p.position, p.rotation);

                    // Playerスポーンなら自動セットアップ
                    if (group.isPlayer)
                    {
                        PlayerSetup.Setup(spawned);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ゲームを起動してる際に途中から呼び込む処理
    /// </summary>
    /// <param name="label"></param>
    public void SpawnByLabel(string label)
    {
        // リストの中からlabelが一致する設定を探す
        SpawnSettings settings = spawnList.Find(s => s.label == label);

        if (settings != null && settings.prefab != null)
        {
            foreach (Transform p in settings.spawnPoints)
            {
                if (p != null)
                {
                    GameObject spawned = Instantiate(settings.prefab, p.position, p.rotation);

                    // Playerスポーンなら自動セットアップ
                    if (settings.isPlayer)
                    {
                        PlayerSetup.Setup(spawned);
                    }
                }
            }
        }
    }
}
