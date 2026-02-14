using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 【ろっぱち】メイド・イン・UEC ミニゲーム用ベースクラス
/// 全てのミニゲームはこのクラスを継承して作成してください。
/// </summary>
public abstract class BaseScript : MonoBehaviour
{
    protected virtual void Start() {}
}
public abstract class MiniGameBase : BaseScript, IMiniGame
{
    [Header("--- 運営設定エリア ---")]
    [Tooltip("このゲームで流したいBGM。未設定ならデフォルトBGMが流れます")]
    [SerializeField] private AudioClip gameBGM;

    
    /// <summary> 運営がBGMを取得するためのプロパティ </summary>
    public AudioClip GameBGM => gameBGM;

    protected MIU_InputSystem InputSystems;
    protected InputAction Move;
    protected InputAction Trigger;
    protected InputAction Action;
    protected Vector2 moveValue;
    protected float triggerValue;
    protected bool actionValue;

    // --- Unity標準機能の制限 ---

    /// <summary>
    /// Startは使用禁止です！代わりに OnGameStart() を使ってください。
    /// 初期化漏れによるバグを防ぐため、運営側で封印しています。
    /// </summary>
    protected sealed override void Start()
    {
        InputSystems = new MIU_InputSystem();
        InputSystems.Disable();
        Move = InputSystems.FindAction("Move");  // WASD
        Trigger = InputSystems.FindAction("Trigger");  // Enter
        Action = InputSystems.FindAction("Action");  // Space
        OnGameStart();
    }

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
    public virtual void OnGameEnd() { }

    // 「メモリ解放」を忘れがちなので、ベース側でケアします
    protected virtual void OnDestroy()
    {
        if (InputSystems != null)
        {
            InputSystems.Disable();
            InputSystems.Dispose();
        }
        OnGameEnd();
    }
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

