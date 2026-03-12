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
                    GameObject cell = Instantiate(cellPrefab);  // プレファブを生成する

                    Transform transform = cell.GetComponent<Transform>();  
                    transform.position = new Vector3(-6.5f + ((8 - l) / 2 + j), 3.5f - ((8 - l) / 2 + i), -1);

                    EA_CellView cellView = cell.GetComponent<EA_CellView>();
                    cellView.Flip(boardModel.GetCell(j, i));
                    cells[i, j] = cell;
                }
            }

            // ミニゲームを開始する
            MGManager.Load();

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