using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace catchMochi
{
    public class MC_GameController : MiniGameBase
    {
        /// <summary>
        /// Girl関連
        /// </summary>
        private MC_GirlModel girlModel;  // 女の子のモデル
        public MC_GirlView girlView;  // 女の子の見た目

        /// <summary>
        /// Mochi関連
        /// </summary>
        public MC_MochiView mochiView; // 餅の見た目
        private string[] phases = {"normal", "catch", "draw", "eat"};  // ステータスの全通り

        private float[] waitSeconds = {0.25f, 0.25f, 0.9f};

        /// <summary>
        /// Mother関連
        /// </summary>
        private MC_MotherModel motherModel;
        public MC_MotherView motherView;

        /// <summary>
        /// Shoji関連
        /// </summary>
        public MC_ShojiView shojiView;

        /// <summary>
        /// UIManager関連
        /// </summary>
        public MC_UIManager uIManager;

        /// <summary>
        /// Game関連
        /// </summary>
        
        // ゲームが進行状態にあるかどうか
        private bool game_in_progress;

        // ゲーム開始時からの経過時刻を記録
        private float time;

        // スピードの補正値
        private float multiplier = 0.1f;

        // C#の古いバージョンでも動くようにListの初期化を修正
        private List<float[]> patterns = new List<float[]> {
            new float[] { 3f, 0.625f, 0.5f },  // [何もない時間, 揺れている時間, 開いている時間]
            new float[] { 2.5f, 0.75f, 0.75f },
            new float[] { 2.75f, 0.75f, 0.5f },
        };

        // 開始時
        public override void OnGameStart()
        {
            // BGMを再生
            BGMPlay();
            
            // Girlのモデルを呼び出す
            girlModel = new MC_GirlModel();

            // ステータスが変化したとき
            girlModel.OnStatusChanged = (string status) =>
            {
                girlView.Animation(status);  // アニメーションに対して即座に反映する
            };

            // Motherのモデルを呼び出す
            motherModel = new MC_MotherModel();

            // 各ステータスの初期化
            game_in_progress = true;
            time = 0.0f;

            // uiを有効化
            uIManager.GetEnabled(true);

            // ランダムにお母さんが現れる
            StartCoroutine(RandomMotherAppear());

            MGManager.Load();  // ゲームの開始を宣言する
        }

        private IEnumerator Title()
        {
            // パラメータリセット
            // mother.gameObject.SetActive(true);
            // suggestion.SetActive(true);
            // mother.enabled = true;
            yield return null;
        }

        // タイミングを調整
        public IEnumerator RandomMotherAppear()
        {
            while (game_in_progress)
            {

                // 1. パターンをランダムに決定する
                // Count は大文字、Random.Rangeの最大値は「含まれない」のでこれでOK
                int pattern_idx = Random.Range(0, patterns.Count);
                float speed = girlModel.eatingSpeed;
                float[] times = patterns[pattern_idx];

                // この辺の処遇を検討中
                float factor = 0.5f*(1 - (float)Mathf.Exp(-multiplier*speed));

                float idleTime = times[0] * Random.Range(0.8f, 1.2f) * (1 - factor);
                float shakeTime = times[1] * Random.Range(0.8f, 1.2f) * (1 - factor);
                float openTime = times[2] * Random.Range(0.8f, 1.2f) * (1 - factor);

                // 2. 何もない時間（待機）
                yield return new WaitForSeconds(idleTime);

                // 3. 障子がガタガタ揺れる
                shojiView.Shaking(true);
                SEPlay("MC_GataGata");
                yield return new WaitForSeconds(shakeTime);
                
                // 4. ここで分岐する(60%で全開、20%で半開、20%で何も起こらない)
                int rand = Random.Range(0, 100);

                // 全開
                if (rand < 70)
                {
                    SEPlay("MC_Open");
                    rand = Random.Range(0, 100);
                    // 普通に出てくる
                    if (rand < 75)
                    {
                        motherView.GetEnabled(true);
                        shojiView.Open(true);
                        yield return new WaitForSeconds(0.1f);
                        motherModel.isMotherSeeing = true;
                    }
                    // 開くだけ
                    else
                    {
                        motherView.GetEnabled(false);
                        shojiView.Open(false);
                        yield return new WaitForSeconds(0.1f);
                        motherModel.isMotherSeeing = false;
                    }
                
                    yield return new WaitForSeconds(openTime);
                    if (!game_in_progress) { yield break; }
                    shojiView.Open(false);
                    motherModel.isMotherSeeing = false;

                    // ふすまを閉めるときのSE
                }
                // 半開
                else if (rand < 80)
                {
                    SEPlay("MC_FakeOpen");
                    shojiView.OpenLittle();
                    yield return new WaitForSeconds(0.2f);
                }

                // 揺れているというフラグを折る
                shojiView.Shaking(false);
            }
        }

        // 毎フレーム呼び出し
        void Update()
        {
            // ユーザーガイドを消す
            if (time > 3f) { uIManager.GetEnabled(false); }
            
            // ゲームクリア
            if (girlModel.ateMochi == 3)
            {
                girlView.Win();
                MGManager.ClearGame();
                StopCoroutine(startEating());
            }
            
            // GameOver
            if (motherModel.isMotherSeeing && girlModel.status == "eat" && game_in_progress)
            {   
                // 変数処理
                // isResult = true;
                game_in_progress = false;

                // 演出
                SEPlay("MC_Found");
                motherView.GetAngry();
                girlView.Surprised();
                StopCoroutine(startEating());
                // StartCoroutine(end());

            }
            else if (game_in_progress)
            {
                time += Time.deltaTime;
            }
        }

        /// <summary>
        /// イベント関連
        /// </summary>
        
        // Enter/Spaceが押されたとき
        protected override void OnActionStarted(float value)
        {
            if (!girlModel.eating && girlModel.ateMochi < 3 && game_in_progress) // 食事を始める
            {
                girlModel.StartEating();
                Debug.Log("食事開始");
                StartCoroutine(startEating());
            }
        }

        // Enter/Spaceがはなされたとき
        protected override void OnActionCanceled(float value)
        {
            if (girlModel.eating && game_in_progress && girlModel.status != "eat")
            {
                // 食べている状態を解除する
                girlModel.CancelEating();
                mochiView.ShowMochi(girlModel.ateMochi);
                // isCancel = true;

                // デバッグ
                switch (girlModel.status)
                {
                    case "catch":
                    Debug.Log("キャッチキャンセル");
                    break;
                    case "draw":
                    Debug.Log("口に運ぶのをキャンセル");
                    break;
                    case "eat":
                    Debug.Log("無理やり食べた");
                    break;
                }

            }
        }
        

        // 指定された秒数だけ待つ
        private IEnumerator WaitPhase(float seconds)
        {
            float end = Time.time + seconds;
            while ((Action.IsPressed() || girlModel.status == "eat") && Time.time <= end)
            {
                yield return null;
            }
        }

        private IEnumerator startEating() {
            while (Action.IsPressed()){
                Debug.Log("eating");
                girlModel.ChangeStatus("catch");
                
                for (int i = 1; i < 4; i++)
                {
                    girlModel.ChangeStatus(phases[i]);

                    // statusが"eat"であるとき、食べるSEを鳴らす
                    if (girlModel.status == "eat" && !motherModel.isMotherSeeing)
                    {
                        mochiView.HideMochi(girlModel.ateMochi);
                        SEPlay("MC_Eat");
                        // SePlayer.Instance.Play("食べ物をパクッ");
                    }

                    // 指定の時間だけ待つ。(ただし途中で遮られる可能性もある)
                    yield return WaitPhase(girlModel.CaliculateEatingSpan(waitSeconds[i-1]));

                    if (!Action.IsPressed())
                    {
                        // statusが"eat"である場合、ateMochiを一つ追加
                        if (girlModel.status == "eat" && !motherModel.isMotherSeeing) { girlModel.ateMochi++; }
                        if (game_in_progress){ girlModel.CancelEating(); }
                        // isCancel = false;
                        yield break;
                    }
                }
                Debug.Log("食事成功！");
                girlModel.CancelEating();
                if (!motherModel.isMotherSeeing) { girlModel.ateMochi++; }
            }
            // isCancel = false; // キャンセルフラグのリセットが必要ならここで
        }
        //食べるほど速度上昇、上限あり
    }
}
