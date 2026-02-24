using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

namespace PTgame
{
    public class PT_Manager : MiniGameBase
    {
        [SerializeField] private List<GameObject> presents; //プレゼント個々制御
        [SerializeField] private List<PT_FallLeaves> leaves; //風用の葉群
        [SerializeField] private int present_count;
        [SerializeField] private float present_slope;
        [SerializeField] private float obstacle_power;
        [SerializeField] private float action_time;
        [SerializeField] private float move_x; //移動距離
        [SerializeField] private float ppm_x; //アニメーション移動 (present parent move x)
        [SerializeField] private float game_time;
        [SerializeField] private float endless_timer;
        [SerializeField] public bool fall; //落下したか判定
        [SerializeField] public bool endlessmode;
        [SerializeField] private GameObject present_parent; //プレゼント全体制御
        [SerializeField] private GameObject present;
        [SerializeField] private GameObject o;
        [SerializeField] private List<GameObject> obstacles; //障害物リスト
        [SerializeField] private PT_Move mover; //操作スクリプト
        [SerializeField] private PT_Obstacle obstacle_info; //障害物スクリプト
        [SerializeField] private Camera game_camera;
        [SerializeField] private int testlevel;

        public override void OnGameStart()
        {
            MGManager.TestPlay(testlevel);
            MGManager.Load();
            //MGManager.BGMPlay(false);
            if (endlessmode)
            {
                present_count = 5;
                present_slope = 0;
                endless_timer = 0;
            }
            else
            {
                present_count = 5 + (int)((Time.timeScale - 1) * 30);
                game_camera.orthographicSize = 2f + present_count / 1.25f;
                game_camera.transform.position = new Vector3(0, -2.5f + (float)present_count / 2, -10);
                if (Time.timeScale < 1.25f)
                    present_slope = UnityEngine.Random.Range(-1.25f + Time.timeScale, 1.25f - Time.timeScale);
                else
                    present_slope = UnityEngine.Random.Range(-0.01f, 0.01f);

            }

            action_time = 0f;
            obstacle_power = 0f;

            fall = false;

            action_time = UnityEngine.Random.Range(1f, 4f / Time.timeScale);

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
                    if (endlessmode)
                    {
                        // 1. 現在のカメラの「上端」の座標を計算
                        // orthographicSize は中心から上端までの距離
                        float cameraTop = game_camera.transform.position.y + game_camera.orthographicSize;

                        // 2. カメラの上端よりさらに外側に配置（マージンとして +2f ほど）
                        float spawnY = cameraTop + 2f + i;

                        // 3. 親オブジェクト(present_parent)の座標系に変換してセット
                        // 親が動いている場合は InverseTransformPoint を使うのが安全です
                        g.transform.position = new Vector3(present_slope * i, spawnY, 0);
                        fallScript.present_tp = i;
                    }
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
                present_slope = presents[1].transform.localPosition.x + presents[1].transform.localPosition.x * game_time;

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

                manage_obstacles(); //障害物の管理
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
                g.transform.localPosition = new Vector3((present_slope + obstacle_power * game_time - move_x * 4 * power) * i, i, 0);
            }
        }

        private void manage_obstacles()
        {
            if (action_time > -1f && action_time < 0f)
            {
                action_time = -10f;
                action_time = UnityEngine.Random.Range(2f, 4f / Time.timeScale);
                if (obstacles.Count == 0) return;
                int i = UnityEngine.Random.Range(0, obstacles.Count);
                o = Instantiate(obstacles[i]);
                obstacle_info = o.GetComponent<PT_Obstacle>();
            }
            if (obstacle_info != null)
            {
                Debug.Log("障害物出現中 影響度: " + obstacle_info.power);
                obstacle_power = obstacle_info.power;
                PT_Wind w = o.GetComponent<PT_Wind>();
                if (w != null)
                {
                    foreach (PT_FallLeaves l in leaves)
                    {
                        l.obstacle_power = obstacle_power;
                    }
                }
                else
                {
                    foreach (PT_FallLeaves l in leaves)
                    {
                        l.obstacle_power = 0;
                    }
                }
            }
            else
            {
                obstacle_power = 0f;
                foreach (PT_FallLeaves l in leaves)
                {
                    l.obstacle_power = 0;
                }
            }
        }
    }
}
