using UnityEngine;

/// <summary>
/// 【ろっぱち】メイド・イン・UEC ミニゲーム用ベースクラス
/// 全てのミニゲームはこのクラスを継承して作成してください。
/// </summary>
public abstract class MiniGameBase : MonoBehaviour, IMiniGame
{
    [Header("--- 運営設定エリア ---")]
    [Tooltip("このゲームで流したいBGM。未設定ならデフォルトBGMが流れます")]
    [SerializeField] private AudioClip gameBGM;

    
    /// <summary> 運営がBGMを取得するためのプロパティ </summary>
    public AudioClip GameBGM => gameBGM;

    // --- Unity標準機能の制限 ---

    /// <summary>
    /// Startは使用禁止です！代わりに OnGameStart() を使ってください。
    /// 初期化漏れによるバグを防ぐため、運営側で封印しています。
    /// </summary>
    // private sealed void Start() { }
    /*
    このStartについては他に上書きすべきメソッドがない場合エラーを履いちゃうからコメントアウトした
    */

    // --- 部員が必ず実装（オーバーライド）する関数 ---

    /// <summary>
    /// ゲームが開始された瞬間に呼ばれます。
    /// オブジェクトの初期位置、タイマーの開始、キャラの生成などはここに書いてください。
    /// </summary>
    public abstract void OnGameStart();

    /// <summary>
    /// ゲーム時間が終了した瞬間に呼ばれます。
    /// 入力を受け付けなくしたり、アニメーションを止めたりする後処理を書いてください。
    /// </summary>
    public abstract void OnGameEnd();

    /// <summary>
    /// ゲーム終了時に、このプレハブ内から出ている全ての音を止めます。
    /// 運営側で終了時に自動実行することを想定しています。
    /// </summary>
    public void StopAllSounds()
    {
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        foreach (var source in sources)
        {
            source.Stop();
        }
    }
}

