using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

public class UIManager : MonoBehaviour
{
    public RectTransform target; // フェード・拡大するテキスト
    public TextMeshProUGUI targetText;
    public RectTransform zoomGroup;  // それ以外のUIをまとめた親オブジェクト(ヒエラルキーのObjects下に入っているすべてのオブジェクトが対象)
    public List<Image> Lives;
    
    [SerializeField] private float first_duration = 1.0f;    // 最初のテキストがフェードインするアニメーションの時間
    [SerializeField] private float second_duration = 1.0f;    // 次に他のオブジェクトが拡大するアニメーションの時間

    public void PlayAnimation(string scene)
    {
        // 初期状態：テキストを消しておく
        targetText = target.GetComponent<TextMeshProUGUI>();
        targetText.alpha = 0;
        targetText.transform.localScale = Vector3.one * 1.2f; // 最初から1.2倍

        // 演出開始
        StartCoroutine(PlayUIAnimation(scene));
    }

    IEnumerator PlayUIAnimation(string scene)
    {
        float elapsed = 0f;

        // 元の拡大率を保持
        Vector3 textStartScale = Vector3.one * 1.2f;
        Vector3 groupStartScale = Vector3.one;
        Vector3 groupEndScale = Vector3.one * 3.0f; // UIのズーム倍率（お好みで）

        // 0. 裏でシーンの読み込みを開始する（まだ切り替えない）
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        asyncLoad.allowSceneActivation = false; // 読み込み完了しても勝手に切り替わらないようにする

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
}