using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

public class UIManager : MonoBehaviour
{
    public RectTransform target; // フェード・拡大するテキスト
    private TextMeshProUGUI targetText;  // 動詞を表示するテキスト
    public TextMeshProUGUI counter;  // ステージ数をカウントするもの
    public TextMeshProUGUI timer;  // ミニゲーム中のタイマー表示
    public RectTransform zoomGroup;  // それ以外のUIをまとめた親オブジェクト(ヒエラルキーのObjects下に入っているすべてのオブジェクトが対象)
    public List<Image> Lives;
    
    [SerializeField] private float first_duration = 1.0f;    // 最初のテキストがフェードインするアニメーションの時間
    [SerializeField] private float second_duration = 1.0f;    // 次に他のオブジェクトが拡大するアニメーションの時間

    public static UIManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 最初の一つだけを保護
        }
        else
        {
            Destroy(gameObject); // 二つ目以降は即座に消す
        }
    }
    
    public void PlayAnimation(string verb, string scene)
    {
        // 初期状態：テキストを消しておく
        targetText = target.GetComponent<TextMeshProUGUI>();
        targetText.alpha = 0;
        timer.alpha = 0;
        targetText.transform.localScale = Vector3.one * 1.2f; // 最初から1.2倍

        // 演出開始
        StartCoroutine(PlayUIAnimation(verb, scene));
    }

    // ミニゲーム中のUI
    public void MinigameUI()
    {
        // 拡大率の初期化
        target.localScale = Vector3.one;
        
        // タイマー以外の非表示
        zoomGroup.localScale = Vector3.zero;
        targetText.alpha = 0;

        // タイマーの表示
        timer.alpha = 0;

    }
    public void UIReset()
    {
        // 拡大率の初期化
        target.localScale = Vector3.one;
        zoomGroup.localScale = Vector3.one;

        // 文字の非表示
        targetText.alpha = 0;
        timer.alpha = 0;

        // オブジェクトの表示
    }

    IEnumerator PlayUIAnimation(string verb, string scene)
    {
        float elapsed = 0f;

        // 元の拡大率を保持
        Vector3 textStartScale = Vector3.one * 1.2f;
        Vector3 groupStartScale = Vector3.one;
        Vector3 groupEndScale = Vector3.one * 3.0f; // UIのズーム倍率（お好みで）

        // 0. 裏でシーンの読み込みを開始する（まだ切り替えない）
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        asyncLoad.allowSceneActivation = false; // 読み込み完了しても勝手に切り替わらないようにする

        // ステージ数をカウント
        counter.text = $"{MGManager.stage}";

        // 動詞を確定させる
        targetText.text = verb;

        // 1. テキストをフェードイン
        while (elapsed < first_duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / first_duration;
            
            // イージング（滑らかにする設定）
            float curve = Mathf.SmoothStep(0, 1, t);

            targetText.alpha = curve;
            target.localScale = Vector3.Lerp(textStartScale, Vector3.one, curve);
            yield return null;
        }

        elapsed = 0f;
        yield return new WaitForSeconds(0.05f);

        // 2. それ以外のUIをズームイン（拡大）
        while (elapsed < second_duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / second_duration;
            
            // イージング（滑らかにする設定）
            float curve = Mathf.SmoothStep(0, 1, t);

            if (zoomGroup != null)
            {
                zoomGroup.localScale = Vector3.Lerp(groupStartScale, groupEndScale, curve);
            }
            yield return null;
        }

        // 2.5 値を確定させる
        targetText.alpha = 1;
        if (zoomGroup != null) zoomGroup.localScale = groupEndScale;

        // 3. アニメーションが終わるまで、かつロードが90%（準備完了）まで待機
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // 4. ついにシーンを切り替える
        asyncLoad.allowSceneActivation = true;

    }

    public void UITimer(int sec)
    {
        timer.alpha = 1;
        Debug.Log($"{sec}");
        timer.text = $"{sec}";
    }

}