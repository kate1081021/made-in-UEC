using UnityEngine;

namespace SL
{
    public class SL_EachHandController : MiniGameBase
    {
        [Header("パラメータ")]
        public float moveDistance = 1f;
        private SpriteRenderer spriteRenderer;
        private Vector3 initialPosition;

        public override void OnGameStart()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            initialPosition = transform.localPosition;
        }

        public void UpdateHand(bool isPushing)
        {
            if (isPushing)
            {
                transform.localPosition = initialPosition + new Vector3(moveDistance, 0f, 0f);
            }
            else
            {
                transform.localPosition = initialPosition;
            }
        }

        public void HideHand()
        {
            spriteRenderer.enabled = false;
        }
    }
}
