using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RectTransform target; // フェード・拡大するテキスト
    private TextMeshProUGUI targetText;  // 動詞を表示するテキスト
    public TextMeshProUGUI counter;  // ステージ数をカウントするもの
    public TextMeshProUGUI timer;  // ミニゲーム中のタイマー表示
    public RectTransform zoomGroup;  // それ以外のUIをまとめた親オブジェクト(ヒエラルキーのObjects下に入っているすべてのオブジェクトが対象)
    public GameObject Cont_Stick;
    public GameObject Cont_PikoPiko;
    public GameObject Cont_Trigger;
    public GameObject Cont_Aim;
    public List<Image> Lives;
    public Image[] timerSources;
    public Animator UIanimator; // リズムに合わせて動くやつのアニメーション
    public bool isZoomed = false; // ズームが終わったかどうかのフラグ
    [SerializeField] private float first_duration = 1.0f;    // 最初のテキストがフェードインするアニメーションの時間
    [SerializeField] private float second_duration = 1.0f;    // 次に他のオブジェクトが拡大するアニメーションの時間
    [SerializeField] private float cont_fadein_duration = 0.5f;
    [SerializeField] private float cont_show_duration = 0.5f;
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
    public void controllerAnimation(string type)
    {
        GameObject target;
        switch (type)
        {
            case "ピコピコ":
            target = Cont_PikoPiko;
            break;
            case "トリガー":
            target = Cont_Trigger;
            break;
            case "スティック":
            target = Cont_Stick;
            break;
            case "エイム":
            target = Cont_Aim;
            break;
            default :
            target = Cont_PikoPiko;
            break;
        }
        StartCoroutine(PlayContAnim(target));
    }
    IEnumerator PlayContAnim(GameObject target)
    {
        Debug.Log("called");
        RectTransform rt = target.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.65f;
        float elapsed = 0f;
        while (elapsed < cont_fadein_duration)
        {
            elapsed += Time.unscaledDeltaTime * MGManager.pitchScale;
            float t = elapsed / cont_fadein_duration;
            
            // イージング（滑らかにする設定）
            float curve = Mathf.SmoothStep(-680, -100, t);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x,curve);
            yield return null;
        }
        Debug.Log("waitinging");
        yield return new WaitForSecondsRealtime(cont_show_duration/MGManager.pitchScale);
        Debug.Log("waited");
        elapsed = 0f;
        while (elapsed < cont_fadein_duration)
        {
            elapsed += Time.unscaledDeltaTime * MGManager.pitchScale;
            float t = elapsed / cont_fadein_duration;
            
            // イージング（滑らかにする設定）
            float curve = Mathf.SmoothStep(-100, 460, t);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x,curve);
            yield return null;
        }
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x,-680);
        rt.localScale = Vector3.one * 0.0001f; // ゼロだとバグる可能性があるとか
        Debug.Log("finished");
    }
    public void PlayAnimation(string scene, string verb)
    {
        // 初期状態：テキストを消しておく
        targetText = target.GetComponent<TextMeshProUGUI>();
        targetText.alpha = 0;
        timer.alpha = 0;
        timerSources[0].color = new Color(255,255,255,0);
        timerSources[1].color = new Color(255,255,255,0);
        targetText.transform.localScale = Vector3.one * 1.2f; // 最初から1.2倍

        // 演出開始
        StartCoroutine(PlayUIAnimation(verb));
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
        timerSources[0].color = new Color(255,255,255,0);
        timerSources[1].color = new Color(255,255,255,0);
    }
    public void UIReset()
    {
        // 拡大率の初期化
        target.localScale = Vector3.one;
        zoomGroup.localScale = Vector3.one;

        // 文字の非表示
        targetText.alpha = 0;
        timer.alpha = 0;
        timerSources[0].color = new Color(255,255,255,0);
        timerSources[1].color = new Color(255,255,255,0);
        // オブジェクトの表示
    }

    // ステージ数を更新
    public void updateStage()
    {
        // ステージ数を更新
        counter.text = $"{MGManager.stage}";
    }
    public void WinAnimation()
    {
        UIanimator.speed = MGManager.pitchScale;
        UIanimator.SetTrigger("Win"); 
    }
    public void LoseAnimation()
    {
        UIanimator.speed = MGManager.pitchScale;
        UIanimator.SetTrigger("Lose"); 
    }

    public void GameStartAnimation()
    {
        UIanimator.speed = MGManager.pitchScale;
        UIanimator.SetTrigger("GameStart");
    }
    public void SpeedUpAnimation()
    {
        UIanimator.speed = 1.0f;
        UIanimator.SetTrigger("SpeedUp");
    }

    public void BossAnimation()
    {
        UIanimator.speed = 1.0f;
        UIanimator.SetTrigger("Boss");
    }

    // リズムに合わせて動くやつ
    public IEnumerator RhythmAnimation(float BPM)
    {
        UIanimator.speed = Time.timeScale;
        UIanimator.SetTrigger("Beat");
        yield return new WaitForSecondsRealtime((60f*4f)/((BPM <= 0 ? BPM : 120)*MGManager.pitchScale));
        UIanimator.SetTrigger("Finish");
    }

    // メインのアニメーションを表示
    IEnumerator PlayUIAnimation(string verb)
    {
        isZoomed = false;
        float elapsed = 0f;

        // 元の拡大率を保持
        Vector3 textStartScale = Vector3.one * 1.2f;
        Vector3 groupStartScale = Vector3.one;
        Vector3 groupEndScale = Vector3.one * 3.0f; // UIのズーム倍率（お好みで）

        // 動詞を確定させる
        targetText.text = verb;

        // 1. テキストをフェードイン
        while (elapsed < first_duration)
        {
            elapsed += Time.unscaledDeltaTime * MGManager.pitchScale;
            float t = elapsed / first_duration;
            
            // イージング（滑らかにする設定）
            float curve = Mathf.SmoothStep(0, 1, t);

            targetText.alpha = curve;
            target.localScale = Vector3.Lerp(textStartScale, Vector3.one, curve);
            yield return null;
        }

        Debug.Log($"フェードイン: {elapsed}s");
        elapsed = 0f;
        yield return new WaitForSeconds(0.05f);

        // 2. それ以外のUIをズームイン（拡大）
        while (elapsed < second_duration)
        {
            elapsed += Time.unscaledDeltaTime * MGManager.pitchScale;
            float t = elapsed / second_duration;
            
            // イージング（滑らかにする設定）
            float curve = Mathf.SmoothStep(0, 1, t);

            if (zoomGroup != null)
            {
                zoomGroup.localScale = Vector3.Lerp(groupStartScale, groupEndScale, curve);
            }
            yield return null;
        }

        Debug.Log($"ズームイン: {elapsed}s");

        // 2.5 値を確定させる
        targetText.alpha = 1;
        if (zoomGroup != null) zoomGroup.localScale = groupEndScale;
        isZoomed = true;

    }

    public void UITimer(int sec)
    {
        timer.alpha = 1;
        timer.text = $"{sec}";
        if (sec < 4)
        {
            timerSources[0].color = new Color(255,255,255,0);
            timerSources[1].color = new Color(255,255,255,255);
        }
        else
        {
            timerSources[0].color = new Color(255,255,255,255);
            timerSources[1].color = new Color(255,255,255,0);
        }
    }

}