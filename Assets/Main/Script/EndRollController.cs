using UnityEngine;
using UnityEngine.UI; // LayoutRebuilderを使用するために必須
using UnityEngine.SceneManagement;

public class EndRollController : MiniGameBase
{
    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed = 100f;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private float accelerationSmoothness = 10f; // 滑らかさ

    private bool isScrolling = false; // Start前は動かさない
    public float upSpeed = 7f;

    private float targetSpeedMultiplier = 1f; // 目標倍率
    private float currentSpeedMultiplier = 1f; // 現在の倍率
    private float contentHeight; // レイアウト計算後の正しい高さを保持

    public override void OnGameStart()
    {
        if (contentRect == null) contentRect = GetComponent<RectTransform>();

        // 1. レイアウトの強制再計算（絶対に消さないこと）
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        // 正しい高さを取得してキャッシュする
        contentHeight = contentRect.sizeDelta.y;

        // 2. 開始位置の初期化
        // コンテンツの上端が画面の下端に来る位置からスタートさせる
        contentRect.anchoredPosition = new Vector2(0, -Screen.height);

        currentSpeedMultiplier = 1f;
        targetSpeedMultiplier = 1f;
        isScrolling = true;
    }

    protected override void OnActionStarted(float value)
    {
        // 入力があったら加速倍率をセット
        targetSpeedMultiplier = upSpeed;
    }

    protected override void OnActionCanceled(float value)
    {
        // 離したら等倍に戻す
        targetSpeedMultiplier = 1f;
    }

    void Update()
    {
        if (!isScrolling) return;

        // 速度倍率を滑らかに変化させる（素晴らしい実装です）
        currentSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, targetSpeedMultiplier, Time.deltaTime * accelerationSmoothness);

        // 移動処理
        contentRect.anchoredPosition += Vector2.up * (scrollSpeed * currentSpeedMultiplier * Time.deltaTime);

        // 3. 終了判定：コンテンツの「最後尾」が画面上端を完全に抜けたら
        if (contentRect.anchoredPosition.y > contentHeight + Screen.height)
        {
            isScrolling = false;
            OnEndRollComplete();
        }
    }

    private void OnEndRollComplete()
    {
        Debug.Log("エンドロール終了");
        // ここにシーン遷移などを記述
        SceneManager.LoadSceneAsync("Title");
    }
}