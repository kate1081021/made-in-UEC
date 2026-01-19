using UnityEngine;

public static class MGManager
{
    /// <summary> 
    /// ミニゲームのロードを確認する
    /// </summary>
    public static bool isMinigameLoaded { get; private set; } = false;

    public static void Loaded()
    {
        isMinigameLoaded = true;
        Debug.Log($"<color=green>【System】Loadフラグが立ちました！ </color>");
    }

    /// <summary> 運営が現在のクリア状況を確認するためのプロパティ </summary>
    public static bool IsClear { get; private set; } = false;

    // --- 部員が自由に使える便利関数 ---

    /// <summary>
    /// ゲームの目的を達成したときにこれを呼んでください。
    /// 運営側のシステムが「成功」として検知します。
    /// </summary>
    public static void ClearGame()
    {
        IsClear = true;
        Debug.Log($"<color=green>【System】Clearフラグが立ちました！ </color>");
    }

    public static void Finished()
    {
        isMinigameLoaded = false;
        IsClear = false;
    }
}
