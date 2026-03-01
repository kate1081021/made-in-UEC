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
        [SerializeField] private AudioClip[] se;
        [SerializeField] private List<GameObject> presents; //プレゼント個々制御
        [SerializeField] private List<PT_FallLeaves> leaves; //風用の葉群
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
        [SerializeField] private GameObject o;
        [SerializeField] private List<GameObject> obstacles; //障害物リスト
        [SerializeField] private PT_Move mover; //操作スクリプト
        [SerializeField] private List<PT_Obstacle> obstacle_info; //障害物スクリプト
        [SerializeField] private Camera game_camera;
        [SerializeField] private int testlevel;
        [SerializeField] public bool endless_mode;
        [SerializeField] private float endless_timer;
        [SerializeField] private float endless_apt; //(add present time)
        [SerializeField] private float endless_apt_duration;
        [SerializeField] private float endless_difficult;
        [SerializeField] private TextMeshProUGUI pc; //(present count)
        [SerializeField] private TextMeshProUGUI et; //(endless timer)
        [SerializeField] private GameObject pc_ui;
        [SerializeField] private GameObject et_ui;



        public override void OnGameStart()
        {
            if (testlevel != 0)
                MGManager.TestPlay(testlevel);
            MGManager.Load();
            BGMPlay(false);
            endless_difficult = 1.0f;
            pc_ui.SetActive(true);
            if (endless_mode)
            {
                present_count = 5;
                present_slope = 0;
                endless_timer = 0;
                endless_apt = 10f;
                endless_apt_duration = 1f;
                et_ui.SetActive(true);
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
                    if (endless_mode)
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
                Debug.Log("present_tp " + g.transform.position);
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

                pc.text = "現在のプレゼント数：" + present_count.ToString() + "個";
                if (endless_mode)
                {
                    endless_timer += game_time;
                    if (endless_timer >= endless_apt)
                    {
                        addpresent();
                        endless_apt += endless_apt_duration;
                        endless_difficult = 1f + present_count / 50f;
                    }
                    int minutes = (int)endless_timer / 60;
                    int seconds = (int)endless_timer % 60;
                    int milliseconds = (int)((endless_timer * 1000) % 1000);
                    et.text = string.Format("{0}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);

                    // ★エンドレスモード時のみ、カメラをスムーズに移動させる
                    move_camera_smoothly();
                }
                present_slope = presents[1].transform.localPosition.x + presents[1].transform.localPosition.x * game_time * endless_difficult;

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
                g.transform.localPosition = new Vector3((present_slope + obstacle_power * game_time - move_x * 4 * power) * i, g.transform.localPosition.y, 0);
            }
        }
        private void manage_obstacles()
        {
            // 1. 出現タイマーの処理
            if (action_time > -1f && action_time < 0f)
            {
                action_time = UnityEngine.Random.Range(2f / Time.timeScale / endless_difficult, 4f / Time.timeScale / endless_difficult);
                if (obstacles.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, obstacles.Count);
                    o = Instantiate(obstacles[randomIndex]);
                    PT_Obstacle script = o.GetComponent<PT_Obstacle>();
                    if (script != null) obstacle_info.Add(script);
                }
            }

            // 2. リストのゴミ掃除（nullになった障害物を一括削除）
            // これでエラーの主要因である「消えたオブジェクトへのアクセス」を防ぎます
            obstacle_info.RemoveAll(obs => obs == null);

            // 3. パワーの計算
            obstacle_power = 0f;
            foreach (PT_FallLeaves l in leaves)
            {
                l.obstacle_power = 0;
            }

            // 全ての生きている障害物のパワーを合計する
            foreach (var info in obstacle_info)
            {
                // ここでは info が null でないことが保証されています
                obstacle_power += info.power;

                // もし風(Wind)コンポーネントがあれば、葉っぱに影響を与える
                PT_Wind w = info.GetComponent<PT_Wind>();
                if (w != null)
                {
                    foreach (PT_FallLeaves l in leaves)
                    {
                        l.obstacle_power += info.power;
                    }
                }
            }
        }
        private void addpresent()
        {
            // 1. 生成
            GameObject g = Instantiate(present, present_parent.transform);

            // 2. 現在のプレゼント数を更新
            present_count++;

            // 3. 生成位置の計算
            // 現在のカメラの上端を取得して、そこより少し上に配置する
            float cameraTop = game_camera.transform.position.y + game_camera.orthographicSize;
            float spawnY = cameraTop + 2f;

            // ワールド座標で高さを指定し、横位置は現在のスロープに合わせる
            g.transform.position = new Vector3(present_slope * (present_count - 1), spawnY, 0);

            // 4. コンポーネントの設定
            PT_FallPresent fallScript = g.GetComponent<PT_FallPresent>();
            if (fallScript != null)
            {
                fallScript.manager = this;
                // 積み上がるべき目標の高さ（現在のインデックス）を渡す
                fallScript.present_tp = present_count - 1;
            }

            // 5. 管理リストに追加（これで Update での移動対象になる）
            presents.Add(g);

            Debug.Log($"プレゼント追加！ 現在の合計: {present_count}個");
        }
        private void move_camera_smoothly()
        {
            if (game_camera == null) return;

            // 1. 「現在のプレゼント数」から、本来あるべき理想のカメラ設定を計算（ゴール地点）
            float targetSize = 2f + present_count / 1.25f;
            float targetY = -2.5f + (float)present_count / 2f;

            // 2. 現在の値をスムーズに目標値へ近づける
            // 第3引数の数値（0.5f や 1.0f）を大きくすると、カメラの動きが速くなります

            // サイズ（ズーム）の移動
            game_camera.orthographicSize = Mathf.Lerp(
                game_camera.orthographicSize,
                targetSize,
                1.0f * Time.deltaTime
            );

            // 座標（高さ）の移動
            Vector3 currentPos = game_camera.transform.position;
            float nextY = Mathf.Lerp(currentPos.y, targetY, 1.0f * Time.deltaTime);
            game_camera.transform.position = new Vector3(0, nextY, -10);
        }
    }
}
