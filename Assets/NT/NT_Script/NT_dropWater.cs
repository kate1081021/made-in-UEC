using UnityEngine;

namespace NT {
    public class NT_dropWater : MiniGameBase
    {
        [Header("NT")]
        [SerializeField] private GameObject dropWater_prefab;
        [SerializeField] private Transform dropWater_spawnPoint;
        [SerializeField] private NT_switch switch_script;
        [SerializeField] private int dropWater_threshold;

        int dropWater_count = 0;

        public override void OnGameStart() {}
        public override void OnGameEnd() {}

        void Update()
        {
            if (switch_script == null) return;
            if (switch_script.count >= dropWater_threshold * (dropWater_count + 1))
            {
                DropWater();
                dropWater_count++;
            }
        }

        private void DropWater()
        {
            // 左右に少しだけランダムにずらす
            float offsetX = Random.Range(-0.05f, 0.05f);
            Vector3 pos = dropWater_spawnPoint.position + new Vector3(offsetX, 0, 0);
            Instantiate(dropWater_prefab, pos, Quaternion.identity);
        }
    }
}