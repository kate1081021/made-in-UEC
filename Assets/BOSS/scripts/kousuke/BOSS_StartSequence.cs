using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BOSS
{
    [System.Serializable]
    public class PlayerVisual
    {
        public Sprite playerSprite;      // 画像
        public float customScale = 1.0f; // この画像専用の大きさ（倍率）
        public Vector3 positionOffset = Vector3.zero; // この画像専用の位置ズレ調整
    }

    public class BOSS_StartSequence : MiniGameBase
    {
        [Header("演出で動かすもの")]
        public BOSS_playerControler playerController; 
        public Animator playerAnimator;               
        public SpriteRenderer playerSpriteRenderer;   
        
        [Header("画像・スケール・位置の設定")]
        public PlayerVisual sequenceStandVisual; // 【1】演出中の立ち画像
        public PlayerVisual readyJumpVisual;     // 【2】しゃがみ（構え）の画像
        public PlayerVisual jumpVisual;          // 【3】飛んでいる最中の画像
        public PlayerVisual gameStartVisual;     // 【4】ゲーム本編開始時の立ち画像
        
        [Header("位置のセッティング")]
        public Vector3 readyPosition;        
        
        [Header("スピード・時間設定")]
        public float jumpSpeed = 15f;        
        public float standTime = 0.5f;       
        public float crouchTime = 0.5f;      

        [Header("UI・背景設定")]
        public GameObject startBackground;    
        public Image fadePanel;               
        public float fadeSpeed = 2f;          

        [Header("ゲーム本編のシステム")]
        public MonoBehaviour[] systemsToStartLate; 

        // ★追加：全体へ「ゲームが本当に始まったか」を知らせる合図
        public static bool isGamePlaying = false; 

        public override void OnGameStart()
        {
            isGamePlaying = false; // ★追加：最初は「まだ始まってない」にしておく

            MGManager.Load();
            StartCoroutine(PlayStartSequence());
        }

        private void SetPlayerVisual(PlayerVisual visual, Vector3 basePos)
        {
            if (playerSpriteRenderer != null && visual.playerSprite != null)
            {
                playerSpriteRenderer.sprite = visual.playerSprite;
                playerController.transform.localScale = new Vector3(visual.customScale, visual.customScale, 1f);
                playerController.transform.position = basePos + visual.positionOffset;
            }
        }

        IEnumerator PlayStartSequence()
        {
            if (fadePanel != null)
            {
                Color c = fadePanel.color;
                c.a = 1f; 
                fadePanel.color = c;
            }

            if (playerController == null) yield break;

            if (startBackground != null) startBackground.SetActive(true);
            
            playerController.transform.position = readyPosition;

            foreach (var system in systemsToStartLate)
            {
                if (system != null) system.enabled = false;
            }

            // --------------------------------------------------
            // 準備：【1】演出中の立ち姿をセット
            // --------------------------------------------------
            if (playerAnimator != null) playerAnimator.enabled = false;
            SetPlayerVisual(sequenceStandVisual, readyPosition);

            Debug.Log("演出1：最初の明転！");
            yield return StartCoroutine(FadeScreen(1f, 0f)); 
            yield return new WaitForSeconds(standTime); 

            // --------------------------------------------------
            // ２．【2】しゃがむ！（構え）
            // --------------------------------------------------
            Debug.Log("演出2：しゃがむ！");
            SetPlayerVisual(readyJumpVisual, readyPosition);
            yield return new WaitForSeconds(crouchTime); 

            // --------------------------------------------------
            // ３．【3】大ジャンプで画面上部に消える
            // --------------------------------------------------
            Debug.Log("演出3：大ジャンプ！");
            SetPlayerVisual(jumpVisual, readyPosition);

            float timer = 0f;
            Vector3 currentJumpPos = readyPosition + jumpVisual.positionOffset;
            
            while (timer < 1.0f)
            {
                currentJumpPos += Vector3.up * jumpSpeed * Time.deltaTime;
                playerController.transform.position = currentJumpPos;
                timer += Time.deltaTime;
                yield return null; 
            }

            // --------------------------------------------------
            // ４．暗転 ＆ 裏でコソコソ準備
            // --------------------------------------------------
            Debug.Log("演出4：暗転！");
            yield return StartCoroutine(FadeScreen(0f, 1f)); 

            if (startBackground != null) startBackground.SetActive(false);

            // --------------------------------------------------
            // 【4】ゲーム本編開始時の立ち姿をセットし、Animatorオン
            // --------------------------------------------------
            SetPlayerVisual(gameStartVisual, readyPosition);
            if (playerAnimator != null) playerAnimator.enabled = true;

            // ==================================================
            // ★暗転中に、タイマーなどのシステムをオンにする！（姿を見せる）
            // ==================================================
            foreach (var system in systemsToStartLate)
            {
                if (system != null) system.enabled = true;
            }

            // タイマーが出た状態で、0.5秒間「真っ暗な画面」をキープする
            yield return new WaitForSeconds(0.5f);

            // --------------------------------------------------
            // ５．明転 ＆ ゲーム本編スタート！
            // --------------------------------------------------
            Debug.Log("演出5：ゲーム本編スタート！");
            yield return StartCoroutine(FadeScreen(1f, 0f)); 
            
            playerController.BOSS_canMove = true;

            // ==================================================
            // ★追加：明転が完全に終わったここで「スタート！」の合図を出す！
            // ==================================================
            isGamePlaying = true; 
        }

        IEnumerator FadeScreen(float startAlpha, float endAlpha)
        {
            if (fadePanel == null) yield break;
            float t = 0f;
            Color color = fadePanel.color;
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                color.a = Mathf.Lerp(startAlpha, endAlpha, t);
                fadePanel.color = color;
                yield return null;
            }
            color.a = endAlpha;
            fadePanel.color = color;
        }
    }
}