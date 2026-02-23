using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;  // UIManager

    public static AsyncOperation asyncLoad;  // 同期ロード用

    public AudioSource BGM_start_1;  // ゲーム開始時のBGM
    public AudioSource BGM_start_2;  // 各ゲームの間の曲(1の短縮ver.)
    public AudioSource Success;  // ミニゲーム成功時のBGM
    public AudioSource Speedup;  // スピードアップ時のBGM
    private double nextPlayTime;  // BGMを次に再生するまでの時間
    private float PitchScale = 1.0f;  // BGMのピッチを管理する

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

    /* BGM */
    // BGMを即座に再生する
    private void PlayImmidiate(AudioSource audio, float scale)
    {
        audio.Play();
        nextPlayTime = AudioSettings.dspTime + audio.clip.length / scale;
    }

    // BGM再生を予約する
    private void PlayNext(AudioSource audio, float scale)
    {
        audio.PlayScheduled(nextPlayTime);
        nextPlayTime += audio.clip.length / scale;
    }

    private IEnumerator MainCoroutine()
    {   
        
        // BGMの総プレイ時間
        double TotalPlayTime = 0.0f;
        double FirstPlayTime = 0.0f;

        // タイムスケールを変更
        MGManager.applyNewTimeScale();
        Time.timeScale = MGManager.timeScale;

        // スピードアップ
        bool speedup = false;
        int stage = MGManager.stage;
        if ((5 < stage && stage <= 15 && stage % 5 == 1) || (stage > 15 && (stage - 15) % 10 == 1)) { speedup = true; }

        // アニメーション&シーン切り替え
        loaded_minigame = 0;//Random.Range(0, minigames.Count);
        string scene = minigames[loaded_minigame].scene_name;  // ミニゲームの名前
        string verb = minigames[loaded_minigame].verb;  // ミニゲームの動詞

        // 裏でシーンの読み込みを開始する（まだ切り替えない）
        asyncLoad = SceneManager.LoadSceneAsync(scene);
        asyncLoad.allowSceneActivation = false; // 読み込み完了しても勝手に切り替わらないようにする

        // 最初のステージの時は少し待つ
        if (MGManager.stage == 1) { yield return new WaitForSeconds(2.0f); }

        // BGMがスタートしたタイミングを記録
        double StartTime = AudioSettings.dspTime;

        // アニメーションが再生されたか
        bool isStageUpdated = false;  // stage数が更新されたら
        bool isAnimationPlaying = false;  // メインのアニメーションが表示されたら

        // 勝利状況の確認(Stage2以降)
        if (MGManager.stage > 1) {
            if (MGManager.IsClear)
            {
                Debug.Log("ミニゲームクリア!!");
                PlayImmidiate(Success, PitchScale);
                FirstPlayTime += Success.clip.length / PitchScale;
                TotalPlayTime += Success.clip.length / PitchScale;
            } 
            else
            {
                Debug.Log("ミニゲーム失敗");
                PlayImmidiate(Success, PitchScale);
                FirstPlayTime += Success.clip.length / PitchScale;
                TotalPlayTime += Success.clip.length / PitchScale;
            }
        }

        // クリア判定をリセット
        MGManager.Finished();

        // スピードアップ
        if (speedup)
        {
            PlayNext(Speedup, 1.0f);
            FirstPlayTime += Speedup.clip.length;
            TotalPlayTime += Speedup.clip.length;
            PitchScale *= 1.059463094f;  // 各音階の比率
            BGM_start_2.pitch = PitchScale;
        }
        
        // 曲を再生し始める
        if (MGManager.stage == 1) {
            PlayImmidiate(BGM_start_1, PitchScale);
            TotalPlayTime += BGM_start_1.clip.length;
        } else {
            PlayNext(BGM_start_2, PitchScale);
            TotalPlayTime += BGM_start_2.clip.length / PitchScale;
        }

        // 曲の再生終了とアニメーションの終了を同期させる
        while (BGM_start_1.isPlaying || BGM_start_2.isPlaying)  // ここの1.1(s)は現在のアニメーションが再生し終わるまでにかかる時間
        {
            double currentTime = AudioSettings.dspTime;
            // stage数
            if (currentTime >= StartTime + FirstPlayTime && !isStageUpdated)
            {
                // stage数更新
                uiManager.updateStage();
                isStageUpdated = true;
                
            }
            // アニメーション
            if (currentTime >= StartTime + TotalPlayTime - 1.1f && !isAnimationPlaying)
            {
                // アニメーション再生
                uiManager.PlayAnimation(scene, verb);
                isAnimationPlaying = true;

            }
            yield return null;
        }

        // 最後にSuccessとFailureのPitchを変える
        Success.pitch = PitchScale;

        // 3. ロードが90%（準備完了）まで待機
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // ついにシーンを切り替える
        asyncLoad.allowSceneActivation = true;
        
        // ミニゲームシーンに移行
        StartCoroutine(MiniGame());

    }

    private IEnumerator MiniGame()
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

        // 仮の爆弾が出てくる時間
        float bombtime = 3f;
        // 仮のシーン切り替えまでの猶予
        float waitUntilClearTime = 1f;
        // ゲームの早期切り上げが可能かどうか(通常はtrue)
        bool stopEarlyFinish = minigames[loaded_minigame].stopEarlyFinish;

        while (elapsed < timelimit) {
            // カウントダウン
            if (last > (timelimit - elapsed)) { uiManager.UITimer(last); last--; }
            if (!stopEarlyFinish && MGManager.IsClear && (timelimit - elapsed) > (bombtime + waitUntilClearTime))
            // 早めに切り上げてる待ち時間中に爆弾が現れないように
            {
                Debug.Log("早めに切り上げ");
                yield return new WaitForSeconds(waitUntilClearTime);
                Debug.Log("早めに切り上げた");
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        uiManager.UITimer(last);

        // 3. アニメーションが終わるまで、かつロードが90%（準備完了）まで待機
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // ステージ数を一増やす
        MGManager.nextStage();

        // 4. ついにシーンを切り替える
        asyncLoad.allowSceneActivation = true;

        // UIをもとに戻す
        uiManager.UIReset();

        // 5. MainCouroutineに戻る
        StartCoroutine(MainCoroutine());

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
