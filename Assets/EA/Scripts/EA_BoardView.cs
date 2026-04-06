using UnityEngine;

namespace EA
{
    public class EA_BoardView : MonoBehaviour
    {
        public EA_Board CreateBoard(int size, GameObject prefab, bool mode_demo)
        {
            EA_Board board = new EA_Board(size); // セルの情報を持ったクラス

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // currentBoard
                    GameObject cell = Instantiate(prefab);  // プレファブを生成する

                    Transform transform = cell.GetComponent<Transform>();  
                    transform.position = new Vector3(-6.5f + ((8 - size) / 2 + j), 3.5f - ((8 - size) / 2 + i), -1);
                    board.currentCells[i, j] = cell;

                    // targetBoard
                    GameObject cell2 = Instantiate(prefab);  // プレファブを生成する
                    Transform transform2 = cell2.GetComponent<Transform>();  
                    transform2.position = new Vector3(3.25f + ((8 - size) / 2 + j) * 0.5f, -0.25f - ((8 - size) / 2 + i) * 0.5f, -1);
                    transform2.localScale = new Vector3(0.5f, 0.5f, 1);
                    board.targetCells[i, j] = cell2;

                    // demoBoard
                    if (mode_demo && i < 4 && j < 4)
                    {
                        GameObject cell3 = Instantiate(prefab);  // プレファブを生成する
                        Transform transform3 = cell3.GetComponent<Transform>();  
                        transform3.position = new Vector3(-6.5f + (5 + j), 3.5f - (2 + i), -9);
                        board.demoCells[i, j] = cell3;
                    }

                }
            }

            return board;

        }

        // デモ版のセルを非表示にする
        public void DeleteDemoBoard(GameObject[,] demoCells)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    demoCells[i, j].SetActive(false);
                }
            }
        }
    }
}
