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
        private bool isFailed = false;

        public override void OnGameStart()
        {
            MGManager.Load();
            BGMPlay();
            SEPlay("fall");
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void OnGameEnd() { }

        void Update() 
        {
            if (isOneShot && isAction) return;

            if (Action.WasPerformedThisFrame() && !isFailed)
            {
                //判定に関わらず手を閉じる
                if (spriteRenderer != null && catchSprite != null)
                {
                    if (thumb != null) { Destroy(thumb); }
                    spriteRenderer.sprite = catchSprite;
                }

                Vector2 handPos = stickTrans.InverseTransformPoint(this.transform.position);
                
                if (-grabRange <= handPos.y && handPos.y <= grabRange)
                {
                    if (stickControl != null) // 成功した場合に棒を止める
                    {
                        stickControl.StopStick();
                    }
                    MGManager.ClearGame();
                    isAction = true;
                    SEPlay("catch");
                    SEPlay("success");
                    Debug.Log("catch");
                }
                else
                {
                    isFailed = true;
                    SEPlay("fail");
                    Debug.Log("Not catch");
                }
            }

            if (isFailed)
            {
                Vector2 handPosF = stickTrans.InverseTransformPoint(this.transform.position);
                
                if (-grabRange <= handPosF.y && handPosF.y <= grabRange)
                {
                    stickControl.BounceStick();
                    isAction = true;
                }
            }
        }
    }
}
