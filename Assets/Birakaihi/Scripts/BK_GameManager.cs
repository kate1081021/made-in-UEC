using System.Collections.Generic;
using UnityEngine;

namespace BK
{
    public class BK_GameManager : MiniGameBase
    {
        public GameObject EnemyLeft; // Imageがついたプレハブ
        public GameObject EnemyRight; // Imageがついたプレハブ
        public Transform canvasTransform; // CanvasのTransform
        public List<BK_LocationData> dataset_easy;  // データのまとめ
        private List<float> y_list;
        private List<GameObject> EnemyList;  // 敵を入れておくリスト

        // 最初に呼び出し
        public override void OnGameStart()
        {
            // Enemy呼び出し
            y_list = new List<float>
            {
                204.96f, 125.88f, 46.846f, -32.192f, -111.2308f, -190.2692f, -269.3077f
            };

            List<int> data = dataset_easy[Random.Range(0, dataset_easy.Count)].data;
            for (int i = 0; i < data.Count; i++)
            {
                int rand = Random.Range(0, 2);
                Vector2 position;
                GameObject newImage;
                float x = 250 + 200*Random.Range(1, 4);

                // 左
                if (rand == 0)
                {
                    position = new Vector2(-x, y_list[data[i]]);
                    newImage = Instantiate(EnemyLeft, canvasTransform);
                }
                // 右
                else
                {
                    position = new Vector2(x, y_list[data[i]]);
                    newImage = Instantiate(EnemyRight, canvasTransform);
                }

                RectTransform pos = newImage.GetComponent<RectTransform>();
                pos.anchoredPosition = position;
                // EnemyList.Add(newImage);
                
            }
            MGManager.Load();  // 最初に呼び出し
        }
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
