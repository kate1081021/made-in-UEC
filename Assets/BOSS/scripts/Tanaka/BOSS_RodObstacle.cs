using UnityEngine;

namespace BOSS
{
    public class BOSS_RodObstacle : MiniGameBase
    {
        private float BOSS_screenBottom;

        public override void OnGameStart()
        {
            BOSS_screenBottom = Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).y;
        }

        void Update()
        {
            // 4. 画面外（下）に行ったら自分を消去
            if (transform.position.y < BOSS_screenBottom - 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}