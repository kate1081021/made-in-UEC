using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace EA
{
    public class EA_BoardModel
    {
        /// プロパティ ///
        
        // 盤面のデータを保持する([y, x])
        private bool[,] currentBoard;

        // 正解となる盤面のデータを保持する([y, x])
        private bool[,] targetBoard;

        // 盤面の一辺の長さ
        public int boardLength;

        // 現在のセルの内容が変化したとき
        public Action <int, int, bool> OnCurrentCellChanged;

        // 正解となるセルが生成されたとき
        public Action <int, int, bool> OnTargetCellCreated;

        /// メソッド ///
        
        // 盤面を準備する
        public void SetBoard(bool[] data)
        {
            // 盤面の配列を生成
            currentBoard = new bool[boardLength, boardLength];
            targetBoard  = new bool[boardLength, boardLength];

            for (int i = 0; i < boardLength; i++)
            {
                for (int j = 0; j < boardLength; j++)
                {
                    // データをセットする
                    currentBoard[i, j] = data[i * boardLength + j];
                    OnCurrentCellChanged?.Invoke(j, i, data[i * boardLength + j]);

                    targetBoard[i, j] = data[i * boardLength + j];
                    OnTargetCellCreated?.Invoke(j, i, data[i * boardLength + j]);

                }
            }

            // どこか一か所をひっくり返す
            int x = Random.Range(0, boardLength);
            int y = Random.Range(0, boardLength);
            Flip(x, y);

        }

        // 座標を指定されたら現状の盤面の裏表を返す
        public bool GetCell(int x, int y)
        {
            return currentBoard[y, x];
        }
        
        // 座標を指定し、そのマスの表裏を反転させる
        public void FlipOne(int x, int y)
        {
            currentBoard[y, x] = !currentBoard[y, x];
            OnCurrentCellChanged?.Invoke(x, y, currentBoard[y, x]);
        }

        // ある座標を中心として、そこから3x3のマス目を反転させる。
        public void Flip(int x, int y)
        {
            for (int i = y - 1; i <= y + 1; i++)
            {
                // iがyの範囲からそれている場合
                if (i < 0 || boardLength <= i) { continue; }

                for (int j = x - 1; j <= x + 1; j++)
                {
                    // jがxの範囲からそれている場合
                    if (j < 0 || boardLength <= j) { continue; }

                    // ひっくり返す
                    FlipOne(j, i);

                }
            }
        }

        // 正解判定を行う
        public bool CheckBoard()
        {
            for (int i = 0; i < boardLength; i++)
            {
                for (int j = 0; j < boardLength; j++)
                {
                    // マス目の真偽が一致しているか確かめる
                    if (currentBoard[i, j] != targetBoard[i, j])
                    {
                        return false;
                    }
                }
            }

            // 完全に一致していると判定する
            return true;

        }
    }
}