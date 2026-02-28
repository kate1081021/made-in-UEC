using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BK
{
    public class BK_GameManager : MiniGameBase
    {
        public GameObject EnemyLeft; // Imageがついたプレハブ
        public GameObject EnemyRight; // Imageがついたプレハブ
        public Transform canvasTransform; // CanvasのTransform
        public List<BK_Enemy> EnemyData = new List<BK_Enemy>();  // BK_Enemy(enemyが立っている場所とy座標を持っている)を保存したデータ
        public List<BK_LocationData> dataset_easy;  // データのまとめ(難易度簡単)
        public List<BK_LocationData> dataset_normal;  // データのまとめ(難易度普通)
        public List<BK_LocationData> dataset;  // 上の３つのうちどれか一つを格納する
        private List<float> y_list;
        public static float escapableRange = 50f;  // 回避の反転許容範囲
        
        // Playerを取得
        public GameObject player;
        private BK_PlayerController player_controller;
        private RectTransform player_pos;  // Playerの位置
        
        // ゲームの進行フラグ
        public bool gameClear = true; 

        // 最初に呼び出し
        public override void OnGameStart()
        {
            // Playerオブジェクトからコンポーネントを抽出
            player_controller = player.GetComponent<BK_PlayerController>();
            player_pos = player.GetComponent<RectTransform>();
            MGManager.TestPlay(50);

            // 難易度設定
            int s = MGManager.stage;
            if (1 <= s && s <= 30)  // 簡単
            {
                dataset = dataset_easy;
            }
            else  // 普通
            {
                dataset = dataset_normal;
            }
            
            // Enemy呼び出し
            y_list = new List<float>
            {
                204.96f, 125.88f, 46.846f, -32.192f, -111.2308f, -190.2692f, -269.3077f
            };

            List<int> data = dataset[Random.Range(0, dataset.Count)].data;
            for (int i = 0; i < data.Count; i++)
            {
                int rand = Random.Range(0, 2);
                float dir = rand == 0 ? -1.0f : 1.0f;
                Vector2 position;
                GameObject newImage;
                float x = 450 + 50 * Random.Range(0, 9);

                // 左
                if (dir == -1.0f)
                {
                    position = new Vector2(-x, y_list[data[i]]);
                    newImage = Instantiate(EnemyLeft, canvasTransform);
                }
                // 右
                else
                {
                    position = new Vector2(x, y_list[data[i]]);
                    newImage = Instantiate(EnemyRight, canvasTransform);
                }

                RectTransform pos = newImage.GetComponent<RectTransform>();
                pos.anchoredPosition = position;
                Debug.Log($"x: {pos.anchoredPosition.x}, y: {pos.anchoredPosition.y}");

                EnemyData.Add(new BK_Enemy(dir, y_list[data[i]]));
                
            }
            MGManager.Load();  // 最初に呼び出し
            StartCoroutine(MainCoroutine());
        }

        private IEnumerator MainCoroutine()
        {
            // ちょっと待つ
            BGMPlay(applyToTimeScale: true);
            player_controller.startWalking();

            // 敵のy座標を上から順に参照していく
            for (int i = 0; i < EnemyData.Count; i++)
            {
                // 許容範囲内で正しい方向キーが押されたか確かめる
                BK_Enemy enemyData = EnemyData[i];
                while (true)
                {
                    // playerのy座標が許容範囲内
                    float player_y = player_pos.anchoredPosition.y;
                    if (Mathf.Abs(player_y - enemyData.Y) <= escapableRange)  
                    {
                        // 方向キーが押された
                        if (Move.WasPressedThisFrame())
                        {
                            // 方向キーの値を確認
                            Vector2 value = Move.ReadValue<Vector2>();

                            // 押されたボタンの向きとプレイヤーがよけた向きが逆になっていることを確認する
                            if ((enemyData.direction == -1.0f && value.x > 0.5f) || (enemyData.direction == 1.0f && value.x < -0.5f))
                            {
                                break;
                            }
                            // 誤って同じ方向を押していた場合
                            else if ((enemyData.direction == 1.0f && value.x > 0.5f) || (enemyData.direction == -1.0f && value.x < -0.5f))
                            {
                                gameClear = false;
                                player_controller.FailureAnimation();
                                break;
                            }
                        }
                    }
                    // playerが許容範囲を超えてしまった場合
                    else if (player_y < enemyData.Y)  
                    {
                        Debug.Log("はいこえたーー！");
                        gameClear = false;
                        player_controller.FailureAnimation();
                        break;
                    }

                    // 処理が止まるのを阻止
                    yield return null;

                }

                // ゲーム失敗
                if (!gameClear) { break; }

            }

            // クリア判定を送る
            if (gameClear) { MGManager.ClearGame(); }

        }
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
