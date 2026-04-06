using UnityEngine;

namespace EA
{
    public class EA_Board
    {
        public GameObject[,] currentCells;
        public GameObject[,] targetCells;
        public GameObject[,] demoCells;

        // コンストラクター
        public EA_Board(int size)
        {
            currentCells = new GameObject[size, size];
            targetCells = new GameObject[size, size];
            demoCells = new GameObject[4, 4];
        }
    }
}
