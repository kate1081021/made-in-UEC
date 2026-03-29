using UnityEngine;

public class GoalProgressUI : MonoBehaviour
{
    [Header("UI設定エリア")]
    [SerializeField] private Vector2 startPosition; // 画面上の開始座標 (X, Y)
    [SerializeField] private Vector2 endPosition;   // 画面上の終了座標 (X, Y)
    [SerializeField] private float duration = 60f; // ゴールまでにかかる時間（秒）

    private RectTransform rectTransform; // UI専用のTransform
    private float elapsedTime = 0f;
    private bool isMoving = true;

    void Start()
    {
        // 自分自身についている RectTransform を取得
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!isMoving || rectTransform == null) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        // UIの位置（anchoredPosition）をLerpで移動させる
        rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);

        if (t >= 1.0f) isMoving = false;
    }
}
