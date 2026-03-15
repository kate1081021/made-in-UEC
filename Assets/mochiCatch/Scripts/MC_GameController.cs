using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace catchMochi
{
    public class MC_GameController : MiniGameBase
    {
        /// <summary>
        /// Girl関連
        /// </summary>
        private MC_GirlModel girlModel;  // 女の子のモデル
        public MC_GirlView girlView;  // 女の子の見た目
        private string[] phases = {"normal", "catch", "draw", "eat"};  // ステータスの全通り

        public float[] waitSeconds;

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
        public bool game_in_progress;

        // ゲーム開始時からの経過時刻を記録
        public float time;

        // public float time; // ゲーム開始からの経過時間
        // public Mother mother;  // お母さんオブジェクトを参照
        // public mochiCatch girl;  // 女の子を管理
        // public Girl_Anim girlanim;
        //public Image girl_picture;
        // public Sprite girl_bikkuri_picture;
        // public bool game_in_progress;
        // public GameObject suggestion;
        // public GameObject panelOfTitle;
        // public GameObject panelOfGirl;
        // public RectTransform Shouji;
        // public Animator Shouji_anim;
        // public bool isResult = false;

        // Update is called once per frame
        public override void OnGameStart()
        {
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

            // StartCoroutine(Title());
            MGManager.Load();  // ゲームの開始を宣言する
        }

        private IEnumerator Title()
        {
            // パラメータリセット
            // panelOfGirl.SetActive(true);
            // mother.gameObject.SetActive(true);
            // suggestion.SetActive(true);
            // mother.enabled = true;
            yield return null;
        }

        // 毎フレーム呼び出し
        void Update()
        {
            // ユーザーガイドを消す
            if (time > 3f) { // suggestion.SetActive(false); 
            }
            
            // ゲームクリア
            if (girlModel.ateMochi == 3) { MGManager.ClearGame(); }
            
            // GameOver
            if (motherModel.isMotherSeeing && girlModel.status == "eat" && game_in_progress)
            {   
                // 変数処理
                // isResult = true;
                game_in_progress = false;

                // 演出
                motherView.GetAngry();
                girlView.Surprised(); 

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
            if (!girlModel.eating) // 食事を始める
            {
                girlModel.StartEating();
                Debug.Log("食事開始");
                StartCoroutine(startEating());
            }
        }

        // Enter/Spaceがはなされたとき
        protected override void OnActionCanceled(float value)
        {
            if (girlModel.eating)
            {
                // 食べている状態を解除する
                girlModel.CancelEating();
                
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
                
                for (int i = 0; i < 3; i++)
                {
                    girlModel.ChangeStatus(phases[i]);

                    // statusが"eat"であるとき、食べるSEを鳴らす
                    if (girlModel.status == "eat" && !motherModel.isMotherSeeing)
                    {
                        // SePlayer.Instance.Play("食べ物をパクッ");
                    }

                    // 指定の時間だけ待つ。(ただし途中で遮られる可能性もある)
                    yield return WaitPhase(girlModel.CaliculateEatingSpan(waitSeconds[i]));

                    if (!Action.IsPressed())
                    {
                        // statusが"eat"である場合、ateMochiを一つ追加
                        if (girlModel.status == "eat" && !motherModel.isMotherSeeing) { girlModel.ateMochi++; }
                        girlModel.CancelEating();  
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
