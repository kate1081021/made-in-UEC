using UnityEngine;

namespace SL
{
    public class SL_BalloonController : MiniGameBase
    {
        public float maxScale = 3f;
        public float minScale = 1f;
        private SpriteRenderer spriteRenderer;

        public override void OnGameStart()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void UpdateScale(float currentValue, float maxValue)
        {
            float scale = Mathf.Lerp(minScale, maxScale, currentValue / maxValue);
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        public void BreakBaloon()
        {
            spriteRenderer.enabled = false;
        }
    }
}