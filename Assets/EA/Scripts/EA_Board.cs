using UnityEngine;

namespace EA
{
    public class EA_Board
    {
        public GameObject[,] currentCells;
        public GameObject[,] targetCells;

        // コンストラクター
        public EA_Board(int size)
        {
            currentCells = new GameObject[size, size];
            targetCells = new GameObject[size, size];
        }
    }
}
