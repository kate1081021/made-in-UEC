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

        // BGM
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip startSound;

        public GameObject[] windowType;
        public List<GameObject> targetWindows = new List<GameObject>();
        public int windowCreates = 2;
        private bool ClearFlag = false;
        private bool isWindowAllClosed = false;
        private bool isBusterAllClosed = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            if (audioSource != null && startSound != null)
            {
                audioSource.clip = startSound;
                audioSource.playOnAwake = false;
                audioSource.Play();
            }

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
                    if (windowType[i].name == "WI_M_window_virus buster")
                    {
                        spawnPos = new Vector2(7f, -2.5f);
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

        public override void OnGameEnd()
        {
            if (audioSource != null) audioSource.Stop();
        }

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
                // Debug.Log("[System] 全ての指定ウィンドウが削除されました。クリア！");
                Debug.Log("time: " + Time.time);
                MGManager.ClearGame();
            }
        }

        // 生成するウィンドウを設定する関数
        private void gameSetting()
        {
            int stage = MGManager.stage;
            Debug.Log("stage: " + stage);
            
            if(stage < 16)
            {
                int random = Random.Range(0, 5);
                if (random < 1)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 < 1) { setTarget(new int[2] { 0, 1 }); }
                    else { setTarget(new int[2] { 1, 0 }); }
                } 
                else if(random < 2)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 < 1) { setTarget(new int[2] { 0, 2 }); }
                    else { setTarget(new int[2] { 2, 0 }); }
                }
                else if (random < 3)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 < 1) { setTarget(new int[2] { 1, 2 }); }
                    else { setTarget(new int[2] { 2, 1 }); }
                }
                else if (random < 4)
                {
                    setTarget(new int[1] { 3 });
                }
                else if (random < 5)
                {
                    setTarget(new int[1] { 4 });
                } 
                else
                {
                    setTarget(new int[2] { 0, 1 });
                }
            }
            else if (stage < 31)
            {
                int random = Random.Range(0, 7);
                if (random == 0)
                {
                    int random2 = Random.Range(0, 6);
                    if (random2 == 0) { setTarget(new int[3] { 0, 1, 2 }); }
                    else if (random2 == 1) { setTarget(new int[3] { 0, 2, 1 }); }
                    else if (random2 == 2) { setTarget(new int[3] { 1, 0, 2 }); }
                    else if (random2 == 3) { setTarget(new int[3] { 1, 2, 0 }); }
                    else if (random2 == 4) { setTarget(new int[3] { 2, 0, 1 }); }
                    else if (random2 == 5) { setTarget(new int[3] { 2, 1, 0 }); }
                    else { setTarget(new int[3] { 0, 1, 2 }); }
                }
                else if (random == 1)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 == 0) { setTarget(new int[2] { 0, 3 }); }
                    else if (random2 == 1) { setTarget(new int[2] { 3, 0 }); }
                    else { setTarget(new int[2] { 0, 3 }); }
                }
                else if (random == 2)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 == 0) { setTarget(new int[2] { 1, 3 }); }
                    else if (random2 == 1) { setTarget(new int[2] { 3, 1 }); }
                    else { setTarget(new int[2] { 1, 3 }); }
                }
                else if (random == 3)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 == 0) { setTarget(new int[2] { 2, 3 }); }
                    else if (random2 == 1) { setTarget(new int[2] { 3, 2 }); }
                    else { setTarget(new int[2] { 2, 3 }); }
                }
                else if (random == 4)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 == 0) { setTarget(new int[2] { 0, 4 }); }
                    else if (random2 == 1) { setTarget(new int[2] { 4, 0 }); }
                    else { setTarget(new int[2] { 0, 4 }); }
                }
                else if (random == 5)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 == 0) { setTarget(new int[2] { 1, 4 }); }
                    else if (random2 == 1) { setTarget(new int[2] { 4, 1 }); }
                    else { setTarget(new int[2] { 1, 4 }); }
                }
                else if (random == 6)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 == 0) { setTarget(new int[2] { 2, 4 }); }
                    else if (random2 == 1) { setTarget(new int[2] { 4, 2 }); }
                    else { setTarget(new int[2] { 2, 4 }); }
                }
                else
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 == 0) { setTarget(new int[2] { 2, 4 }); }
                    else if (random2 == 1) { setTarget(new int[2] { 4, 2 }); }
                    else { setTarget(new int[2] { 2, 4 }); }
                }
            }
            else
            {
                int random = Random.Range(0, 7);
                if (random == 0)
                {
                    int random2 = Random.Range(0, 6);
                    if (random2 == 0) { setTarget(new int[3] { 0, 1, 3 }); }
                    else if (random2 == 1) { setTarget(new int[3] { 0, 3, 1 }); }
                    else if (random2 == 2) { setTarget(new int[3] { 1, 0, 3 }); }
                    else if (random2 == 3) { setTarget(new int[3] { 1, 3, 0 }); }
                    else if (random2 == 4) { setTarget(new int[3] { 3, 0, 1 }); }
                    else if (random2 == 5) { setTarget(new int[3] { 3, 1, 0 }); }
                    else { setTarget(new int[3] { 0, 1, 3 }); }
                }
                else if (random == 1)
                {
                    int random2 = Random.Range(0, 6);
                    if (random2 == 0) { setTarget(new int[3] { 0, 1, 4 }); }
                    else if (random2 == 1) { setTarget(new int[3] { 0, 4, 1 }); }
                    else if (random2 == 2) { setTarget(new int[3] { 1, 0, 4 }); }
                    else if (random2 == 3) { setTarget(new int[3] { 1, 4, 0 }); }
                    else if (random2 == 4) { setTarget(new int[3] { 4, 0, 1 }); }
                    else if (random2 == 5) { setTarget(new int[3] { 4, 1, 0 }); }
                    else { setTarget(new int[3] { 0, 1, 4 }); }
                }
                else if (random == 2)
                {
                    int random2 = Random.Range(0, 6);
                    if (random2 == 0) { setTarget(new int[3] { 0, 2, 3 }); }
                    else if (random2 == 1) { setTarget(new int[3] { 0, 3, 2 }); }
                    else if (random2 == 2) { setTarget(new int[3] { 2, 0, 3 }); }
                    else if (random2 == 3) { setTarget(new int[3] { 2, 3, 0 }); }
                    else if (random2 == 4) { setTarget(new int[3] { 3, 0, 2 }); }
                    else if (random2 == 5) { setTarget(new int[3] { 3, 2, 0 }); }
                    else { setTarget(new int[3] { 0, 2, 3 }); }
                }
                else if (random == 3)
                {
                    int random2 = Random.Range(0, 6);
                    if (random2 == 0) { setTarget(new int[3] { 0, 2, 4 }); }
                    else if (random2 == 1) { setTarget(new int[3] { 0, 4, 2 }); }
                    else if (random2 == 2) { setTarget(new int[3] { 2, 0, 4 }); }
                    else if (random2 == 3) { setTarget(new int[3] { 2, 4, 0 }); }
                    else if (random2 == 4) { setTarget(new int[3] { 4, 0, 2 }); }
                    else if (random2 == 5) { setTarget(new int[3] { 4, 2, 0 }); }
                    else { setTarget(new int[3] { 0, 2, 4 }); }
                }
                else if (random == 4)
                {
                    int random2 = Random.Range(0, 6);
                    if (random2 == 0) { setTarget(new int[3] { 1, 2, 3 }); }
                    else if (random2 == 1) { setTarget(new int[3] { 1, 3, 2 }); }
                    else if (random2 == 2) { setTarget(new int[3] { 2, 1, 3 }); }
                    else if (random2 == 3) { setTarget(new int[3] { 2, 3, 1 }); }
                    else if (random2 == 4) { setTarget(new int[3] { 3, 1, 2 }); }
                    else if (random2 == 5) { setTarget(new int[3] { 3, 2, 1 }); }
                    else { setTarget(new int[3] { 1, 2, 3 }); }
                }
                else if (random == 5)
                {
                    int random2 = Random.Range(0, 6);
                    if (random2 == 0) { setTarget(new int[3] { 1, 2, 4 }); }
                    else if (random2 == 1) { setTarget(new int[3] { 1, 4, 2 }); }
                    else if (random2 == 2) { setTarget(new int[3] { 2, 1, 4 }); }
                    else if (random2 == 3) { setTarget(new int[3] { 2, 4, 1 }); }
                    else if (random2 == 4) { setTarget(new int[3] { 4, 1, 2 }); }
                    else if (random2 == 5) { setTarget(new int[3] { 4, 2, 1 }); }
                    else { setTarget(new int[3] { 1, 2, 4 }); }
                }
                else if (random == 6)
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 == 0) { setTarget(new int[2] { 3, 4 }); }
                    else if (random2 == 1) { setTarget(new int[2] { 4, 3 }); }
                    else { setTarget(new int[2] { 3, 4 }); }
                }
                else
                {
                    int random2 = Random.Range(0, 2);
                    if (random2 == 0) { setTarget(new int[2] { 3, 4 }); }
                    else if (random2 == 1) { setTarget(new int[2] { 4, 3 }); }
                    else { setTarget(new int[2] { 3, 4 }); }
                }
            }
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
                if (windowType[i].name == "WI_M_window_virus buster")
                {
                    spawnPos = new Vector2(7f, -2.5f);
                }


                GameObject newWindow = Instantiate(windowType[i], spawnPos, Quaternion.identity);

                // レイヤー設定
                SortingGroup sg = newWindow.GetComponent<SortingGroup>();
                if (sg != null)
                {
                    sg.sortingOrder = sortNum;
                    sortNum++;
                }
                targetWindows.Add(newWindow);

            }
        }
        
    }
}