using UnityEngine;

namespace WI 
{

    public class WI_T_startstop : MiniGameBase 
    {

        // private WI_M_buttonManager windowclose;
        public GameObject[] windowType;

        private GameObject[] targetWindows;
        // public static WI_T_startstop instance;
        private bool ClearFlag = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            MGManager.Load();

            ClearFlag = false;
            int WindowCreate = 3;
            targetWindows = new GameObject[WindowCreate];

            for (int i = 0; i < WindowCreate; i++)
            {
                float randomX = Random.Range(-3.3f, 3.3f);
                float randomY = Random.Range(-0.15f, 0.15f);

                Vector2 spawnPos = new Vector2(randomX, randomY);

                GameObject newWindow = Instantiate(windowType[i], spawnPos, Quaternion.identity);

                targetWindows[i] = newWindow;
            }
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
