using UnityEngine;
using System.Collections.Generic; //生成順用
using UnityEngine.Rendering;

namespace WI
{
    public class WI_T_startstop : MiniGameBase
    {

        public GameObject[] windowType;
        public List<GameObject> targetWindows = new List<GameObject>();
        private bool ClearFlag = false;
        private bool isWindowAllClosed = false;
        private bool isBusterAllClosed = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            // MGManager.TestPlay(1);
            MGManager.Load();

            ClearFlag = false;
            targetWindows.Clear();

            for (int i = 0; i < windowType.Length; i++)
            {
                Vector2 spawnPos = Vector2.zero;

                float randomX = Random.Range(-3.3f, 3.3f);
                float randomY = Random.Range(-0.15f, 0.15f);

                spawnPos = new Vector2(randomX, randomY);
                if (windowType[i].name == "WI_M_window_virus buster")
                {
                    spawnPos = new Vector2(7.4f,-2f);
                }


                GameObject newWindow = Instantiate(windowType[i], spawnPos, Quaternion.identity);

                // レイヤー設定
                SortingGroup sg = newWindow.GetComponent<SortingGroup>();
                if (sg != null)
                {
                    sg.sortingOrder = i;
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
            if (ClearFlag || targetWindows.Count == 0) return;

            int currentWindowCount = 0;
            int currentBusterCount = 0;

            foreach (GameObject win in targetWindows)
            {
                if (win != null)
                {
                    if (win.name.Contains("indow(Clone)") && win.activeInHierarchy)
                    {
                        currentWindowCount++;
                    }
            
                    if (win.name.Contains("WI_M_window_virus buster"))
                    {
                        currentBusterCount++;
                    }
                }
            }

            if (currentWindowCount <= 0) isWindowAllClosed = true;

            if (currentBusterCount <= 0) isBusterAllClosed = true;

            if (isWindowAllClosed && isBusterAllClosed)
            {
                ClearFlag = true;
                Debug.Log("[System] 全ての指定ウィンドウが削除されました。クリア！");
                MGManager.ClearGame();
            }
        }
        
    }
}