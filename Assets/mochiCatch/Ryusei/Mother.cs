using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace catchMochi
{
    public class Mother : MonoBehaviour
    {
        public GameObject mother;
        private Image image;
        public GameObject girl;
        public Animator shoji_anim;  // 障子のアニメーションを管理
        public bool isMotherSeeing = false;
        public int maximum;
        public float multiple;
        public float waitTimeMultiple;
        public bool finish = false;
        public Sprite okannNormal;
        public Sprite okannAngry;
        private int mochi;
        private int speedUp;
        private GameManager gameManager;
        // C#の古いバージョンでも動くようにListの初期化を修正
        private List<float[]> patterns = new List<float[]> {
            new float[] { 5.0f, 2.0f, 1.0f },  // [何もない時間, 揺れている時間, 開いている時間]
            new float[] { 3.0f, 2.5f, 1.5f },
            new float[] { 4.0f, 1.5f, 1.0f },
            new float[] { 1.0f, 1.0f, 1.0f },
            new float[] { 2.0f, 3.0f, 1.0f }
        };

        void OnEnable()
        {
            image = mother.GetComponent<Image>();
            gameManager = Object.FindAnyObjectByType<GameManager>();
            this.image.sprite = okannNormal;
            if (shoji_anim != null)
            {
                StartCoroutine(RandomMotherAppear());
            }
        }

        public IEnumerator RandomMotherAppear()
        {
            while (gameManager.game_in_progress)
            {

                // 1. パターンをランダムに決定する
                // Count は大文字、Random.Rangeの最大値は「含まれない」のでこれでOK
                int pattern_idx = Random.Range(0, patterns.Count);
                mochi = girl.GetComponent<mochiCatch>().ateMochi;
                speedUp = Mathf.Max(mochi, maximum);
                float[] times = patterns[pattern_idx];
                float factor = 0.5f*(1 - (float)Mathf.Exp(-multiple*speedUp));

                float idleTime = times[0] * Random.Range(0.8f, 1.2f) * (1 - factor) * waitTimeMultiple;
                float shakeTime = times[1] * Random.Range(0.8f, 1.2f) * (1 - factor);
                float openTime = times[2] * Random.Range(0.8f, 1.2f) * (1 - factor);

                // 2. 何もない時間（待機）
                yield return new WaitForSeconds(idleTime);

                // 3. 障子がガタガタ揺れる
                shoji_anim.SetBool("isShaking", true);
                yield return new WaitForSeconds(shakeTime);
                
                // 4. ここで分岐する(60%で全開、20%で半開、20%で何も起こらない)
                int rand = Random.Range(0, 100);

                // 全開
                if (rand < 70)
                {
                    rand = Random.Range(0, 100);
                    // 普通に出てくる
                    if (rand < 75)
                    {
                        image.enabled = true;
                        shoji_anim.SetBool("open", true);
                        yield return new WaitForSeconds(0.1f);
                        isMotherSeeing = true;
                    }
                    // 開くだけ
                    else
                    {
                        image.enabled = false;
                        shoji_anim.SetBool("open", true);
                        yield return new WaitForSeconds(0.1f);
                        isMotherSeeing = false;
                    }
                
                    yield return new WaitForSeconds(openTime);
                    if (finish) { this.image.sprite = okannAngry; finish = false; yield break; }
                    shoji_anim.SetBool("open", false);
                    isMotherSeeing = false;

                    // ふすまを閉めるときのSE
                }
                // 半開
                else if (rand < 80)
                {
                    shoji_anim.SetTrigger("open_little");
                    yield return new WaitForSeconds(0.2f);
                }

                // 揺れているというフラグを折る
                shoji_anim.SetBool("isShaking", false);
            }
        }
    }
}