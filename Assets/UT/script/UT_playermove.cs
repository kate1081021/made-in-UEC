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
        public float timelimit = 10;

        public Image I0;
        public Image I1;
        public Sprite S0;
        public Sprite S1;
        public Sprite S2;
        public Sprite S3;
        public Sprite S4;
        public Sprite S5;
        public Sprite S6;
        public Sprite S7;
        public Sprite S8;
        public Sprite S9;

        [Tooltip("最大HP")]
        public int maxHp = 68;
        [Tooltip("現在のHP")]
        public int initialHp;
        private int currentHp;
        public GameObject generator; 
        public Slider HPbar;
        public override void OnGameStart()
        {
            MGManager.Load();
            rb = GetComponent<Rigidbody2D>();
            currentHp = initialHp; 
            HPbar.maxValue = maxHp; // スライダーの最大値を設定
            HPbar.value = currentHp; // 現在のHPを反映
            StartCoroutine(wait());
        }
        public override void OnGameEnd() 
        {
            Destroy(generator);
            DestroyAllWithTag("bullet");
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
            yield return new WaitForSeconds(timelimit);
            OnGameEnd();
        }
        void FixedUpdate()
        {
            Vector2 pos = Move.ReadValue<Vector2>() * movespeed * Time.timeScale * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + pos);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("bullet") && !Invincible)
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
                StartCoroutine(muteki());
                if (currentHp <= 0) Debug.Log("failure");
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
    }
}
