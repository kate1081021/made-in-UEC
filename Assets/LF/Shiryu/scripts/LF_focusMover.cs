using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace LoupeFire
{
    public class LF_focusMover : MiniGameBase
    {
        [SerializeField] GameObject parent;
        [SerializeField] LF_loupeMover loupescript;
        [SerializeField] bool flag;
        [SerializeField] int SuccessOrFailure = 0; //成功失敗を記録．まだわからない場合は0，成功は1，失敗は-1
        [SerializeField] GameObject match;
        [SerializeField] LF_SceneChange scenechange;
        private Transform matchtransform;
        [SerializeField] AudioClip beep;
        AudioSource audioSource;

        public override void OnGameStart()
        {
            parent = this.gameObject.transform.parent.gameObject;  //自分の親（ルーペ）を取得
            audioSource = GetComponent<AudioSource>();
            loupescript = parent.GetComponent<LF_loupeMover>();
            matchtransform = match.GetComponent<Transform>();
            flag = loupescript.flag;
        }
        void Update()
        {
            flag = loupescript.flag;
            if (flag && SuccessOrFailure == 0)
            {
                Vector2 matchpos = matchtransform.position;
                Vector2 pos = transform.position;
                float abs = Vector2.Distance(matchpos, pos);
                //float abssqrd = Mathf.Pow(transform.position.x - matchtransform.position.x, 2f) + Mathf.Pow(transform.position.y - matchtransform.position.y, 2f); //焦点とマッチまでの距離の平方．もっといい方法あるんですかね……？
                if (abs <= 0.5)  //距離が0.5以下なら成功とする
                {
                    SuccessOrFailure = 1;
                    scenechange.StartClear();
                    MGManager.ClearGame();
                    Debug.Log(abs.ToString());
                } else
                {
                    audioSource.PlayOneShot(beep);
                    SuccessOrFailure = -1;
                    Debug.Log("失敗！！！！ " + abs.ToString());
                    StartCoroutine(retry());
                }
            }
        }
        /*
        public override void OnGameEnd()  //ゲームの終了時にClearGameを呼ぼうと企んだ残骸
        {
            if (SuccessOrFailure == 1)
            {
                MGManager.ClearGame();
            }
        }
        */
        IEnumerator retry()
        {
            yield return new WaitForSeconds(1f / Time.timeScale);  //１秒待つ
            loupescript.flag = false;
            SuccessOrFailure = 0;
        }
    }
}