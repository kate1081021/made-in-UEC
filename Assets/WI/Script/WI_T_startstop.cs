using UnityEngine;
using System.Collections.Generic; //生成順用

namespace WI 
{

    public class WI_T_startstop : MiniGameBase 
    {

        public GameObject[] windowType;
        public List<GameObject> targetWindows = new List<GameObject>();
        private bool ClearFlag = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            MGManager.Load();

            ClearFlag = false;
            int WindowCreate = 3;
            targetWindows.Clear();

            for (int i = 0; i < WindowCreate; i++)
            {
                float randomX = Random.Range(-3.3f, 3.3f);
                float randomY = Random.Range(-0.15f, 0.15f);

                Vector2 spawnPos = new Vector2(randomX, randomY);

                GameObject newWindow = Instantiate(windowType[i], spawnPos, Quaternion.identity);

                // レイヤー設定
                SpriteRenderer sr = newWindow.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = i;
                }

                targetWindows.Add(newWindow);
            }
        }

        // インデックスを返す関数
        // public int GetWindowIndex(GameObject windowObj)
        // {
        //     return targetWindows.IndexOf(windowObj);
        // }

        // レイヤー番号を返す関数
        public int GetWindowRayer(GameObject windowObj)
        {
            SpriteRenderer sr = windowObj.GetComponent<SpriteRenderer>();
            if (sr != null) return sr.sortingOrder;
            // エラー回避
            return -1;
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void Update()
        {
            if (ClearFlag) return;
            int windowCount;
            int activeCount = 0;

            foreach (GameObject win in targetWindows)
            {
                if (win != null && win.activeSelf == true)
                {
                    activeCount++;
                }
            }

            windowCount = activeCount;

            if (windowCount <= 0)
            {
                ClearFlag = true;
                MGManager.ClearGame();
            }
        }
    }
}
