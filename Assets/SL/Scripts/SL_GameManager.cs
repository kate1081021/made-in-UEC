using UnityEngine;

namespace SL
{
    public class SL_GameManager : MiniGameBase
    {
        [Header("パラメータ")]
        public float balloonDamage = 0f;
        public float balloonDamageMax = 30f;
        public float balloonReducingSpeed = 0.05f;

        [Header("オブジェクト")]
        public SL_BalloonController balloonController;
        public SL_EachHandController rightHandController;
        public SL_EachHandController leftHandController;

        public override void OnGameStart()
        {
            MGManager.Load();
        }

        void Update()
        {
            //手の移動(可能なら同時押しに対応)
            float trigger = Trigger.ReadValue<float>();
            if (trigger > 0)
            {
                rightHandController.UpdateHand(true);
                leftHandController.UpdateHand(false);
            }
            else if (trigger < 0)
            {
                rightHandController.UpdateHand(false);
                leftHandController.UpdateHand(true);
            }
            else
            {
                rightHandController.UpdateHand(false);
                leftHandController.UpdateHand(false);
            }

            //鼻風船のサイズ変更
            balloonController.UpdateScale(balloonDamage, balloonDamageMax);
        }

        void FixedUpdate()
        {
            //鼻風船のサイズ縮小
            balloonDamage -= balloonReducingSpeed;
            if (balloonDamage < 0f)
            {
                balloonDamage = 0f;
            }
        }

        protected override void OnTriggerStarted(float value)
        {
            //鼻風船へのダメージ
            balloonDamage += 1f;
            if (balloonDamage >= balloonDamageMax)
            {
                OnGameClear();
            }
        }

        private void OnGameClear()
        {
            MGManager.ClearGame();
            balloonController.BreakBaloon();
            rightHandController.HideHand();
            leftHandController.HideHand();
            //表情変化
            //背景処理
        }
    }
}