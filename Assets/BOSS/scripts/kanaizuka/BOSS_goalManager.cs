using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BOSS
{
    public class BOSS_goalManager : MiniGameBase
    {
        // マップマネージャを参照できるようにする
        [SerializeField] private BOSS_MapMoveManager mapMoveManager;

        Scene currentscene;
        GameObject[] rootobjects;

        [Header("諸数値")]
        [SerializeField] private float goal_duration = 15.0f;
        [SerializeField] private GameObject clear;

        public override void OnGameStart()
        {
            currentscene = SceneManager.GetActiveScene();
            StartCoroutine(WaitTimer());
        }

        // まずは時間だけ測るコルーチン
        IEnumerator WaitTimer()
        {
            yield return new WaitForSeconds(goal_duration);
            
            // 時間になったらマップマネージャにマップ遷移させる
            mapMoveManager.MoveToFinalGoal();
        }

        // マップマネージャの処理が終わったら、ここが呼ばれる
        public void CompleteGoalSequence()
        {
            rootobjects = currentscene.GetRootGameObjects();
            
            foreach (GameObject obj in rootobjects)
            {
                if (obj != gameObject && !obj.CompareTag("MainCamera") && !obj.CompareTag("Player"))
                {
                    obj.SetActive(false);
                }
            }

            clear.SetActive(true);
            Debug.Log("BOSS戦クリア");
        }
    }
}