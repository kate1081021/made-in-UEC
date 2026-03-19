using System.Collections;
using UnityEngine;

namespace BOSS
{
    public class BOSS_bomb : MonoBehaviour
    {
        [Header("諸数値")]
        [SerializeField] private float generate_x_pos = 10.0f; 
        [SerializeField] private float generate_y_pos = 0.0f; 
        [SerializeField] private float throwing_duration = 0.5f; 

        [Header("着弾座標の上限下限")]
        [SerializeField] private float max_target_x = 5.0f;
        [SerializeField] private float min_target_x = -5.0f;
        [SerializeField] private float max_target_y = 5.0f;
        [SerializeField] private float min_target_y = 2.0f;

        [Header("prefab")]
        public GameObject bomb_prefab; 
        public GameObject taeget_prefab; 
        public GameObject rubble_1_prefab;
        public GameObject rubble_2_prefab;
        public GameObject rubble_3_prefab;
        public GameObject rubble_4_prefab;

        void Start()
        {
            // ★単発型：生成された瞬間に「1回だけ」投げる！
            ThrowBombSingle();
        }

        private void ThrowBombSingle()
        {
            int place = Random.Range(0,2);
            Vector3 spawn_pos = (place == 0) 
                ? new Vector3(generate_x_pos, generate_y_pos, 0.0f) 
                : new Vector3(-generate_x_pos, generate_y_pos, 0.0f);

            GameObject bomb = Instantiate(bomb_prefab, spawn_pos, Quaternion.identity);

            float target_x = Random.Range(min_target_x, max_target_x);
            float target_y = Random.Range(min_target_y, max_target_y);
            Vector3 target_pos = new Vector3(target_x, target_y, 0.0f);

            GameObject target = Instantiate(taeget_prefab, target_pos, Quaternion.identity);

            StartCoroutine(ThrowCoroutine(bomb, target, spawn_pos, target_pos));
        }

        IEnumerator ThrowCoroutine(GameObject bomb, GameObject target, Vector3 start, Vector3 end)
        {
            float elapsed = 0f;

            while (elapsed < throwing_duration)
            {
                if (bomb == null) yield break;

                elapsed += Time.deltaTime;
                float ratio = elapsed / throwing_duration;
                bomb.transform.position = Vector3.Lerp(start, end, ratio);

                yield return null;
            }

            if (bomb != null) bomb.transform.position = end;
        
            Destroy(bomb);
            Destroy(target);
            
            if (rubble_1_prefab != null) Instantiate(rubble_1_prefab, end, Quaternion.identity);
            if (rubble_2_prefab != null) Instantiate(rubble_2_prefab, end, Quaternion.identity);
            if (rubble_3_prefab != null) Instantiate(rubble_3_prefab, end, Quaternion.identity);
            if (rubble_4_prefab != null) Instantiate(rubble_4_prefab, end, Quaternion.identity);

            // ★役目を終えたら、この爆弾発射装置（自分自身）もヒエラルキーから消去して綺麗にする
            Destroy(gameObject);
        }
    }
}