using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace PTgame
{
    public class PT_Manager : MiniGameBase
    {
        [SerializeField] private List<GameObject> presents; //プレゼント個々制御
        [SerializeField] private int present_count;
        [SerializeField] private float present_slope;
        [SerializeField] private float obstacle_power;
        [SerializeField] private float action_time;
        [SerializeField] private float move_x; //移動距離
        [SerializeField] private float ppm_x; //アニメーション移動 (present parent move x)
        [SerializeField] private float game_time;
        [SerializeField] public bool fall; //落下したか判定
        [SerializeField] private GameObject present_parent; //プレゼント全体制御
        [SerializeField] private GameObject present;
        [SerializeField] private List<GameObject> obstacles; //障害物リスト
        [SerializeField] private PT_Move mover; //操作スクリプト
        [SerializeField] private PT_Obstacle obstacle_info; //障害物スクリプト
        [SerializeField] private Camera camera;
        [SerializeField] private int testlevel;

        public override void OnGameStart()
        {
            MGManager.TestPlay(testlevel);
            MGManager.Load();

            present_count = 5 + (int)((Time.timeScale - 1) * 30);

            camera.orthographicSize = 2f + present_count / 1.25f;
            camera.transform.position = new Vector3(0, -2.5f + (float)present_count / 2, -10);

            action_time = 0f;
            obstacle_power = 0f;

            if (Time.timeScale < 1.25f)
                present_slope = UnityEngine.Random.Range(-1.25f + Time.timeScale, 1.25f - Time.timeScale);
            else
                present_slope = UnityEngine.Random.Range(-0.01f, 0.01f);

            fall = false;

            action_time = UnityEngine.Random.Range(1f, 6f / Time.timeScale);

            for (int i = 0; i < present_count; i++)
            {
                GameObject g = Instantiate(present, present_parent.transform);
                g.transform.localPosition = new Vector3(present_slope * i, i, 0);
                // 1. 生成したオブジェクトにある PT_FallPresent スクリプトを探す
                PT_FallPresent fallScript = g.GetComponent<PT_FallPresent>();

                // 2. そのスクリプトの manager 変数に、この Manager 自身 (this) を入れる
                if (fallScript != null)
                {
                    fallScript.manager = this;
                }
                presents.Add(g);
            }
        }

        public override void OnGameEnd()
        {
            if (present_slope * present_slope < 1)
            {
                MGManager.ClearGame(); //クリア
            }
        }

        void Update()
        {
            //Debug.Log("local" + presents[1].transform.localPosition);
            //Debug.Log("present_slope"+ present_slope +"delta" + (movex*0.01f));
            game_time = Time.timeScale * Time.deltaTime;
            action_time -= game_time;
            if (present_slope * present_slope < 1)
            {
                present_slope = presents[1].transform.localPosition.x + presents[1].transform.localPosition.x * (float)present_count / 10 * game_time;

                //Debug.Log("1.00 "+present_slope+" vs "+Time.timeScale+" "+(present_slope*Time.timeScale));
                //present_slope += Time.timeScale*move.transform.position.x/500f;

                move_x = mover.HandleMove().x; //操作取得

                manage_obstacles(); //障害物の管理

                change_animation(); //プレゼントの傾けるや左右移動のアニメーション関連

                change_present_position(); //プレゼントボックスの移動関連
            }
            else
            {
                Debug.Log("Lose...");
                fall = true;
                change_animation(); //プレゼントの傾けるや左右移動のアニメーション関連
            }
        }

        private void change_animation()
        {
            if (fall)
            {
                if (ppm_x * ppm_x > 0.0001f)
                {
                    ppm_x = ppm_x / (1.05f * Time.timeScale);
                }
                else
                {
                    ppm_x = 0;
                }
            }
            else if (move_x < 0)
            {
                if (ppm_x <= 0)
                {
                    ppm_x -= game_time * 10;
                }
                else if (ppm_x > 0)
                {
                    ppm_x = ppm_x / (10 * Time.timeScale) - game_time * 10;
                }
            }
            else if (move_x > 0)
            {
                if (ppm_x >= 0)
                {
                    ppm_x += game_time * 10;
                }
                else if (ppm_x < 0)
                {
                    ppm_x = ppm_x / (10 * Time.timeScale) + game_time * 10;
                }
            }
            else
            {
                if (ppm_x * ppm_x > 0.0001f)
                {
                    ppm_x = ppm_x / (1.2f * Time.timeScale);
                }
                else
                {
                    ppm_x = 0;
                }
            }
            present_parent.transform.position = new Vector3((float)Math.Atan(ppm_x) / 10, (float)Math.Atan(ppm_x) * (float)Math.Atan(ppm_x) / 10, 0);
            present_parent.transform.localEulerAngles = new Vector3(0, 0, (float)Math.Atan(ppm_x) * 10f);
        }

        private void change_present_position()
        {
            for (int i = 0; i < presents.Count; i++)
            {
                GameObject g = presents[i];
                float power = (float)Math.Atan((float)present_count / 80);
                if (power > 0.1f)
                    power = 0.1f;
                //Debug.Log("power"+power);
                g.transform.localPosition = new Vector3((present_slope + obstacle_power / 100 - move_x * 4 * power) * i, i, 0);
            }
        }

        private void manage_obstacles()
        {
            if (action_time > -1f && action_time < 0f)
            {
                action_time = -10f;
                if (obstacles.Count == 0) return;
                int i = UnityEngine.Random.Range(0, obstacles.Count);
                GameObject o = Instantiate(obstacles[i]);
                obstacle_info = o.GetComponent<PT_Obstacle>();
            }
            if (obstacle_info != null)
            {
                Debug.Log("障害物出現中 影響度: " + obstacle_info.power);
                obstacle_power = obstacle_info.power;
            }
            else
            {
                obstacle_power = 0f;
            }
        }
    }
}
