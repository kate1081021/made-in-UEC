using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace LoupeFire
{
    public class LF_focusMover : MiniGameBase
    {
        [SerializeField] GameObject parent;
        [SerializeField] LF_loupeMover loupescript;
        [SerializeField] bool flag;
        [SerializeField] int SuccessOrFailure = 0; //成功失敗を記録．まだわからない場合は0，成功は1，失敗は-1
        [SerializeField] GameObject match;
        private Transform matchtransform;
        public override void OnGameStart()
        {
            parent = this.gameObject.transform.parent.gameObject;  //自分の親（ルーペ）を取得
            loupescript = parent.GetComponent<LF_loupeMover>();
            matchtransform = match.GetComponent<Transform>();
            flag = loupescript.flag;
        }
        void Update()
        {
            flag = loupescript.flag;
            if (flag && SuccessOrFailure == 0)
            {
                float abssqrd = Mathf.Pow(transform.position.x - matchtransform.position.x, 2f) + Mathf.Pow(transform.position.y - matchtransform.position.y, 2f); //焦点とマッチまでの距離の平方．もっといい方法あるんですかね……？
                if (abssqrd <= 0.1)  //距離の平方が0.1以下なら成功とする
                {
                    SuccessOrFailure = 1;
                    MGManager.ClearGame();
                    Debug.Log(abssqrd.ToString());
                } else
                {
                    SuccessOrFailure = -1;
                    Debug.Log("失敗！！！！ " + abssqrd.ToString());
                }
            }
        }
    }
}