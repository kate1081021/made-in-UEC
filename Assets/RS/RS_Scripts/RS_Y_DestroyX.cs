using UnityEngine;

namespace RS
{
    public class RS_DestroyByInputX : MiniGameBase
    {
        private SpriteRenderer spriteRenderer;
        private bool canInput = false;

        public override void OnGameStart()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            canInput = true;
        }

        void Update()
        {
            if (!canInput || !Input.GetKeyDown(KeyCode.Alpha3)) return;

            if (spriteRenderer != null && spriteRenderer.color.a > 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }
}