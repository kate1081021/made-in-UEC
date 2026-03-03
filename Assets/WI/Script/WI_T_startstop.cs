using UnityEngine;
using System.Collections.Generic; //生成順用
using UnityEngine.Rendering;

namespace WI
{
    public class WI_T_startstop : MiniGameBase
    {
        // 1以上であればTestPlay(TestStage)でゲーム実行
        [SerializeField] private int TestStage = 0;

        // trueであれば全てのウィンドウ表示
        [SerializeField] private bool allInstanciate = false;

        public GameObject[] windowType;
        public List<GameObject> targetWindows = new List<GameObject>();
        public int windowCreates = 2;
        private bool ClearFlag = false;
        private bool isWindowAllClosed = false;
        private bool isBusterAllClosed = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            int sortNum = 0;

            BGMPlay(true);

            if (TestStage > 0)
            {
                MGManager.TestPlay(TestStage);
            }
            else
            {
                MGManager.Load();
            }

            ClearFlag = false;
            targetWindows.Clear();

            if (allInstanciate)
            {
                for (int i = 0; i < windowCreates; i++)
                {
                    Vector2 spawnPos;

                    float randomX = Random.Range(-3.3f, 3.3f);
                    float randomY = Random.Range(-0.15f, 0.15f);

                    spawnPos = new Vector2(randomX, randomY);
                    if (windowType[i].name == "WI_I_window_virus buster")
                    {
                        spawnPos = new Vector2(7f, -2.5f);
                    }


                    GameObject newWindow = Instantiate(windowType[i], spawnPos, Quaternion.identity);

                    // レイヤー設定
                    SortingGroup sg = newWindow.GetComponent<SortingGroup>();
                    if (sg != null)
                    {
                        sg.sortingOrder = sortNum;
                        if (windowType[i].name.Contains("WI_M_popup") || windowType[i].name.Contains("WI_T_ad"))
                        {
                            sortNum += 2;
                        }
                        else
                        {
                            sortNum++;
                        }
                    }
                    targetWindows.Add(newWindow);

                }
            }
            else
            {
                gameSetting();
            }
            
        }

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
            
                    if (win.name.Contains("WI_I_window_virus buster"))
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
                // Debug.Log("[System] 全ての指定ウィンドウが削除されました。クリア！");
                Debug.Log("time: " + Time.time);
                MGManager.ClearGame();
            }
        }

        // 生成するウィンドウを設定する関数
        private void gameSetting()
        {
            int stage = TestStage;
            Debug.Log("stage: " + stage);
            int number1 = 0, number2 = 0;
            int random = Random.Range(0, 2);

            if (stage <= 15)
            {
                if (random == 0) { number1 = 2; }
                else { number2 = 1; }
            }
            else if (stage <= 30)
            {
                if (random == 0) { number1 = 3; }
                else { number1 = 1; number2 = 1; }
            }
            else
            {
                if (random == 0) { number1 = 2; number2 = 1; }
                else { number2 = 2; }
            }

            pattern(number1, number2);
        }

        // 指定したidのウィンドウを全て生成する関数
        private void setTarget(int[] id)
        {
            int sortNum = 0;
            foreach(int i in id)
            {
                Vector2 spawnPos;

                float randomX = Random.Range(-3.3f, 3.3f);
                float randomY = Random.Range(-0.15f, 0.15f);

                Debug.Log("windowType: " + windowType[i]);

                spawnPos = new Vector2(randomX, randomY);
                if (windowType[i].name == "WI_I_window_virus buster")
                {
                    spawnPos = new Vector2(7f, -2.5f);
                }


                GameObject newWindow = Instantiate(windowType[i], spawnPos, Quaternion.identity);

                // レイヤー設定
                SortingGroup sg = newWindow.GetComponent<SortingGroup>();
                if (sg != null)
                {
                    sg.sortingOrder = sortNum;
                    if (windowType[i].name.Contains("WI_M_popup") || windowType[i].name.Contains("WI_T_ad"))
                    {
                        Debug.Log("skip");
                        sortNum += 2;
                    }
                    else
                    {
                        sortNum++;
                    }
                }
                targetWindows.Add(newWindow);

            }
        }

        private int[] GetnCrPattern(int[] baseArray, int r)
        {
            int[] result = new int[r];
            int n = baseArray.Length;
            for (int i = 0; i < r; i++)
            {
                int random = Random.Range(0, n - i);
                int tmp = baseArray[random];
                baseArray[random] = baseArray[n - 1 - i];
                baseArray[n - 1 - i] = tmp;
                result[i] = tmp;
            }
            return result;
        }

        private int[] GetRandomPattern(int[] baseArray)
        {
            int[] result = (int[])baseArray.Clone();
            for (int i = result.Length - 1; i > 0; i--)
            {
                int r = Random.Range(0, i + 1);
                int tmp = result[i];
                result[i] = result[r];
                result[r] = tmp;
            }
            return result;
        }

        private void pattern(int number1, int number2)
        {
            int[] result = new int[number1 + number2];
            int[] result1 = GetnCrPattern(new int[] {0, 1, 2, 3}, number1);
            int[] result2 = GetnCrPattern(new int[] {4, 5, 6}, number2);
            result1.CopyTo(result, 0);
            result2.CopyTo(result, number1);

            setTarget(GetRandomPattern(result));
        }
    }
}