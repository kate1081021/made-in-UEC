using System.Collections;
using UnityEngine;

public class BOSS_bomb : MonoBehaviour
{
    [Header("諸数値")]
    [SerializeField] private float generate_duration = 7.0f; //爆弾の生成間隔
    [SerializeField] private float generate_x_pos = 10.0f; //爆弾の生成位置のx座標
    [SerializeField] private float generate_y_pos = 0.0f; //爆弾の生成位置のy座標
    [SerializeField] private float throwing_duration = 0.5f; //爆弾をなげてから着弾するまでの時間
    [SerializeField] private float rbble_throwing_duration = 2.0f; //瓦礫が落ちきる時間
    [SerializeField] private int left_ratio = 2; //制御点のx座標計算用
    [SerializeField] private int right_ratio = 1;

    [Header("着弾座標の上限下限")]
    [SerializeField] private float max_target_x = 5.0f;
    [SerializeField] private float min_target_x = -5.0f;
    [SerializeField] private float max_target_y = 3.5f;
    [SerializeField] private float min_target_y = 2.0f;

    [Header("prefab")]
    public GameObject bomb_prefab; //爆弾のオブジェクト
    public GameObject taeget_prefab; //ターゲットのオブジェクト
    public GameObject rubble_prefab; //瓦礫オブジェクト

    private float time = 0.0f; //時間計測用変数
    private float controll_pos_x; //2次ベジェ曲線の制御点のx座標を一時的に保存するための変数
    private float controll_pos_y; //2次ベジェ曲線の制御点のy座標を一時的に保存するための変数
    private int count = 0; //爆弾を投げた回数を数える用の変数
    private int bomb_number; //爆弾の個数を管理する用の変数

    void Update(){
        time += Time.deltaTime;

        if(time > generate_duration){
            GameObject bomb;
            GameObject target;
            //placeの値が0の時は右側に、そうでないときは左側に爆弾を生成
            int place = Random.Range(0,2);
        
            Vector3 spawn_pos;
            Vector3 controll_pos;

            if(place == 0){
                spawn_pos = new Vector3(generate_x_pos,generate_y_pos,0.0f);
            }else{
                spawn_pos = new Vector3(-generate_x_pos,generate_y_pos,0.0f);
            }

            bomb = Instantiate(bomb_prefab, spawn_pos, Quaternion.identity);

            //ターゲットの座標の決定と生成
            float target_x = Random.Range(min_target_x,max_target_x);
            float target_y = Random.Range(min_target_y,max_target_y);

            Vector3 target_pos = new Vector3(target_x,target_y,0.0f);

            target = Instantiate(taeget_prefab,target_pos,Quaternion.identity);

            //爆弾とターゲットのx座標の特定比率の内分点を制御点のx座標とする
            controll_pos_x = (left_ratio * spawn_pos.x + right_ratio * target_pos.x) / (left_ratio + right_ratio);
            controll_pos_y = target_pos.y + 1.5f;
            controll_pos = new Vector3(controll_pos_x,controll_pos_y,0.0f);

            //爆弾を投げる処理
            StartCoroutine(ThrowCoroutine(bomb,target,spawn_pos,target_pos,controll_pos));

            //タイマーリセット
            time = 0.0f;
        }
    }

    IEnumerator ThrowCoroutine(GameObject bomb, GameObject target, Vector3 start, Vector3 end,Vector3 controll)
    {
        GameObject rubble;

        float elapsed = 0f;

        while (elapsed < throwing_duration)
        {
            if (bomb == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / throwing_duration;

            Vector3 m1 = Vector3.Lerp(start,controll,t);
            Vector3 m2 = Vector3.Lerp(controll,end,t);

            bomb.transform.position = Vector3.Lerp(m1,m2,t);

            yield return null;
        }

        // 到着後の処理
        if (bomb != null) bomb.transform.position = end;
        
        Destroy(bomb);
        Destroy(target);
        
        bomb_number = (count / 2 + 1) * 2;
        count++;

        for(int i = 0;i < bomb_number;i++){
            rubble = Instantiate(rubble_prefab,end,Quaternion.identity);
            Vector3 rand_target;

            if(i % 2 == 0){
                rand_target = new Vector3(end.x + Random.Range(1f,8f),-7f,0f);
            }else{
                rand_target = new Vector3(end.x + Random.Range(-1f,-8f),-7f,0f);
            }

            if(rubble.TryGetComponent<BOSS_rubble_moving>(out var mov)){
                mov.Initialize(end,rand_target,rbble_throwing_duration);
            }
        }
    }
}
