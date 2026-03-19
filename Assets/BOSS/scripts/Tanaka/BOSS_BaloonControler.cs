using UnityEngine;

namespace BOSS
{
    public class BOSS_BaloonControler : MonoBehaviour
    {
        [Header("はなちょうちんの設定")]
        [SerializeField] public float BOSS_initialScale = 0.1f;
        [SerializeField] public float BOSS_maxScale = 5.0f;
        [SerializeField] public float BOSS_growSpeed = 1.2f;

        [HideInInspector] public bool BOSS_isFlipped = false; // ジェネレータからセットされる

        private float BOSS_currentScale;

        void Start()
        {
            BOSS_currentScale = BOSS_initialScale;
            BOSS_ApplyScale();
        }

        void Update()
        {
            BOSS_currentScale += BOSS_growSpeed * Time.deltaTime;
            BOSS_ApplyScale();

            if (BOSS_currentScale >= BOSS_maxScale)
            {
                Destroy(gameObject);
            }
        }

        private void BOSS_ApplyScale()
        {
            // 反転フラグが立ってたらX軸をマイナスにする
            float BOSS_xScale = BOSS_isFlipped ? -BOSS_currentScale : BOSS_currentScale;
            transform.localScale = new Vector3(BOSS_xScale, BOSS_currentScale, 1f);
        }
    }
}