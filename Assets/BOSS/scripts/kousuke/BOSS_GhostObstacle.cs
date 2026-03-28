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
        public float fadeInSpeed = 2f;         
        public float fadeOutSpeed = 2f;        

        [Header("周回（軌道）の設定")]
        public float orbitRadius = 2.5f;       
        public float orbitSpeed = 2.0f;        

        private float currentShrinkSpeed;
        private float currentExpandSpeed;      
        private float currentFadeInSpeed;      
        private float currentFadeOutSpeed;     
        private float currentOrbitSpeed;       
        private float currentAngle = 0f;       
        private float currentAlpha = 0f;       

        private bool isGhostActive = false;
        private bool isFadingOut = false;      
        private float timer = 0f;

        private Transform followTarget;
        private SpriteRenderer[] allSprites;   

        public void Init(Transform target = null)
        {
            currentShrinkSpeed = shrinkSpeed * Time.timeScale;
            currentExpandSpeed = expandSpeed * Time.timeScale; 
            currentFadeInSpeed = fadeInSpeed * Time.timeScale;
            currentFadeOutSpeed = fadeOutSpeed * Time.timeScale;
            currentOrbitSpeed = orbitSpeed * Time.timeScale;

            followTarget = target; 
            
            // 出現した時はランダムな角度（位置）からスタートする
            currentAngle = Random.Range(0f, Mathf.PI * 2f);

            allSprites = GetComponentsInChildren<SpriteRenderer>();
            
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

            // ターゲットの周りをグルグル回る処理
            if (followTarget != null)
            {
                currentAngle += currentOrbitSpeed * Time.unscaledDeltaTime;

                float x = followTarget.position.x + Mathf.Cos(currentAngle) * orbitRadius;
                float y = followTarget.position.y + Mathf.Sin(currentAngle) * orbitRadius;

                // お化け自体の位置を更新
                transform.position = new Vector3(x, y, transform.position.z);

                // 視界を奪う「黒い穴」はターゲットの真上に固定
                if (darknessObject != null)
                {
                    darknessObject.transform.position = new Vector3(followTarget.position.x, followTarget.position.y, darknessObject.transform.position.z);
                }
            }

            // ① 視界が狭まる＆お化けがフワッと現れる処理
            if (!isFadingOut)
            {
                if (currentAlpha < 1f)
                {
                    currentAlpha += currentFadeInSpeed * Time.unscaledDeltaTime;
                    if (currentAlpha > 1f) currentAlpha = 1f; 
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
                if (currentAlpha > 0f)
                {
                    currentAlpha -= currentFadeOutSpeed * Time.unscaledDeltaTime;
                    if (currentAlpha < 0f) currentAlpha = 0f; 
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

        private void SetGhostAlpha(float alpha)
        {
            if (allSprites == null) return;

            foreach (var sr in allSprites)
            {
                if (darknessObject != null && sr.gameObject == darknessObject)
                {
                    continue; 
                }

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