using UnityEngine;

namespace BOSS
{
    public class BOSS_GhostObstacle : MonoBehaviour
    {
        [Header("お化け・視界の設定")]
        public GameObject darknessObject;      
        public float startVisionScale = 20f;   
        public float minVisionScale = 3f;      
        public float shrinkSpeed = 5f;         
        public float expandSpeed = 10f;        
        public float ghostLifeTime = 5f;       

        [Header("お化けのフェード（透明度）設定")]
        public float fadeInSpeed = 2f;         // ★追加：出現時に濃くなるスピード
        public float fadeOutSpeed = 2f;        // ★追加：消失時に薄くなるスピード

        private float currentShrinkSpeed;
        private float currentExpandSpeed;      
        private float currentFadeInSpeed;      // ★追加
        private float currentFadeOutSpeed;     // ★追加
        private float currentAlpha = 0f;       // ★追加：現在の透明度（0＝透明、1＝不透明）

        private bool isGhostActive = false;
        private bool isFadingOut = false;      
        private float timer = 0f;

        private Transform followTarget;
        private SpriteRenderer[] allSprites;   // ★追加：お化けの画像パーツ一覧

        public void Init(Transform target = null)
        {
            // timeScaleの計算
            currentShrinkSpeed = shrinkSpeed * Time.timeScale;
            currentExpandSpeed = expandSpeed * Time.timeScale; 
            currentFadeInSpeed = fadeInSpeed * Time.timeScale;
            currentFadeOutSpeed = fadeOutSpeed * Time.timeScale;

            followTarget = target; 
            
            if (followTarget != null)
            {
                transform.position = followTarget.position;
            }

            // ★追加：プレハブに含まれるすべての画像（SpriteRenderer）を取得
            allSprites = GetComponentsInChildren<SpriteRenderer>();
            
            // ★追加：最初は完全に透明（Alpha = 0）にしておく
            currentAlpha = 0f;
            SetGhostAlpha(currentAlpha);

            if (darknessObject != null)
            {
                darknessObject.SetActive(true);
                darknessObject.transform.localScale = Vector3.one * startVisionScale;
            }
            
            isGhostActive = true;
            isFadingOut = false; 
            timer = ghostLifeTime;
        }

        void Update()
        {
            if (!isGhostActive) return;

            if (followTarget != null)
            {
                transform.position = followTarget.position;
            }

            // ① 視界が狭まる＆お化けがフワッと現れる処理
            if (!isFadingOut)
            {
                // ★追加：お化けの画像を徐々に濃くする（フェードイン）
                if (currentAlpha < 1f)
                {
                    currentAlpha += currentFadeInSpeed * Time.unscaledDeltaTime;
                    if (currentAlpha > 1f) currentAlpha = 1f; // 1（完全な不透明）でストップ
                    SetGhostAlpha(currentAlpha);
                }

                if (darknessObject != null && darknessObject.transform.localScale.x > minVisionScale)
                {
                    darknessObject.transform.localScale -= Vector3.one * currentShrinkSpeed * Time.unscaledDeltaTime;

                    if (darknessObject.transform.localScale.x < minVisionScale)
                    {
                        darknessObject.transform.localScale = Vector3.one * minVisionScale;
                    }
                }

                timer -= Time.deltaTime; 
                if (timer <= 0f)
                {
                    isFadingOut = true; 
                }
            }
            // ② 視界が徐々に晴れる＆お化けがフワッと消える処理
            else
            {
                // ★追加：お化けの画像を徐々に薄くする（フェードアウト）
                if (currentAlpha > 0f)
                {
                    currentAlpha -= currentFadeOutSpeed * Time.unscaledDeltaTime;
                    if (currentAlpha < 0f) currentAlpha = 0f; // 0（完全な透明）でストップ
                    SetGhostAlpha(currentAlpha);
                }

                if (darknessObject != null)
                {
                    darknessObject.transform.localScale += Vector3.one * currentExpandSpeed * Time.unscaledDeltaTime;

                    if (darknessObject.transform.localScale.x >= startVisionScale)
                    {
                        Vanish();
                    }
                }
                else
                {
                    Vanish();
                }
            }
        }

        // ★追加：お化けの画像の透明度（Alpha）を変更する専用の関数
        private void SetGhostAlpha(float alpha)
        {
            if (allSprites == null) return;

            foreach (var sr in allSprites)
            {
                // 視界を狭くするための「黒い穴の画像」はフェードさせない（大きさで表現するため）
                if (darknessObject != null && sr.gameObject == darknessObject)
                {
                    continue; // darknessObjectならスキップして次へ
                }

                // 画像の色（RGBA）を取得して、A（透明度）だけを書き換える
                Color color = sr.color;
                color.a = alpha;
                sr.color = color;
            }
        }

        private void Vanish()
        {
            isGhostActive = false;
            Destroy(gameObject); 
        }
    }
}