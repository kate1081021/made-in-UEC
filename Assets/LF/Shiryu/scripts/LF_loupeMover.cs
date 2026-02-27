using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LoupeFire
{
    public class LF_loupeMover : MiniGameBase
    {
        [SerializeField] int howToMove;
        [SerializeField] int Moving_Vertically; //上下のどちらに動いているか，または動いていないか．1:上，-1:下，0:動いていない
        [SerializeField] int Moving_Horizontally; //左右のどちらに動いているか，または動いていないか．
        [SerializeField] int Moving_Roundly; //円運動しているか．
        [SerializeField] float speed; //虫眼鏡の速さ
        float circle_radius = 2.0f;
        Vector3 lastpos;
        int lastdir;
        float initialTime;  //円運動用
        float lasttime = 0f; //円運動用
        float initialDeg; //円運動用
        public bool flag = false;  //trueになったら成功失敗を判定する
        public override void OnGameStart()
        {
            MGManager.Load();
            //MGManager.TestPlay(100);  //テストプレイ用なので，後で消す．
            howToMove = UnityEngine.Random.Range(0, 3);
            //howToMove = 0;
            speed = 0.05f * Time.timeScale;
            lastdir = 1;
            switch (howToMove)
            {
                case 0:
                    lastpos = new Vector3(0, UnityEngine.Random.Range(-1f, 1f), 0);
                break;
                case 1:
                    lastpos = new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), 0.6f, 0);
                break;
                case 2:
                    initialDeg = UnityEngine.Random.Range(0, 360);
                break;
            }
            init();
        }
        void Update()
        {
            switch (howToMove)
            {
                case 0:
                    move_vertically();
                break;
                case 1:
                    move_horizontally();
                break;
                case 2:
                    move_roundly();
                break;
            }
            if (!flag && Moving_Vertically == 0 && Moving_Horizontally == 0 && Moving_Roundly == 0)
            {
                init();
            }
        }
        void move_vertically() //縦方向
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
            if (Action.WasPerformedThisFrame() && !flag)
            {
                lastdir = Moving_Vertically;
                Moving_Vertically = 0;
                lastpos = transform.position;
                flag = true;
                //MGManager.ClearGame();
            }
            return;
        }
        void move_horizontally() //横方向
        {
            if (Moving_Horizontally != 0) //虫眼鏡を往復させる処理．
            {
                transform.Translate(2 * speed * Moving_Horizontally, 0, 0);
            }
            if (Moving_Horizontally == 1 && transform.position.x > 6f)
            {
                Moving_Horizontally = -1;
            } else if(Moving_Horizontally == -1 && transform.position.x < -6f)
            {
                Moving_Horizontally = 1;
            }
            if (Action.WasPerformedThisFrame() && !flag)
            {
                lastdir = Moving_Horizontally;
                Moving_Horizontally = 0;
                lastpos = transform.position;
                flag = true;
                //MGManager.ClearGame();
            }
            return;
        }
        void move_roundly()  //円運動
        {
            if (Moving_Roundly == 1)
            {
                Vector2 pos = transform.position;
                float rad = speed * Mathf.Rad2Deg * (Time.time - initialTime + lasttime) / circle_radius + Mathf.Rad2Deg * initialDeg ;
                pos.x = Mathf.Cos(rad) * circle_radius;
                pos.y = 2.7f + Mathf.Sin(rad) * circle_radius;
                transform.position = pos;
            }
            if (Action.WasPerformedThisFrame() && !flag)
            {
                lasttime = Time.time - initialTime + lasttime;
                Moving_Roundly = 0;
                flag = true;
                //MGManager.ClearGame();
            }
            return;

        }
        void init()
        {
            switch (howToMove)
            {
                case 0:
                    Moving_Vertically = lastdir;
                    transform.position = lastpos;
                break;
                case 1:
                    Moving_Horizontally = lastdir;
                    transform.position = lastpos;
                break;
                case 2:
                    Moving_Roundly = 1;
                    initialTime  = Time.time;
                    //transform.position = new Vector3(0, 0.6f, 0);
                    speed = 0.1f * Time.timeScale;
                break;
            }
        }
    }
}