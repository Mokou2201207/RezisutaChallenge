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

        [Header("開始時にスポーンするか")]
        public bool isAutoSpawn = false;
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
                    Instantiate(group.prefab, p.position, p.rotation);
                }
            }
        }
    }

    public void SpawnByLabel(string label)
    {
        // リストの中から label が一致する設定を探す
        SpawnSettings settings = spawnList.Find(s => s.label == label);

        if (settings != null && settings.prefab != null)
        {
            foreach (Transform p in settings.spawnPoints)
            {
                if (p != null)
                {
                    Instantiate(settings.prefab, p.position, p.rotation);
                }
            }
        }
    }
}
