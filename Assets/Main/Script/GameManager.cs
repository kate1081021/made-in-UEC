using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIManager uiManager;

    public static GameManager Instance;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // タイトルコール

        // ゲーム進行コルーチン呼び出し
        StartCoroutine(MainCoroutine());
    }

    IEnumerator MainCoroutine()
    {
        // UIManager
        uiManager = Object.FindFirstObjectByType<UIManager>();

        
        // アニメーション&シーン切り替え
        uiManager.PlayAnimation("mochiCatch");
        StartCoroutine(MiniGame());

        yield return null;
    }

    IEnumerator MiniGame()
    {
        // ミニゲームがロードされるまで待機
        while (!MGManager.isMinigameLoaded) { yield return null; }

        // 0. 裏でMainシーンの読み込みを開始する（まだ切り替えない）
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");
        asyncLoad.allowSceneActivation = false; // 読み込み完了しても勝手に切り替わらないようにする

        // ミニゲームがロードされてからtimelimit秒だけ待つ
        float elapsed = 0f;

        while (elapsed < MGManager.timelimit) { 
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3. アニメーションが終わるまで、かつロードが90%（準備完了）まで待機
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // 勝利状況の確認
        if (MGManager.IsClear)
        {
            Debug.Log("ミニゲームクリア!!");
        } 
        else
        {
            Debug.Log("ミニゲーム失敗");
        }

        // ロード状況とクリア状況をリセット
        MGManager.Finished();

        // 4. ついにシーンを切り替える
        asyncLoad.allowSceneActivation = true;

        // 少し待機
        yield return new WaitForSeconds(1.0f);

        // 5. MainCouroutineに戻る
        StartCoroutine(MainCoroutine());

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
