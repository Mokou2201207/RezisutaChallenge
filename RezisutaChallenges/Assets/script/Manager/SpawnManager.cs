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

    public void SpawnAllGroups()
    {
        foreach (var group in spawnList)
        {
            if (!group.isAutoSpawn || group.prefab == null) continue;

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
