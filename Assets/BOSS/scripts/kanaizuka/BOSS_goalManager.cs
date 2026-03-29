using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace BOSS{

    public class BOSS_goalManager : MiniGameBase{

        //現在のシーンを格納する変数
        Scene currentscene;

        //シーン内のオブジェクト情報を格納するための配列を宣言
        GameObject[] rootobjects;

        [Header("諸数値")]
        [SerializeField] private float goal_duration = 60.0f; //クリア判定をする時間(秒)
        [SerializeField] private GameObject clear; //クリア時に表示するオブジェクト
        [SerializeField] private GameObject clear_yamada; //クリア時の山田オブジェクト
        [SerializeField] private GameObject panel; //暗転等を実行するpanel
        [SerializeField] private GameObject goal_object; //ゴールの崖
        [SerializeField] private Animator changing; //暗転等のアニメーション

        // 時間を測るための変数
        private float timer = 0f;
        private bool isGoalStarted = false;

        public override void OnGameStart(){
            currentscene = SceneManager.GetActiveScene(); //現在シーンを取得
        }

        void Update(){
            // ★追加：演出監督から「スタート！」の合図が出るまで、時間の計測は「待て」！
            if (!BOSS_StartSequence.isGamePlaying) return;

            // すでにゴール処理が始まっていたら何もしない
            if(isGoalStarted) return;

            // 時間を計測
            timer += Time.deltaTime;

            // 指定した時間（60秒）に達したら、ゴール演出をスタート！
            if(timer >= goal_duration){
                isGoalStarted = true;
                StartCoroutine(goal());
            }
        }

        IEnumerator goal(){
            panel.SetActive(true); //panelを有効化
            changing.SetTrigger("Show"); //暗転

            //シーン内のオブジェクト情報の取得
            rootobjects = currentscene.GetRootGameObjects();
            
            yield return new WaitForSeconds(0.4f); //画面切り替え中

            //rootobjectsに格納したgoalManager,panel以外のすべてのオブジェクトをオフに
            foreach (GameObject obj in rootobjects){

                if (obj == null) continue;

                if (obj != gameObject && !obj.CompareTag("MainCamera") && !obj.CompareTag("panel")){
                    obj.SetActive(false);
                }
            }

            clear.SetActive(true);
            yield return new WaitForSeconds(1f); //暗転中

            clear.SetActive(true); //クリア演出を有効化

            yield return new WaitForSeconds(0.6f);
            panel.SetActive(false); //panelを無効化
            goal_object.SetActive(true);
            clear_yamada.SetActive(true);

            if(Camera.main.TryGetComponent<BOSS_clear_camera>(out var mov)){
                mov.clear_camera();
            }

            SEPlay("BOSS_GoalSE",false);
            yield return new WaitForSeconds(4f);

            MGManager.ClearGame();
        }
    }
}