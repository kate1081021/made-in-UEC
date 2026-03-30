using UnityEngine;

namespace BOSS
{
    public class BOSS_WindController : MonoBehaviour
    {
        [Header("風の設定")]
        public float windStrength = 5.0f;
        
        [Header("扇風機の登場設定")]
        public float spawnOffset = 3.0f;
        public float insideOffset = 1.0f;
        [Tooltip("扇風機の移動スピード")]
        public float fanMoveSpeed = 5.0f;

        [Header("演出オブジェクト")]
        public GameObject fanObject;
        public ParticleSystem windParticle;

        private Transform playerTarget;
        private bool isBlowing = false;
        private bool isFromRight = true;
        
        // 移動アニメーション用
        private float targetFanX;  // 画面内の停止位置
        private float spawnFanX;   // 画面外の初期位置（帰る場所）
        private bool isFanMoving = false;    // 登場中フラグ
        private bool isFanRetreating = false;// ★追加：退出（帰還）中フラグ

        void Awake()
        {
            if (fanObject != null) fanObject.SetActive(false);
            if (windParticle != null) windParticle.Stop();
        }

        public void Init()
        {
            if (fanObject != null) fanObject.SetActive(false);
            if (windParticle != null) windParticle.Stop();
        }

        public void StartWind(Transform player)
        {
            isBlowing = true;
            playerTarget = player;
            
            isFanMoving = true;
            isFanRetreating = false; // 帰還フラグはリセット

            isFromRight = Random.value > 0.5f;

            float zDistance = Mathf.Abs(Camera.main.transform.position.z);
            float screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, zDistance)).x;
            float screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, zDistance)).x;
            
            // 出現位置（帰る場所）と、目標位置を計算して保存しておく
            spawnFanX = isFromRight ? screenRight + spawnOffset : screenLeft - spawnOffset;
            targetFanX = isFromRight ? screenRight - insideOffset : screenLeft + insideOffset;

            if (fanObject != null)
            {
                // まず画面外に配置
                fanObject.transform.position = new Vector3(spawnFanX, 0, 0);

                Vector3 scale = fanObject.transform.localScale;
                scale.x = isFromRight ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                fanObject.transform.localScale = scale;

                fanObject.SetActive(true);
            }

            if (windParticle != null)
            {
                windParticle.gameObject.SetActive(true);
                windParticle.transform.position = fanObject.transform.position;
                windParticle.transform.rotation = isFromRight ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);
                windParticle.Play();
            }
        }

        public void StopWind()
        {
            // 風の押し出し判定はすぐに終了する
            isBlowing = false;
            
            // 登場中にStopが呼ばれた場合のために登場フラグをOFF
            isFanMoving = false;
            
            // ★追加：退出アニメーションを開始！
            isFanRetreating = true; 

            // 風のエフェクトの「発生」は止める（残りの風は飛んでいく）
            if (windParticle != null)
            {
                windParticle.Stop();
            }
        }

        void Update()
        {
            // 1. 登場中の移動処理
            if (isFanMoving && fanObject != null)
            {
                Vector3 currentPos = fanObject.transform.position;
                currentPos.x = Mathf.MoveTowards(currentPos.x, targetFanX, fanMoveSpeed * Time.deltaTime);
                fanObject.transform.position = currentPos;

                if (windParticle != null) windParticle.transform.position = currentPos;

                if (Mathf.Abs(currentPos.x - targetFanX) < 0.01f)
                {
                    isFanMoving = false;
                }
            }
            // 2. ★追加：退出（帰還）中の移動処理
            else if (isFanRetreating && fanObject != null)
            {
                Vector3 currentPos = fanObject.transform.position;
                // 最初に出現した画面外の場所（spawnFanX）に向かって帰っていく
                currentPos.x = Mathf.MoveTowards(currentPos.x, spawnFanX, fanMoveSpeed * Time.deltaTime);
                fanObject.transform.position = currentPos;

                if (windParticle != null) windParticle.transform.position = currentPos;

                // 画面外の目標位置に到着したら、完全に非表示にする
                if (Mathf.Abs(currentPos.x - spawnFanX) < 0.01f)
                {
                    isFanRetreating = false;
                    fanObject.SetActive(false);
                    if (windParticle != null) windParticle.gameObject.SetActive(false);
                }
            }

            // 3. プレイヤーを押し出す処理（風が吹いている時のみ）
            if (isBlowing && playerTarget != null)
            {
                Vector3 pushDirection = isFromRight ? Vector3.left : Vector3.right;
                playerTarget.Translate(pushDirection * windStrength * Time.deltaTime, Space.World);
            }
        }
    }
}