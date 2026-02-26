using UnityEngine;

public class UT_rule
{
    /*
     scriptフォルダ内に各弾幕に対応するフォルダを作ってその中にその弾幕の弾やスクリプトを作っていってください。
     create emptyをして○○generator (○○は弾幕の名前)みたいな名前を付けて
    最終的にそれをヒエラルキーにおいておけばゲーム開始時に弾幕が生成されるようにしていただけると助かります。
    実際のゲームでは開始時にランダムでその○○generatorを1個生成する感じでいいかなと思っています。
    弾の当たり判定について、colliderをつけてIs triggerをオンにしてTagをbulletにすることで当たり判定が実装できるはずです。

    ○○generatorに着けたスクリプト内の
    public class ○○ : MiniGameBase
    の後に
        public UT_playermove pm;
    と宣言し、
    OnGameStart内で
        pm = GameObject.Find("Player").GetComponent<UT_playermove>();
        pm.generator = gameObject;
    としてください。また、生み出したオブジェクトには全てbulletタグをつけておいてください。
    ゲーム終了時にbulletタグがついたものを全て削除する、という形にしているためつけないと残ってしまい
    次のゲームで不具合が起こる可能性があります。
    弾幕の制限時間は同じくOnGameStart内で
        pm.timelimit = ○○f;
    とすることで設定できます。デフォルトは15秒です。


    何か疑問点などありましたらDiscord上などでいつでもご連絡ください。
     */
}
