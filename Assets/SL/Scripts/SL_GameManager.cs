using UnityEngine;

namespace SL
{
    public class SL_GameManager : MiniGameBase
    {
        [Header("クリアチェッカー変数")]
        bool Cleared = true;

        [Header("パラメータ")]
        public float balloonDamage = 0f;
        public float balloonDamageMax = 30f;
        public float balloonReducingSpeed = 0.05f;

        [Header("オブジェクト")]
        public SL_BalloonController balloonController;
        public SL_EachHandController rightHandController;
        public SL_EachHandController leftHandController;
        public SL_AudioManager sL_AudioManager;
        public SL_ImageController sL_ImageController;

        public override void OnGameStart()
        {
            MGManager.Load();
            //BGMPlay(); 運営の方針の関数です。こちらにする場合、この下のやつは消しちゃってください。
            sL_AudioManager.SL_BGMStart();
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
            //クリアしていないときのみSEを鳴らしています
            if (Cleared)
            {
            sL_AudioManager.AttackSe();    
            }
            if (balloonDamage >= balloonDamageMax)
            {
                if (Cleared)
                {
                //OngameClearが何度も呼ばれてしまうことをついでに防いでいます
                OnGameClear();
                sL_AudioManager.GameClearSe();
                Cleared = false;
                }
            }
        }

        private void OnGameClear()
        {
            MGManager.ClearGame();
            sL_AudioManager.SL_BGMStop();
            balloonController.BreakBaloon();
            rightHandController.HideHand();
            leftHandController.HideHand();
            sL_ImageController.changeDogImage();
            sL_ImageController.changeBackGround();
            //表情変化
            //背景処理
        }
    }
}