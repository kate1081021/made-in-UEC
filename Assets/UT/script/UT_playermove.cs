using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // UI操作に必要

namespace UT
{

    public class UT_playermove : MiniGameBase
    {
        [SerializeField] private float movespeed;
        private Rigidbody2D rb;
        [Tooltip("被弾時に受けるダメージ")]
        public int damage;
        [Tooltip("無敵時間")]
        public float duration;
        bool Invincible = false;
        public float timelimit = 15;

        [SerializeField] Image I0;
        [SerializeField] Image I1;
        [SerializeField] Sprite S0;
        [SerializeField] Sprite S1;
        [SerializeField] Sprite S2;
        [SerializeField] Sprite S3;
        [SerializeField] Sprite S4;
        [SerializeField] Sprite S5;
        [SerializeField] Sprite S6;
        [SerializeField] Sprite S7;
        [SerializeField] Sprite S8;
        [SerializeField] Sprite S9;
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject die_back;
        [Tooltip("最大HP")]
        public int maxHp = 68;
        [Tooltip("現在のHP")]
        public int initialHp;
        private int currentHp;
        public GameObject generator;
        public Slider HPbar;
        public Sprite heart;
        public Sprite broken_heart;
        public GameObject heart_piece;
        bool alive;
        public override void OnGameStart()
        {
            //MGManager.TestPlay(100);
            MGManager.Load();
            rb = GetComponent<Rigidbody2D>();
            alive = true;
            canvas.gameObject.SetActive(true);
            die_back.gameObject.SetActive(false);
            gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0, 0, 1);
            currentHp = initialHp; 
            HPbar.maxValue = maxHp; // スライダーの最大値を設定
            HPbar.value = currentHp; // 現在のHPを反映
            StartCoroutine(wait());
        }
        public override void OnGameEnd() 
        {
            Destroy(generator);
            DestroyAllWithTag("bullet");
            gameObject.GetComponent<SpriteRenderer>().sprite = heart;
            if (currentHp > 0)
            {
                MGManager.ClearGame();
            }
        }

        public void DestroyAllWithTag(string tagName)
        {
            // タグに一致する全オブジェクトを取得
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tagName);
            // すべて削除
            foreach (GameObject obj in objects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }

        IEnumerator wait()
        {
            yield return new WaitForSeconds(timelimit / Time.timeScale);
            OnGameEnd();
        }
        void FixedUpdate()
        {
            Vector2 pos = Move.ReadValue<Vector2>() * movespeed * Time.timeScale * Time.fixedDeltaTime;
            if(alive)rb.MovePosition(rb.position + pos);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("bullet") && !Invincible && alive)
            {
                currentHp -= damage;
                if (currentHp < 0) currentHp = 0;
                HPbar.value = currentHp;
                I0.sprite = (currentHp % 10 == 0) ? S0 :
                            (currentHp % 10 == 1) ? S1 :
                            (currentHp % 10 == 2) ? S2 :
                            (currentHp % 10 == 3) ? S3 :
                            (currentHp % 10 == 4) ? S4 :
                            (currentHp % 10 == 5) ? S5 :
                            (currentHp % 10 == 6) ? S6 :
                            (currentHp % 10 == 7) ? S7 :
                            (currentHp % 10 == 8) ? S8 : S9;
                I1.sprite = (currentHp / 10 == 0) ? S0 :
                            (currentHp / 10 == 1) ? S1 :
                            (currentHp / 10 == 2) ? S2 :
                            (currentHp / 10 == 3) ? S3 :
                            (currentHp / 10 == 4) ? S4 :
                            (currentHp / 10 == 5) ? S5 : S6;

                Debug.Log("HP = " + currentHp);
                if (currentHp <= 0)
                {
                    Debug.Log("failure");
                    canvas.gameObject.SetActive(false);
                    die_back.gameObject.SetActive(true);
                    alive = false;
                    gameObject.GetComponent<SpriteRenderer>().sprite = broken_heart;
                    StartCoroutine(heart_break());
                }
                else
                {
                    StartCoroutine(muteki());
                }
            }
        }
        IEnumerator muteki()
        {
            Invincible = true;
            for (int i = 0; i < 4; i++)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0, 0, 0.3f);
                yield return new WaitForSeconds(duration / (8* Time.timeScale));
                gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0, 0, 1);
                yield return new WaitForSeconds(duration / (8 * Time.timeScale));
            }
            Invincible = false;
        }

        [SerializeField] float g;
        [SerializeField] Vector3 v1;
        [SerializeField] Vector3 v2;
        [SerializeField] Vector3 v3;
        [SerializeField] Vector3 v4;
        

        IEnumerator heart_break()
        {
            yield return new WaitForSeconds(1f);
            gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 0, 0, 0);
            GameObject piece1 = Instantiate(heart_piece, gameObject.transform.position, Quaternion.identity);
            GameObject piece2 = Instantiate(heart_piece, gameObject.transform.position, Quaternion.identity);
            GameObject piece3 = Instantiate(heart_piece, gameObject.transform.position, Quaternion.identity);
            GameObject piece4 = Instantiate(heart_piece, gameObject.transform.position, Quaternion.identity);
            float downV = 0;
            while (true)
            {
                downV -= g * Time.deltaTime * Time.timeScale;
                piece1.transform.position += (v1 + new Vector3(0, downV, 0)) * Time.deltaTime * Time.timeScale;
                piece1.transform.rotation *= Quaternion.Euler(0, 0, 540 * Time.deltaTime * Time.timeScale);
                piece2.transform.position += (v2 + new Vector3(0, downV, 0)) * Time.deltaTime * Time.timeScale;
                piece2.transform.rotation *= Quaternion.Euler(0, 0, 700 * Time.deltaTime * Time.timeScale);
                piece3.transform.position += (v3 + new Vector3(0, downV, 0)) * Time.deltaTime * Time.timeScale;
                piece3.transform.rotation *= Quaternion.Euler(0, 0, 900 * Time.deltaTime * Time.timeScale);
                piece4.transform.position += (v4 + new Vector3(0, downV, 0)) * Time.deltaTime * Time.timeScale;
                piece4.transform.rotation *= Quaternion.Euler(0, 0, 1140 * Time.deltaTime * Time.timeScale);
                if (piece1.transform.position.y < -5 && piece2.transform.position.y < -5 && piece3.transform.position.y < -5 && piece4.transform.position.y < -5)
                {
                    Destroy(piece1);
                    Destroy(piece2);
                    Destroy(piece3);
                    Destroy(piece4);
                    break;
                }
                yield return null;
            }
        }
    }
}
