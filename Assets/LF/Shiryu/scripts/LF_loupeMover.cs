using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LoupeFire
{
    public class LF_loupeMover : MiniGameBase
    {
        [SerializeField] int Moving_Vertically; //上下のどちらに動いているか，または動いていないか．1:上，-1:下，0:動いていない
        [SerializeField] float speed; //虫眼鏡の速さ
        public bool flag = false;  //trueになったら成功失敗を判定する
        public override void OnGameStart()
        {
            MGManager.Load();
            //MGManager.TestPlay(100);  //テストプレイ用なので，後で消す．
            Moving_Vertically = 1;
            speed = 0.05f * Time.timeScale;
        }
        void Update()
        {
            if (Moving_Vertically != 0) //虫眼鏡を往復させる処理．
            {
                transform.Translate(0, speed * Moving_Vertically, 0);
            }
            if (Moving_Vertically == 1 && transform.position.y > 3f)
            {
                Moving_Vertically = -1;
            } else if(Moving_Vertically == -1 && transform.position.y < -3f)
            {
                Moving_Vertically = 1;
            }
            if (Action.WasPerformedThisFrame())
            {
                Moving_Vertically = 0;
                flag = true;
                //MGManager.ClearGame();
            }
        }
    }
}