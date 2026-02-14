using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    [SerializeField] private List<CreateScene> minigames;  // ミニゲーム一覧を持つ
    private int loaded_minigame = 0;  // ロードされているゲームの番号


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
        // タイムスケールを変更
        MGManager.applyNewTimeScale();
        Time.timeScale = MGManager.timeScale;
        Debug.Log(MGManager.timeScale);

        // UIManager
        // uiManager = FindFirstObjectByType<UIManager>();

        
        // アニメーション&シーン切り替え
        loaded_minigame = Random.Range(0, minigames.Count - 1);
        string scene = minigames[loaded_minigame].scene_name;  // ミニゲームの名前
        string verb = minigames[loaded_minigame].verb;  // ミニゲームの動詞
        uiManager.PlayAnimation(verb, scene);
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

        // ミニゲーム用のUIに切り替える
        uiManager.MinigameUI();

        // ミニゲームがロードされてからtimelimit秒だけ待つ
        float elapsed = 0f;
        float timelimit = minigames[loaded_minigame].timelimit;
        int last = (int)timelimit;

        while (elapsed < timelimit) { 
            // カウントダウン
            if (last > (timelimit - elapsed)) { uiManager.UITimer(last); last--; }
            elapsed += Time.deltaTime;
            yield return null;
        }
        uiManager.UITimer(last);

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

        // ステージ数を一増やす
        MGManager.nextStage();

        // ロード状況とクリア状況をリセット
        MGManager.Finished();

        // 4. ついにシーンを切り替える
        asyncLoad.allowSceneActivation = true;

        // UIをもとに戻す
        uiManager.UIReset();

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
