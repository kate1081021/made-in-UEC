using UnityEngine;

namespace EA
{
    public class EA_CarsorView : MiniGameBase
    {   
        // カーソルの座標を取得
        private Transform tf;

        // スタート時に呼ばれる
        public override void OnGameStart()
        {
            tf = GetComponent<Transform>();
        }

        // カーソルを指定された座標まで移動する。(lはboardLength)
        public void CarsorMove(int x, int y, int l)
        {
            tf.position = new Vector3(-6.5f + ((8 - l) / 2 + x), 3.5f - ((8 - l) / 2 + y), -2);
        }
    }
}
