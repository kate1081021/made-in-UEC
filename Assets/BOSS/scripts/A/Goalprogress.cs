using UnityEngine;

public class GoalProgressUI : MonoBehaviour
{
    [Header("UI設定エリア")]
    [SerializeField] private Vector2 startPosition; // 画面上の開始座標 (X, Y)＝Empty状態（下）
    [SerializeField] private Vector2 endPosition;   // 画面上の終了座標 (X, Y)＝Full状態（上）
    [SerializeField] private float duration = 60f;  // ゴールまでにかかる時間（秒）

    private RectTransform rectTransform; 
    private CanvasGroup canvasGroup; 
    
    private float elapsedTime = 0f;
    private bool isMoving = true;

    // ★修正：Start()ではなく、一番最初に呼ばれる Awake() で部品を準備する！
    void Awake()
    {
        // これで、OnEnableが呼ばれる時には絶対に準備が完了しています！
        rectTransform = GetComponent<RectTransform>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    // （Start()は中身が空になったので削除しました）

    // 演出監督によってスクリプトが【オン】にされた瞬間に呼ばれる（暗転中）
    void OnEnable()
    {
        // 1. Awakeのおかげで部品が絶対にあるので、確実に下（startPosition）へ移動します！
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startPosition;
        }

        // 2. 時間を「0秒」に戻す
        elapsedTime = 0f; 
        isMoving = true;

        // 3. 確実に下へ移動してから、姿を現す
        if (canvasGroup != null) canvasGroup.alpha = 1f; 
    }

    // 演出監督によってスクリプトが【オフ】にされた瞬間に呼ばれる
    void OnDisable()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f; // 0%透明にして隠す
    }

    void Update()
    {
        // 演出監督から「スタート！」の合図が出るまで待機
        if (!BOSS.BOSS_StartSequence.isGamePlaying) return;

        if (!isMoving || rectTransform == null) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);

        if (t >= 1.0f) isMoving = false;
    }
}