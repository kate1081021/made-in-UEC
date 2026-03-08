using UnityEngine;
using System;
using UnityEditor.SearchService;

public static class MGManager
{
    // 現在シーン内に存在するMiniGameBaseを継承したオブジェクトのリスト
    public static System.Collections.Generic.List<MiniGameBase> ActiveMiniGames = new();
    /// <summary> 
    /// ミニゲームのロードを確認する
    /// </summary>
    public static bool isMinigameLoaded { get; private set; } = false;

    /// <summary> 運営が現在のクリア状況を確認するためのプロパティ </summary>
    public static bool IsClear { get; private set; } = false;

    /// <summary> 今はデバッグ中かのプロパティ 公開前にfalseにする </summary>
    public static bool isDebugMode { get; private set; } = true;
    
    /// 現在いるステージ(何ゲームクリアしたのかを管理)
    public static int stage { get; private set; } = 1;

    /// ロードするシーン名
    public static string scene;

    /// ゲームが何倍速で動いているのかを管理する
    public static float timeScale = 1.0f;

    // 音の倍速
    public static float pitchScale = 1.0f;

    // 終了フラグ（これを各ミニゲームが書き換えるのではなく、管理側で制御する）
    public static bool isAllGameEndProcessed = false;

    // テストプレイの判断
    public static bool isMainCalled = false;

    // --- 部員が自由に使える便利関数 ---
    /// <summary>
    /// ゲームが開始した直後にこれを呼んでください。
    /// 運営側のシステムがミニゲームの開始を認識します。
    /// </summary>
    public static void Load()
    {
        isMinigameLoaded = true;
        Debug.Log($"<color=green>【System】Loadフラグが立ちました！ </color>");
    }

    /// <summary>
    /// ゲームの目的を達成したときにこれを呼んでください。
    /// 運営側のシステムが「成功」として検知します。
    /// </summary>
    public static void ClearGame()
    {
        IsClear = true;
        Debug.Log($"<color=green>【System】Clearフラグが立ちました！ </color>");
    }

    /// <summary>
    /// テストプレイがしたい場合に呼び出してください。
    /// 入力されたステージ数に応じて、timeScaleを調整します。
    /// <summary>
    public static void TestPlay(int s)
    {   
        if (!isMainCalled)
        {
            timeScale = Stage2TimeScale(s);
            Time.timeScale = timeScale;
            stage = s;
            Debug.Log($"<color=green>【System】ステージ{s}での速度が再現されます。(timeScale={timeScale}) </color>");
            isDebugMode = false;
        }
    }

    /// これより下の関数(メソッド)は呼び出さないでください。
    
    /// 同じシーンをロードし続ける
    public static void stuckScene(string s)
    {
        scene = s;
    }
    
    /// 次のステージに進む
    public static void nextStage()
    {
        stage += 1;
    }

    // タイムスケールを変更する
    public static void applyNewTimeScale()
    {
        timeScale = Stage2TimeScale(stage);
    }

    /// ミニゲームが終了したとき、フラグをリセットする。
    public static void Finished()
    {
        isMinigameLoaded = false;
        IsClear = false;
        isAllGameEndProcessed = false; // フラグもリセット
    }

    // ステージ数を入力したら、それに対応するtimeScaleを返す
    private static float Stage2TimeScale(int s)
    {
        // 初期状態では1.0倍、その5ステージ進むごとに0.2ずつ増やす。
        // 1.6倍にまで到達したら、その後は5ステージごとに0.3ずつ増やす。
        // 上限はない。
        if (s <= 15)
        {
            return 1.0f + 0.05f * ((s - 1) / 5);
        }
        else
        {
            return 1.15f + 0.05f * ((s - 16) / 10);
        }

    }
}
