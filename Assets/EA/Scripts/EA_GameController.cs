using System.Collections;
using UnityEngine;

namespace EA
{
    public class EA_GameContorller : MiniGameBase
    {
        /// <summary>
        /// Board関連
        /// </summary>
        private EA_BoardModel boardModel;  // 盤面のモデル

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
        private GameObject[,] cells;  // セルをまとめて保持する
        

        /// <summary>
        /// Carsol関連
        /// </summary>
        public EA_CarsorView carsolView;  // カーソルの見た目
        public GameObject carsol;  // カーソルオブジェクト
        private int carsor_x = 0;
        private int carsor_y = 0;

        /// <summary>
        /// UI関連
        /// </summary>
        public EA_UIManager uiManager;  // UIManager


        // ゲーム開始時に呼ばれる
        public override void OnGameStart()
        {
            // 盤面のモデルを作成する
            boardModel = new EA_BoardModel();

            // データセットの中から一つ抽出する
            bool[] data = ChooseData().data;

            // 盤面を作成する
            boardModel.SetBoard(data);

            // セルを生成する
            int l = boardModel.boardLength;
            cells = new GameObject[l, l];

            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < l; j++)
                {
                    // currentBoard
                    GameObject cell = Instantiate(cellPrefab);  // プレファブを生成する

                    Transform transform = cell.GetComponent<Transform>();  
                    transform.position = new Vector3(-6.5f + ((8 - l) / 2 + j), 3.5f - ((8 - l) / 2 + i), -1);

                    EA_CellView cellView = cell.GetComponent<EA_CellView>();
                    cellView.Flip(boardModel.GetCell(j, i));
                    cells[i, j] = cell;

                    // targetBoard
                    GameObject cell2 = Instantiate(cellPrefab);  // プレファブを生成する
                    Transform transform2 = cell2.GetComponent<Transform>();  
                    transform2.position = new Vector3(3.25f + (((8 - l) / 2 + j) * 0.5f), -0.25f - (((8 - l) / 2 + i)) * 0.5f, -1);
                    transform2.localScale = new Vector3(0.5f, 0.5f, 1);

                    EA_CellView cellView2 = cell2.GetComponent<EA_CellView>();
                    cellView2.Flip(boardModel.GetTargetCell(j, i));

                }
            }

            // カーソルを移動する
            carsolView.CarsorMove(0, 0, l);

            // ミニゲームを開始する
            MGManager.Load();

        }

        // 上下左右キーが押されている
        protected override void OnMovePerformed(Vector2 value)
        {
            Vector2 val = convert_stick_to_dir(value);

            // x座標
            carsor_x = (carsor_x + (int)val.x) % boardModel.boardLength;
            if (carsor_x < 0) { carsor_x += boardModel.boardLength; }
            Debug.Log(carsor_x);

            // y座標
            carsor_y = (carsor_y - (int)val.y) % boardModel.boardLength;
            if (carsor_y < 0) { carsor_y += boardModel.boardLength; }
            Debug.Log(carsor_y);

            // カーソルを動かす
            carsolView.CarsorMove(carsor_x, carsor_y, boardModel.boardLength);

        }

        // Enter/Spaceキーが押された
        protected override void OnActionStarted(float value)
        {
            // 盤面でひっくり返す処理を行う
            boardModel.Flip(carsor_x, carsor_y);

            // 見た目の処理
            for (int i = carsor_y - 1; i <= carsor_y + 1; i++)
            {
                // iがyの範囲からそれている場合
                if (i < 0 || boardModel.boardLength <= i) { continue; }

                for (int j = carsor_x - 1; j <= carsor_x + 1; j++)
                {
                    // jがxの範囲からそれている場合
                    if (j < 0 || boardModel.boardLength <= j) { continue; }

                    // ひっくり返す
                    bool side = boardModel.GetCell(j, i);
                    cells[i, j].GetComponent<EA_CellView>().Flip(side);

                }
            }
        }

        // デモプレイ(1回だけ呼ばれる)
        private IEnumerator DemoPlay()
        {
            yield return null;
        }

        // メインの部分
        private IEnumerator MainCoroutine()
        {
            yield return null;
        }

    }
}