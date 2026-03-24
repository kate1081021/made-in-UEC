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

        /// <summary>
        /// UI関連
        /// </summary>
        public EA_UIManager uiManager;  // UIManager

        /// <summary>
        /// 勝利演出
        /// </summary>
        public EA_RisajuView rView;


        // ゲーム開始時に呼ばれる
        public override void OnGameStart()
        {
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

            // セルを生成する
            int size = boardModel.boardLength;
            cells = boardView.CreateBoard(size, cellPrefab);

            // 盤面を作成する
            boardModel.SetBoard(data);

            // カーソルを移動する
            carsolView.CarsorMove(0, 0, size);

            // ミニゲームを開始する
            MGManager.Load();

        }


        // デモプレイ(1回だけ呼ばれる)
        private IEnumerator DemoPlay()
        {
            yield return null;
        }

        // 上下左右キーが押されている
        protected override void OnMovePerformed(Vector2 value)
        {
            // クリア時または硬直時は呼び出さない
            if (MGManager.IsClear || waitForNextTry) { return; }

            // スティック対応
            Vector2 val = convert_stick_to_dir(value);

            // x座標
            carsor_x = (carsor_x + (int)val.x) % boardModel.boardLength;
            if (carsor_x < 0) { carsor_x += boardModel.boardLength; }

            // y座標
            carsor_y = (carsor_y - (int)val.y) % boardModel.boardLength;
            if (carsor_y < 0) { carsor_y += boardModel.boardLength; }

            // カーソルを動かす
            carsolView.CarsorMove(carsor_x, carsor_y, boardModel.boardLength);

        }

        // Enter/Spaceキーが押された
        protected override void OnActionStarted(float value)
        {
            // クリア時または硬直時は呼び出さない
            if (MGManager.IsClear || waitForNextTry) { return; }

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

                // りさじゅう呼び出し
                StartCoroutine(rView.Animation());

                // クリア時の演出を呼び出す
                uiManager.GameClear();

            }
            else
            {
                // 間違っていた場合は少し時間を空けてから盤面をもとに戻す
                waitForNextTry = true;
                yield return new WaitForSeconds(0.5f);
                waitForNextTry = false;
                boardModel.Flip(carsor_x, carsor_y);

            }

            // なんでもない
            yield return null;

        }

    }
}