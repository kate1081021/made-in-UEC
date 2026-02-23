using UnityEngine;

[CreateAssetMenu(fileName = "CreateScene", menuName = "CreateScene")]
public class CreateScene : ScriptableObject
{
    public string scene_name;  // 呼び出すシーンの名前
    public string verb;  // ゲームを表す動詞
    public float timelimit;  // ミニゲームの制限時間(1.0xを基準とする)
    public bool stopEarlyFinish; // 早期クリア時の切り上げを止めるオプション(通常時はfalse)
    
}
