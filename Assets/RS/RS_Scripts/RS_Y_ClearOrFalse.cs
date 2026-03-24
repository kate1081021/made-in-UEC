using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Threading;
using UnityEditor.Experimental.GraphView;

namespace RS
{
    public class RS_Spawn : MiniGameBase
    {
        [SerializeField] private AudioClip[] se;
        [SerializeField] private float limit = 6;
        private float timer = 0;
        [SerializeField] GameObject time_0;
        [SerializeField] GameObject time_board;
        [SerializeField] private SpriteRenderer sr;
        public GameObject[] prefabs;
        [Range(1, 6)] public int count = 3;

        private void gameoption()
        {
            int stage = MGManager.stage;
            if (stage <= 10)
            {
                count = 3;
            }
            else if (stage <= 20)
            {
                count = 5;
            }
            else
            {
                count = 7;
            }
        }


        private List<RS_Y_appear> spawnedScripts = new List<RS_Y_appear>();
        private int currentIndex = 0;
        private bool gameActive = false;
        private bool gameover = false;
        public RS_SpritesManager spm;

        [SerializeField]
        private InputActionReference arrow;
        private bool active_arrow = true;

        public override void OnGameStart()
        {
            MGManager.Load();
            BGMPlay(false);
            currentIndex = 0;
            gameoption();
            SpawnClones();
            gameActive = true;
        }

        private void SpawnClones()
        {
            spawnedScripts.Clear();
            for (int i = 0; i < count; i++)
            {
                float x = (i * 2.0f) - (count - 1.0f);
                Vector3 spawnPos = new Vector3(x, 1, -2);

                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
                GameObject clone = Instantiate(randomPrefab, spawnPos, Quaternion.identity);

                var script = clone.GetComponent<RS_Y_appear>();
                if (script != null)
                {
                    script.Setup();
                    spawnedScripts.Add(script);
                }
            }
        }


        void Update()
        {
            // Vector2 v = arrow.action.ReadValue<Vector2>();
            // Debug.Log(v.x);
            timer += Time.deltaTime * Time.timeScale;
            if (timer <= limit)
            {
                Vector3 tp = time_board.transform.position;
                time_board.transform.position = new Vector3(timer / limit * 4, tp.y, tp.z);
                Vector3 sc = time_board.transform.localScale;
                time_board.transform.localScale = new Vector3((1 - timer / limit) * 8, sc.y, sc.z);
            }
            else
            {
                Vector3 tp = time_board.transform.position;
                time_board.transform.position = new Vector3(4, tp.y, tp.z);
                Vector3 sc = time_board.transform.localScale;
                time_board.transform.localScale = new Vector3(0, sc.y, sc.z);
                Gameover();
            }
            sr.color = new Color(1, time_board.transform.localScale.x / 8f, time_board.transform.localScale.x / 8f);
        }

        private void OnEnable()
        {
            arrow.action.Enable();
        }

        private void OnDisable()
        {
            arrow.action.Disable();
        }

        protected override void OnMovePerformed(Vector2 value)
        {
            Vector2 dir = convert_stick_to_dir(value);
            //Debug.Log("呼ばれた！ " + value.x + " " + dir.x);
            if (active_arrow && (dir.x + dir.y == 1 || dir.x + dir.y == -1))
            {
                //Debug.Log("判定 ");
                active_arrow = false;

                if (dir.x == -1) Judge(1); // 左
                else if (dir.y == -1) Judge(2); // 下
                else if (dir.x == 1) Judge(3); // 右
                else if (dir.y == 1) Judge(4); // 上
            }
        }

        protected override void OnMoveCanceled(Vector2 value)
        {
            active_arrow = true;
        }

        private void Judge(int number)
        {
            //Debug.Log("jubge start!");
            if (!gameActive || currentIndex >= spawnedScripts.Count) return;

            RS_Y_appear currentTarget = spawnedScripts[currentIndex];

            if (currentTarget == null) return;

            if (!gameover)
            {
                if (number == currentTarget.aciton_type)
                {
                    Debug.Log($"正解！左から {currentIndex + 1} 番目を消しました");
                    Destroy(currentTarget.gameObject);
                    SEPlay("c", se[0]);
                    currentIndex++; // 次へ

                    if (currentIndex >= count)
                    {
                        Debug.Log("ゲームクリア！");
                        spm.is_clear = true;
                        spm.is_spelled = true;
                        time_board.SetActive(false);
                        time_0.SetActive(false);
                        MGManager.ClearGame();
                    }
                }
                else
                {
                    Gameover();
                }
            }
        }
        private void Gameover()
        {
            if (!spm.is_spelled)
            {
                Debug.Log("不正解！ゲームオーバー！");
                spm.is_clear = false;
                spm.is_spelled = true;
                time_board.SetActive(false);
                time_0.SetActive(false);
                gameover = true;
                SEPlay("f", se[1]);
                foreach (var script in spawnedScripts)
                {
                    if (script != null)
                    {
                        script.HideImmediately();
                    }
                }
            }
        }
    }
}