using UnityEngine;
using System.Collections.Generic;

namespace RS
{
    public class RS_Spawn : MiniGameBase
    {
        public GameObject[] prefabs;
        [Range(1, 5)] public int count = 3;

        private List<RS_Y_appear> spawnedScripts = new List<RS_Y_appear>();
        private int currentIndex = 0;
        private bool gameActive = false;
        public RS_SpritesManager spm;

        public override void OnGameStart()
        {
            currentIndex = 0;
            SpawnClones();
            gameActive = true;
        }

        private void SpawnClones()
        {
            spawnedScripts.Clear();
            for (int i = 0; i < count; i++)
            {
                float x = (i * 2.0f) - (count - 1.0f);
                Vector3 spawnPos = new Vector3(x, 0, -2);
                
                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
                GameObject clone = Instantiate(randomPrefab, spawnPos, Quaternion.identity);

                var script = clone.GetComponent<RS_Y_appear>();
                if (script != null)
                {
                    script.Setup();
                    spawnedScripts.Add(script);
                }
            }
        }

        void Update()
        {
            if (!gameActive || currentIndex >= spawnedScripts.Count) return;

            RS_Y_appear currentTarget = spawnedScripts[currentIndex];

            if (currentTarget == null) return;

            if (Input.GetKeyDown(currentTarget.targetKey))
            {
                if (currentTarget.GetCurrentAlpha() > 0.5f)
                {
                    Debug.Log($"正解！左から {currentIndex + 1} 番目を消しました");
                    Destroy(currentTarget.gameObject);
                    currentIndex++; // 次へ

                    if (currentIndex >= count)
                    {
                        Debug.Log("ゲームクリア！");
                        spm.is_clear = true;
                        spm.is_spelled = true;
                        MGManager.ClearGame();
                    }
                }
            }
        }
    }
}