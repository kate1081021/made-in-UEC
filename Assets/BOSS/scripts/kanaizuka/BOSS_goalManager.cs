using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BOSS{

    public class BOSS_goalManager : MiniGameBase{

        //現在のシーンを格納する変数
        Scene currentscene;

        //シーン内のオブジェクト情報を格納するための配列を宣言
        GameObject[] rootobjects;

        [Header("諸数値")]
        [SerializeField] private float goal_duration = 15.0f; //クリア判定をする時間(秒)
        [SerializeField] private GameObject clear; //クリア時に表示するオブジェクト

        public override void OnGameStart(){
            currentscene = SceneManager.GetActiveScene(); //現在シーンを取得
            StartCoroutine(goal());
        }

        IEnumerator goal(){
            yield return new WaitForSeconds(goal_duration); //一定時間待機

            //シーン内のオブジェクト情報の取得
            rootobjects = currentscene.GetRootGameObjects();
            
            //rootobjectsに格納したgoalManager以外のすべてのオブジェクトをオフに
            foreach (GameObject obj in rootobjects){
                if (obj != gameObject && !obj.CompareTag("MainCamera")){
                    obj.SetActive(false);
                }
            }

            clear.SetActive(true);
        }
    }
}