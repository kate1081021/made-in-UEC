using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
    [SerializeField] private bool isLoopBGM = false;
    [SerializeField] private List<string> SEName;
    [SerializeField] private List<AudioClip> SEFile;
    private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();
    private AudioSource mainSource;

    
    /// <summary> 運営がBGMを取得するためのプロパティ </summary>
    public AudioClip GameBGM => gameBGM;

    protected MIU_InputSystem InputSystems;
    protected InputAction Move;
    protected InputAction Trigger;
    protected InputAction Action;
    protected Vector2 moveValue;
    protected float triggerValue;
    protected float actionValue;
    protected InputAction Trigger_left;
    protected InputAction Trigger_right;
    protected float triggerleftValue;
    protected float triggerrightValue;

    // --- Unity標準機能の制限 ---

    /// <summary>
    /// Startは使用禁止です！代わりに OnGameStart() を使ってください。
    /// 初期化漏れによるバグを防ぐため、運営側で封印しています。
    /// </summary>
    protected sealed override void Start()
    {
        InputSystems = new MIU_InputSystem();
        InputSystems.Enable();
        Move = InputSystems.FindAction("Move");  // WASD
        Trigger = InputSystems.FindAction("Trigger");  // q/e
        Trigger_left = InputSystems.FindAction("Trigger_left");  // q
        Trigger_right = InputSystems.FindAction("Trigger_right");  // e
        Action = InputSystems.FindAction("Action");  // Space

        if(SEName.Count != 0){
            for (int i = 0; i < SEName.Count; i++)
            {
                soundEffects.Add(SEName[i],SEFile[i]);
            }
        }
        mainSource = gameObject.AddComponent<AudioSource>();
        Debug.Log($"BGM Volume: {MGManager.sound_volume}");
        ApplyVolume(MGManager.sound_volume);

        Move.performed += OnMove;
        Move.canceled += OnMove;
        Move.started += OnMove;
        Trigger.started += OnTrigger;
        Trigger.performed += OnTrigger;
        Trigger.canceled += OnTrigger;
        Action.started += OnAction;
        Action.performed += OnAction;
        Action.canceled += OnAction;
        Trigger_left.started += OnTriggerLeft;
        Trigger_left.performed += OnTriggerLeft;
        Trigger_left.canceled += OnTriggerLeft;
        Trigger_right.started += OnTriggerRight;
        Trigger_right.performed += OnTriggerRight;
        Trigger_right.canceled += OnTriggerRight;

        OnGameStart();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveValue = ctx.ReadValue<Vector2>();

        if (ctx.started)   {OnMoveStarted(moveValue);}
        if (ctx.performed) {OnMovePerformed(moveValue);}
        if (ctx.canceled)  {OnMoveCanceled(moveValue);}

    }
    public void OnTrigger(InputAction.CallbackContext ctx)
    {
        triggerValue = ctx.ReadValue<float>();

        if (ctx.started)   {OnTriggerStarted(triggerValue);}
        if (ctx.performed) {OnTriggerPerformed(triggerValue);}
        if (ctx.canceled)  {OnTriggerCanceled(triggerValue);}

    }
    public void OnAction(InputAction.CallbackContext ctx)
    {
        actionValue = ctx.ReadValue<float>();

        if (ctx.started)   {OnActionStarted(actionValue);}
        if (ctx.performed) {OnActionPerformed(actionValue);}
        if (ctx.canceled)  {OnActionCanceled(actionValue);}
    }

    public void OnTriggerLeft(InputAction.CallbackContext ctx)
    {
        triggerleftValue = ctx.ReadValue<float>();

        if (ctx.started)   {OnTriggerLeftStarted(triggerleftValue);}
        if (ctx.performed) {OnTriggerLeftPerformed(triggerleftValue);}
        if (ctx.canceled)  {OnTriggerLeftCanceled(triggerleftValue);}

    }

    public void OnTriggerRight(InputAction.CallbackContext ctx)
    {
        triggerrightValue = ctx.ReadValue<float>();

        if (ctx.started)   {OnTriggerRightStarted(triggerrightValue);}
        if (ctx.performed) {OnTriggerRightPerformed(triggerrightValue);}
        if (ctx.canceled)  {OnTriggerRightCanceled(triggerrightValue);}
    }
    /*
    このStartについては他に上書きすべきメソッドがない場合エラーを履いちゃうからコメントアウトした
    */

    // 便利関数一覧 使いたいときにぜひ使ってね
    public Vector2 convert_stick_to_dir(Vector2 val)
    {
        float mag = val.magnitude;
        Vector2 ans = new Vector2(0,0);
        if (mag < 0.5) { return ans; }
        float theta = Mathf.Atan2(val.y,val.x) * Mathf.Rad2Deg;
        if (-45 < theta && theta <= 45)
        {
            ans = new Vector2(1,0);
        } else if (45 < theta && theta <= 135)
        {
            ans = new Vector2(0,1);
        } else if (-135 < theta && theta <= -45)
        {
            ans = new Vector2(0,-1);
        } else
        { ans = new Vector2(-1,0); } //左に-180と180の境目があってめんどくさい
        return ans;
    }

    protected virtual void OnMoveStarted(Vector2 value) {}
    protected virtual void OnMovePerformed(Vector2 value) {}
    protected virtual void OnMoveCanceled(Vector2 value) {}
    protected virtual void OnTriggerStarted(float value) {}
    protected virtual void OnTriggerPerformed(float value) {}
    protected virtual void OnTriggerCanceled(float value) {}
    protected virtual void OnActionStarted(float value) {}
    protected virtual void OnActionPerformed(float value) {}
    protected virtual void OnActionCanceled(float value) {}
    protected virtual void OnTriggerLeftStarted(float value) {}
    protected virtual void OnTriggerLeftPerformed(float value) {}
    protected virtual void OnTriggerLeftCanceled(float value) {}
    protected virtual void OnTriggerRightStarted(float value) {}
    protected virtual void OnTriggerRightPerformed(float value) {}
    protected virtual void OnTriggerRightCanceled(float value) {}


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

        MGManager.ActiveMiniGames.Remove(this);
        if (Move != null)
        {
            Move.performed -= OnMove;
            Move.canceled -= OnMove;
            Move.started -= OnMove;
        }
        if (Trigger != null)
        {
            Trigger.started -= OnTrigger;
            Trigger.performed -= OnTrigger;
            Trigger.canceled -= OnTrigger;
        }
        if (Action != null)
        {
            Action.started -= OnAction;
            Action.performed -= OnAction;
            Action.canceled -= OnAction;
        }
        if (Trigger_left != null)
        {
            Trigger_left.started -= OnTriggerLeft;
            Trigger_left.performed -= OnTriggerLeft;
            Trigger_left.canceled -= OnTriggerLeft;
        }
        if (Trigger_right != null)
        {
            Trigger_right.started -= OnTriggerRight;
            Trigger_right.performed -= OnTriggerRight;
            Trigger_right.canceled -= OnTriggerRight;
        }
        if (InputSystems != null)
        {
            InputSystems.Disable();
            InputSystems.Dispose();
        }
        Debug.Log("OnGameEnd");
    }
    /// <summary>
    /// ゲーム終了時に、このプレハブ内から出ている全ての音を止めます。
    /// 運営側で終了時に自動実行することを想定しています。
    /// </summary>
    public void BGMPlay(bool applyToTimeScale = false)
    {
        mainSource.clip = gameBGM;

        if (applyToTimeScale)
        {
            mainSource.pitch = MGManager.timeScale;
        }
        else
        {
            mainSource.pitch = MGManager.pitchScale;
        }
        if (isLoopBGM) { mainSource.loop = true; }
        mainSource.Play();
    }

    public void BGMStop()
    {
        mainSource.Stop();
    }

    public void SEPlay(string id, bool applyToTimeScale = false)
    {
        if (applyToTimeScale)
        {
            mainSource.pitch = MGManager.timeScale;
        }
        else
        {
            mainSource.pitch = MGManager.pitchScale;
        }
        mainSource.PlayOneShot(soundEffects[id]);
    }

    // 音量の設定を適応する
    public void ApplyVolume(float value)
    {
        mainSource.volume = value;
    }

    // 二重実行防止用のフラグ
    private bool isEndProcessed = false;

    protected virtual void OnEnable()
    {
        // リストに自分を追加
        if (!MGManager.ActiveMiniGames.Contains(this))
        {
            MGManager.ActiveMiniGames.Add(this);
        }
    }



    // 運営側から一斉に呼ばれる終了関数
    public void ExecuteGameEnd()
    {
        if (isEndProcessed) return; // 既に実行済みならスルー

        OnGameEnd(); // 部員が書いた終了処理（判定更新など）を実行
        isEndProcessed = true;
    }
}

