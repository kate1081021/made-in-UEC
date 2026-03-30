using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;  // UIManager

    public static AsyncOperation asyncLoad;  // 同期ロード用

    public AudioSource BGM_start_1;  // ゲーム開始時のBGM
    public AudioSource BGM_start_2;  // 各ゲームの間の曲(1の短縮ver.)
    public AudioSource Success;  // ミニゲーム成功時のBGM
    public AudioSource Failure; // ミニゲーム失敗時のBGM
    public AudioSource Speedup;  // スピードアップ時のBGM
    public AudioSource ClearGame; // ノーマルクリア時の効果音
    public AudioSource StartBoss; // ボスステージ入るときの効果音
    private double nextPlayTime;  // BGMを次に再生するまでの時間
    private float PitchScale = 1.0f;  // BGMのピッチを管理する
    

    [SerializeField] private List<CreateScene> minigames;  // ミニゲーム一覧を持つ
    [SerializeField] private Transform lives; // ライフたちの親の参照
    private int lifeRemain = 4;
    private int loaded_minigame = 0;  // ロードされているゲームの番号
    private int debug_scene = -1;  // デバッグでロード中のシーンの番号
    private List<int> minigameQueue = new List<int>();
    private bool isPlayedBossGame = false; // ボスステージやったかどうか？のフラグ


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
        // BGMの音量を調整
        BGM_start_1.volume = MGManager.sound_volume;
        BGM_start_2.volume = MGManager.sound_volume;
        Success.volume = MGManager.sound_volume;
        Failure.volume = MGManager.sound_volume;
        Speedup.volume = MGManager.sound_volume;

        // デバッグ用の中間コルーチン isDebugModeを折れば、通常通りのゲームが始まる
        StartCoroutine(TestPlayCoroutine());
    }

    IEnumerator TestPlayCoroutine()
    {
        Debug.Log("called");
        yield return null;
        while (MGManager.isDebugMode){
            yield return null;
        }

        // シーン検索
        for (int i = 0; i < minigames.Count; i++)
        {
            if (MGManager.scene == minigames[i].scene_name) { debug_scene = i; }
        }

        // 加速設定
        ScaleChangeTestPlay();
        lifeRemain = 4;
        LifeReset lr = lives.gameObject.GetComponent<LifeReset>();
        lr.lifeReset();

        if (TitleManager.isNormalMode) // ノーマルモード用の初期化
        {
            SettingNormal();
        }
        MGManager.initialize();
        PitchScale = 1.0f;
        Time.timeScale = 1.0f;

        // ゲーム進行コルーチン呼び出し
        StartCoroutine(MainCoroutine());
    }

    void SettingNormal()
    {
        Debug.Log("NORMAL MODE");
        List<int> number = new List<int>();
        for (int i = 0; i < minigames.Count-1; i++)
        {
            number.Add(i);
        }
        for (int i = 0; i < minigames.Count-1; i++)
        {
            int rand = Random.Range(0,number.Count);
            minigameQueue.Add(number[rand]);
            number.RemoveAt(rand);
        }
        minigameQueue.Add(minigames.Count-1); // ボスステージは最後に追加
        
        /* デバッグ用
        string debug = "";
        for (int i = 0; i < minigameQueue.Count; i++)
        {
            debug = debug + minigameQueue[i].ToString();
            debug = debug + ",";
        }
        Debug.Log(debug);
        */
    }

    /* テストプレイ時の加速用 */
    private void ScaleChangeTestPlay()
    {
        int stage = MGManager.stage;
        if (stage == 1) { MGManager.isMainCalled = true; } // 初期状態のときは通常通りにする
        else
        {
            int multiple = 0;
            for (int i = 1; i <= stage; i++)
            {
                if ((5 < i && i <= 15 && i % 5 == 1) || (i > 15 && (i - 15) % 10 == 1))
                { multiple++; }
            }
            Debug.Log(multiple);
            for (int i = 0; i < multiple; i++)
            {
                PitchScale *= 1.059463094f;  // 各音階の比率
            }
            BGM_start_2.pitch = PitchScale;
            MGManager.pitchScale = PitchScale;
        }
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
        Debug.Log("Started");
        yield return null;
        // BGMの総プレイ時間
        double TotalPlayTime = 0.0f;
        double FirstPlayTime = 0.0f;

        // 操作タイプ
        string controllType = "";

        if (TitleManager.isNormalMode && minigameQueue.Count == 0)
        {
            Time.timeScale = 1.0f; // ボスステージでは速度をリセット
            MGManager.timeScale = 1.0f; // 関数を挟まず代入
            Time.timeScale = MGManager.timeScale;
        }
        else
        {
        // タイムスケールを変更
        MGManager.applyNewTimeScale();
        Time.timeScale = MGManager.timeScale;
        }
        // スピードアップ
        bool speedup = false;
        int stage = MGManager.stage;
        // デバッグ後に、スピードが上がったかどうかのチェック
        if ((MGManager.isMainCalled && ((5 < stage && stage <= 15 && stage % 5 == 1) || (stage > 15 && (stage - 15) % 10 == 1))) || (!MGManager.isMainCalled && stage > 5)) { speedup = true; }

        // アニメーション&シーン切り替え
        if (TitleManager.isNormalMode) // ノーマルモード
        {
            if (minigameQueue.Count != 0)
            {
            loaded_minigame = minigameQueue[0];
            minigameQueue.RemoveAt(0);
            } else { loaded_minigame = -1; }
        }
        else
        {
            Debug.Log("エンドレスの抽選");
            loaded_minigame = debug_scene == -1 ? Random.Range(0, minigames.Count-1) : debug_scene;
        }
        string scene = ""; string verb = "";
        Debug.Log(loaded_minigame);
        if (loaded_minigame != -1)
        {
            scene = minigames[loaded_minigame].scene_name;  // ミニゲームの名前
            verb = minigames[loaded_minigame].verb;  // ミニゲームの動詞
            controllType = minigames[loaded_minigame].type;
            // 裏でシーンの読み込みを開始する（まだ切り替えない）
            asyncLoad = SceneManager.LoadSceneAsync(scene);
        }
        asyncLoad.allowSceneActivation = false; // 読み込み完了しても勝手に切り替わらないようにする

        // 最初のステージの時は少し待つ
        if (MGManager.stage == 1)
        {
            uiManager.GameStartAnimation();
            yield return new WaitForSeconds(2.0f);
        }

        // BGMがスタートしたタイミングを記録
        double StartTime = AudioSettings.dspTime;

        // アニメーションが再生されたか
        bool isStageUpdated = false;  // stage数が更新されたら
        bool isAnimationPlaying = false;  // メインのアニメーションが表示されたら
        bool isControllerAnimated = false; // Controllerのアニメが出てきたら
        // 勝利状況の確認(Stage2以降)
        if (MGManager.stage > 1 && MGManager.isMainCalled) {
            if (MGManager.IsClear)
            {
                if(loaded_minigame == -1)
                {
                    PlayImmidiate(ClearGame, PitchScale);
                    uiManager.WinAnimation();
                    FirstPlayTime += ClearGame.clip.length / PitchScale;
                    TotalPlayTime += ClearGame.clip.length / PitchScale;
                }
                else
                {
                    Debug.Log("ミニゲームクリア!!");
                    PlayImmidiate(Success, PitchScale);
                    uiManager.WinAnimation();
                    FirstPlayTime += Success.clip.length / PitchScale;
                    TotalPlayTime += Success.clip.length / PitchScale;
                }
            }
            else
            {
                Debug.Log("ミニゲーム失敗");
                PlayImmidiate(Failure, PitchScale);
                uiManager.LoseAnimation();
                FirstPlayTime += Failure.clip.length / PitchScale;
                TotalPlayTime += Failure.clip.length / PitchScale;
                if (isPlayedBossGame)
                {
                    for (int i = 0; i < lifeRemain; i++)
                    {
                        Transform target = lives.GetChild(i);
                        target.gameObject.SetActive(false);
                        yield return new WaitForEndOfFrame(); // 無効化まで待つ
                    }
                    lifeRemain = 0;
                }
                else
                {
                    Transform target = lives.GetChild(lifeRemain-1);
                    target.gameObject.SetActive(false);
                    lifeRemain--;
                    yield return new WaitForEndOfFrame(); // 無効化まで待つ
                    if (lifeRemain == 0)
                    {
                        Debug.Log("gameover");
                    }
                }
            }
        }

        // クリア判定をリセット
        MGManager.Finished();
        if (lifeRemain == 0)
        {
            while (Success.isPlaying || Failure.isPlaying)
            {
                yield return null;
            }
            GameOver();
            yield break;
        }
        if (loaded_minigame == -1)
        {
            while (Success.isPlaying || Failure.isPlaying || ClearGame.isPlaying)
            {
                yield return null;
            }
            GameClear();
            yield break;
        }
        // スピードアップ と、ボス判定
        if (TitleManager.isNormalMode && minigameQueue.Count == 0)
        {
            PlayNext(StartBoss, 1.0f);
            isPlayedBossGame = true;
            FirstPlayTime += Speedup.clip.length;
            TotalPlayTime += Speedup.clip.length;
            PitchScale = 1.0f;
            MGManager.pitchScale = PitchScale;

            BGM_start_2.pitch = PitchScale;
        }
        else if (speedup)
        {
            if (MGManager.isMainCalled){
            PlayNext(Speedup, 1.0f);
            FirstPlayTime += Speedup.clip.length;
            TotalPlayTime += Speedup.clip.length;
            PitchScale *= 1.059463094f;  // 各音階の比率
            MGManager.pitchScale = PitchScale;
            BGM_start_2.pitch = PitchScale;
            }
            else // テストプレイでステージをいじった後は効果音だけ鳴らすように
            {
                PlayImmidiate(Speedup, 1.0f);
                FirstPlayTime += Speedup.clip.length;
                TotalPlayTime += Speedup.clip.length;
                BGM_start_2.pitch = PitchScale;
            }
        }
        // 曲を再生し始める
        if (MGManager.stage == 1) {
            PlayImmidiate(BGM_start_1, PitchScale);
            TotalPlayTime += BGM_start_1.clip.length;
            PlayNext(BGM_start_2, PitchScale);
            TotalPlayTime += BGM_start_2.clip.length / PitchScale;
        } else {
            PlayNext(BGM_start_2, PitchScale);
            TotalPlayTime += BGM_start_2.clip.length / PitchScale;
        }

        
        while (Success.isPlaying || Failure.isPlaying)
        {
            yield return null;
        }
        if (speedup) // スピードアップのアニメーション用
        {
            uiManager.SpeedUpAnimation();
            while (Speedup.isPlaying)
            {
                yield return null;
            }
        } else if (TitleManager.isNormalMode && minigameQueue.Count == 0)
        {
            uiManager.BossAnimation();
            while (Speedup.isPlaying) // ボス用に変更
            {
                yield return null;
            }
        }

        // 曲の再生終了とアニメーションの終了を同期させる
        while (BGM_start_1.isPlaying || BGM_start_2.isPlaying)  // ここの1.1(s)は現在のアニメーションが再生し終わるまでにかかる時間
        {
            double currentTime = AudioSettings.dspTime;
            // stage数
            if (currentTime >= StartTime + FirstPlayTime && !isStageUpdated)
            {
                // stage数更新
                StartCoroutine(uiManager.updateStage());
                StartCoroutine(uiManager.RhythmAnimation(120)); // 仮置きしている現状のBPM
                //独自で分けます
                if (MGManager.stage != 1)
                { uiManager.controllerAnimation(controllType); }
                isStageUpdated = true;
            }
            // アニメーション
            if (MGManager.stage == 1 && currentTime >= StartTime + TotalPlayTime - 2.2f && !isControllerAnimated)
            {
                uiManager.controllerAnimation(controllType);
                isControllerAnimated = true;
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
        Failure.pitch = PitchScale;
        // デバッグ後初回の終わり
        MGManager.isMainCalled = true;

        // 3. ロードが90%（準備完了）まで待機
        while (asyncLoad.progress < 0.9f || !uiManager.isZoomed)
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
        float last = timelimit;

        // 仮の爆弾が出てくる時間
        float bombtime = 3f;
        // 仮のシーン切り替えまでの猶予
        float waitUntilClearTime = 1f;
        // ゲームの早期切り上げが可能かどうか(通常はtrue)
        bool stopEarlyFinish = minigames[loaded_minigame].stopEarlyFinish;

        while (elapsed < timelimit) {
            // カウントダウン
            if (last > (timelimit - elapsed)) 
            {
                uiManager.UITimer(last);
                if (!last.ToString().Contains("."))
                {
                    last -= 0.5f;
                } else
                {
                    last--;
                }
            }

            // 早めにゲームをクリアしたとき or 強制終了時
            if ((!stopEarlyFinish && MGManager.IsClear && (timelimit - elapsed) > (bombtime + waitUntilClearTime)) || MGManager.isFinishedForcibly)
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
        foreach (var game in MGManager.ActiveMiniGames.ToArray())
        {
            if (game != null) game.ExecuteGameEnd();
        }
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

    void GameOver()
    {
        Debug.Log($"<color=green> ゲームオーバー…(GameOver()より呼ばれています) </color>");
        MGManager.pitchScale = 1.0f;
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(uiManager.gameObject, SceneManager.GetActiveScene());
        SceneManager.LoadScene("GameOver");
    }
    void GameClear()
    {
        Debug.Log($"<color=green> ゲームクリア！(GameClear()より呼ばれています) </color>");
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
        SceneManager.MoveGameObjectToScene(uiManager.gameObject, SceneManager.GetActiveScene());
        SceneManager.LoadScene("EndCredits");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
