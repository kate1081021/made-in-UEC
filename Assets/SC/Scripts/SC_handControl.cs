using UnityEngine;

namespace SC
{

    public class SC_handControl : MiniGameBase 
    {
        public Transform stickTrans;
        public float grabRange = 0.5f;
        public Sprite catchSprite;

        private SpriteRenderer spriteRenderer;

        public override void OnGameStart()
        {
            MGManager.Load();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void OnGameEnd() { }

        void Update() 
        {
            if (Action.WasPerformedThisFrame())
            {
                Vector2 handPos = stickTrans.InverseTransformPoint(this.transform.position);
                
                if (-grabRange <= handPos.y && handPos.y <= grabRange)
                {
                    if (spriteRenderer != null && catchSprite != null)
                    {
                        spriteRenderer.sprite = catchSprite;
                    }
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = Color.red;
                    }
                    MGManager.ClearGame();
                    Debug.Log("catch");
                }
                else { Debug.Log("Not catch"); }
            }
        }
    }
}
