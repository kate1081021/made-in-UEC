using NUnit.Framework;
using UnityEngine;

namespace SC
{

    public class SC_handControl : MiniGameBase 
    {
        public Transform stickTrans;
        public float grabRange = 0.5f;
        public bool isOneShot = true;
        public Sprite catchSprite;
        public GameObject thumb;
        public SC_stickControl stickControl; // 棒のスクリプト

        private SpriteRenderer spriteRenderer;
        private bool isAction = false;

        public override void OnGameStart()
        {
            MGManager.Load();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void OnGameEnd() { }

        void Update() 
        {
            if (isOneShot && isAction) return;

            if (Action.WasPerformedThisFrame())
            {
                Vector2 handPos = stickTrans.InverseTransformPoint(this.transform.position);
                
                if (-grabRange <= handPos.y && handPos.y <= grabRange)
                {
                    if (spriteRenderer != null && catchSprite != null)
                    {
                        if (thumb != null) { Destroy(thumb); }
                        spriteRenderer.sprite = catchSprite;
                    }
                    if (stickControl != null) // 成功した場合に棒を止める
                    {
                        stickControl.StopStick();
                    }
                    MGManager.ClearGame();
                    Debug.Log("catch");
                }
                else { Debug.Log("Not catch"); }

                isAction = true;
            }
        }
    }
}
