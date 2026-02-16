using UnityEngine;

namespace SK
{
    // ★ファイル名と一致させる（SK_NoteMover.cs推奨）
    public class SK_NoteMover : MonoBehaviour
    {
        public float baseSpeed = 800f;
        
        // ★ここを修正：型を「SK_WWG」に変更
        private SK_WWG gameManager;

        // ★ここを修正：引数の型も「SK_WWG」に変更
        public void Setup(SK_WWG manager)
        {
            gameManager = manager;
        }

        void Update()
        {
            if (gameManager == null) return;

            float currentSpeed = baseSpeed * gameManager.GetSpeedMultiplier();

            transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

            if (transform.localPosition.x < -650)
            {
                Destroy(gameObject);
            }
        }
    }
}