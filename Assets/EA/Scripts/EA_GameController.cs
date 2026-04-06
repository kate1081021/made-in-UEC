using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EA
{
    public class EA_GameContorller : MiniGameBase
    {
        /// <summary>
        /// Board関連
        /// </summary>
        private EA_BoardModel boardModel;  // 盤面のモデル
        public EA_BoardView boardView;  // 盤面の見た目

        // 正解の盤面のデータ(4x4)
        public EA_BoardData[] targets4;

        // 正解の盤面のデータ(6x6)
        public EA_BoardData[] targets6;

        // 正解の盤面のデータ(8x8)
        public EA_BoardData[] targets8;

        // stageに合わせてデータセットから一つデータを選んで持ってくる
        public EA_BoardData ChooseData()
        {
            // 4x4
            if (1 <= MGManager.stage && MGManager.stage <= 15)
            {
                int idx = Random.Range(0, targets4.Length);
                boardModel.boardLength = 4;
                return targets4[idx];
            }
            // 6x6
            else if (16 <= MGManager.stage && MGManager.stage <= 30)
            {
                int idx = Random.Range(0, targets6.Length);
                boardModel.boardLength = 6;
                return targets6[idx];
            }
            // 8x8
            else
            {
                int idx = Random.Range(0, targets8.Length);
                boardModel.boardLength = 8;
                return targets8[idx];
            }
        }

        /// <summary>
        /// Cell関連
        /// </summary>
        public GameObject cellPrefab;  // セルオブジェクトのプレファブ
        private EA_Board cells;  // セルのデータをまとめて持っている(下記の二つ)

        // private GameObject[,] currentCells;  // セルをまとめて保持する
        // private GameObject[,] targetCells;  // 正解のセルをまとめて保持する
        

        /// <summary>
        /// Carsol関連
        /// </summary>
        public EA_CarsorView carsolView;  // カーソルの見た目
        public GameObject carsol;  // カーソルオブジェクト
        private int carsor_x = 0;
        private int carsor_y = 0;
        private bool waitForNextTry = false;  // 一回間違えた場合、次に操作できるようになるまでラグが生じるようにする。
        private Vector2 previous_val;  // 最後に受け取ったカーソル入力

        /// DemoScreen
        public GameObject demoScreen;

        /// <summary>
        /// CrossButton関連
        /// </summary>
        public EA_CrossButtonView cbView;

        /// <summary>
        /// AButton関連
        /// </summary>
        public EA_AButtonView aView;


        /// <summary>
        /// UI関連
        /// </summary>
        public EA_UIManager uiManager;  // UIManager

        /// <summary>
        /// 勝利演出
        /// </summary>
        public EA_RisajuView rView;

        /// <summary>
        /// 特殊変数
        /// </summary>
        public static bool isFirstGame = true;


        // ゲーム開始時に呼ばれる
        public override void OnGameStart()
        {
            // BGM再生
            BGMPlay();

            // 盤面のモデルを作成する
            boardModel = new EA_BoardModel();

            // データセットの中から一つ抽出する
            bool[] data = ChooseData().data;

            // 現在のセルの内容が変更されたとき
            boardModel.OnCurrentCellChanged = (x, y, side) =>
            {
                // データの変更を見た目に反映する
                cells.currentCells[y, x].GetComponent<EA_CellView>().Flip(side);
            };

            // 正解のセルの内容が変更されたとき
            boardModel.OnTargetCellCreated = (x, y, side) =>
            {
                // データの変更を見た目に反映する
                cells.targetCells[y, x].GetComponent<EA_CellView>().Flip(side);
            };

            // 正解のセルの内容が変更されたとき
            if (isFirstGame){
                boardModel.OnDemoCellChanged = (x, y, side) =>
                {
                    // データの変更を見た目に反映する
                    cells.demoCells[y, x].GetComponent<EA_CellView>().Flip(side);
                };
            }

            // セルを生成する
            int size = boardModel.boardLength;
            cells = boardView.CreateBoard(size, cellPrefab, isFirstGame);

            // 盤面を作成する
            boardModel.SetBoard(data);

            // デモを再生
            if (isFirstGame)
            {
                // カーソルを移動する
                carsolView.CarsorMove(5, 2, 8);

                // セッティング
                waitForNextTry = true;
                StartCoroutine(DemoPlay());

            }
            else
            {
                // デモスクリーンを閉じる
                demoScreen.SetActive(false);

                // カーソルを移動する
                carsolView.CarsorMove(0, 0, size);
            }

            // ミニゲームを開始する
            MGManager.Load();


            // ここからは二回目になる
            isFirstGame = false;

        }

        // カーソル操作
        protected override void OnMovePerformed(Vector2 value)
        {
            // クリア時または硬直時は呼び出さない
            if (MGManager.IsClear || waitForNextTry) { return; }

            // スティック対応
            Vector2 val = convert_stick_to_dir(value);
            if (val == previous_val) { return; }
            previous_val = val;

            // x座標
            carsor_x = (carsor_x + (int)val.x) % boardModel.boardLength;
            if (carsor_x < 0) { carsor_x += boardModel.boardLength; }

            // y座標
            carsor_y = (carsor_y - (int)val.y) % boardModel.boardLength;
            if (carsor_y < 0) { carsor_y += boardModel.boardLength; }

            // カーソルを動かす
            carsolView.CarsorMove(carsor_x, carsor_y, boardModel.boardLength);

        }

        protected override void OnMoveCanceled(Vector2 value)
        {
            previous_val = convert_stick_to_dir(value);
        }

        // デモプレイ(1回だけ呼ばれる)
        private IEnumerator DemoPlay()
        {
            // 少し待機
            yield return new WaitForSeconds(0.5f);

            // 右キーを押す
            carsolView.CarsorMove(6, 2, 8);
            cbView.Pressed(1);
            yield return new WaitForSeconds(0.3f);
            cbView.Pressed(0);
            yield return new WaitForSeconds(0.2f);

            // 下キーを押す
            carsolView.CarsorMove(6, 3, 8);
            cbView.Pressed(2);
            yield return new WaitForSeconds(0.3f);
            cbView.Pressed(0);
            yield return new WaitForSeconds(0.2f);

            // Aボタンを押す
            boardModel.DemoFlip(1, 1);
            aView.Pressed(true);
            yield return new WaitForSeconds(0.3f);
            aView.Pressed(false);
            yield return new WaitForSeconds(0.5f);

            // 硬直解除&画面リセット
            waitForNextTry = false;
            demoScreen.SetActive(false);
            boardView.DeleteDemoBoard(cells.demoCells);
            carsolView.CarsorMove(0, 0, boardModel.boardLength);


        }

        // Enter/Spaceキーが押された
        protected override void OnActionStarted(float value)
        {
            // クリア時または硬直時は呼び出さない
            if (MGManager.IsClear || waitForNextTry) { return; }

            // SE
            SEPlay("card_open");

            // 硬直がなければそのまま呼び出し
            StartCoroutine(CheckCell());

        }

        // 判定のためのコルーチン
        private IEnumerator CheckCell()
        {
            // 盤面でひっくり返す処理を行う
            boardModel.Flip(carsor_x, carsor_y);

            // クリア判定を行う
            if (boardModel.CheckBoard())
            {
                // ゲームクリア
                MGManager.ClearGame();

                // SE
                SEPlay("clear");

                // りさじゅう呼び出し
                StartCoroutine(rView.Animation());

                // クリア時の演出を呼び出す
                uiManager.GameClear();

            }
            else
            {
                // 間違っていた場合は少し時間を空けてから盤面をもとに戻す
                SEPlay("fail");  // SE
                waitForNextTry = true;
                yield return new WaitForSeconds(0.5f);
                SEPlay("card_close");  // SE
                waitForNextTry = false;
                boardModel.Flip(carsor_x, carsor_y);

            }

            // なんでもない
            yield return null;

        }

    }
}