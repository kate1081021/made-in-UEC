//(-4,3.3)
using UnityEngine;
using UnityEngine.UIElements;

namespace garbage
{
    public class GB_ArrowMover : MiniGameBase
    {
        public int mypos = 0;   //やはりidで場所を管理する
        [SerializeField] Vector2 input;    //現在のスティック入力をここにおいておく
        bool leftflag = false, rightflag = false;    //左右のキーが押されている間はtrueとして，"押された瞬間"を取得できるようにする
        public override void OnGameStart()
        {
            transform.position = new Vector2(-4f, -3.3f);    //初期位置に移動
        }
        void Update()
        {
            input = Move.ReadValue<Vector2>();
            input = convert_stick_to_dir(input);    //現在のスティックの状態をinputに入れる
            //ここから
            if (input.x == -1f && !leftflag)    
            {
                leftflag = true;
                if (mypos > 0)
                {
                    transform.position = new Vector2(-4f + 4 * --mypos, -3.3f);
                }
            }
            if (input.x == 1f && !rightflag)
            {
                rightflag = true;
                if (mypos < 2)
                {
                    transform.position = new Vector2(-4f + 4 * ++mypos, -3.3f);
                }
            }
            if (leftflag && input.x != -1f){leftflag = false;}; if (rightflag && input.x != 1f){rightflag = false;};
            //ここまで矢印を動かす処理
        }
    }
}