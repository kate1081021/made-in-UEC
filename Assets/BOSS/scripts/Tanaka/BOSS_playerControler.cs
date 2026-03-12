using Unity.VisualScripting;
using UnityEngine;

namespace BOSS
{
    public class BOSS_PlayerControler : MiniGameBase
    {
        Rigidbody2D BOSS_playerRb;
        [SerializeField]
        public int BOSS_playerLife = 3;
        public int BOSS_playerSpeed = 5;
        private Vector2 BOSS_screenLimit;
        private Vector2 BOSS_playerHalfSize;

        public override void OnGameStart()
        {
            MGManager.Load();
            BOSS_playerRb = GetComponent<Rigidbody2D>();
            BOSS_screenLimit = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
            SpriteRenderer BOSS_sr = GetComponent<SpriteRenderer>();
            if (BOSS_sr != null)
            {
                BOSS_playerHalfSize = BOSS_sr.bounds.extents;
            }
        }

        void Update()
        {
            BOSS_playerRb.linearVelocity = BOSS_playerSpeed * Move.ReadValue<Vector2>();
            BOSS_ClampPosition();
        }

        void BOSS_ClampPosition()
        {
            Vector3 BOSS_pos = transform.position;
            BOSS_pos.x = Mathf.Clamp(BOSS_pos.x, -BOSS_screenLimit.x + BOSS_playerHalfSize.x, BOSS_screenLimit.x - BOSS_playerHalfSize.x);
            BOSS_pos.y = Mathf.Clamp(BOSS_pos.y, -BOSS_screenLimit.y + BOSS_playerHalfSize.y, BOSS_screenLimit.y - BOSS_playerHalfSize.y);

            transform.position = BOSS_pos;
        }
    }
}