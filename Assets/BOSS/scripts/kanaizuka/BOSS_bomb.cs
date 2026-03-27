using System.Collections;
using UnityEngine;

public class BOSS_bomb : MonoBehaviour
{
    [Header("諸数値")]
    [SerializeField] private float generate_duration = 7.0f; //爆弾の生成間隔
    [SerializeField] private float generate_x_pos = 10.0f; //爆弾の生成位置のx座標
    [SerializeField] private float generate_y_pos = 0.0f; //爆弾の生成位置のy座標
    [SerializeField] private float throwing_duration = 0.5f; //爆弾をなげてから着弾するまでの時間

    [Header("着弾座標の上限下限")]
    [SerializeField] private float max_target_x = 5.0f;
    [SerializeField] private float min_target_x = -5.0f;
    [SerializeField] private float max_target_y = 5.0f;
    [SerializeField] private float min_target_y = 2.0f;

    [Header("prefab")]
    public GameObject bomb_prefab; //爆弾のオブジェクト
    public GameObject taeget_prefab; //ターゲットのオブジェクト
    public GameObject rubble_1_prefab;
    public GameObject rubble_2_prefab;
    public GameObject rubble_3_prefab;
    public GameObject rubble_4_prefab;
    public GameObject rubble_5_prefab;
    public GameObject rubble_6_prefab;

    float time = 0.0f; //時間計測用変数

    void Update(){
        time += Time.deltaTime;

        if(time > generate_duration){
            GameObject bomb;
            GameObject target;
            //placeの値が0の時は右側に、そうでないときは左側に爆弾を生成
            int place = Random.Range(0,2);
        
            Vector3 spawn_pos;

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

            //爆弾を投げる処理
            StartCoroutine(ThrowCoroutine(bomb,target,spawn_pos,target_pos));

            //タイマーリセット
            time = 0.0f;
        }
    }

    IEnumerator ThrowCoroutine(GameObject bomb, GameObject target, Vector3 start, Vector3 end)
    {
        GameObject rubble_1;
        GameObject rubble_2;
        GameObject rubble_3;
        GameObject rubble_4;
        //以下2つは後ほど実装予定
        // GameObject rubble_5;
        // GameObject rubble_6;

        float elapsed = 0f;

        while (elapsed < throwing_duration)
        {
            if (bomb == null) yield break;

            elapsed += Time.deltaTime;
            float ratio = elapsed / throwing_duration;

            bomb.transform.position = Vector3.Lerp(start, end, ratio);

            yield return null;
        }

        // 到着後の処理
        if (bomb != null) bomb.transform.position = end;
        
        Destroy(bomb);
        Destroy(target);
        //瓦礫を発生させる処理、とりあえず仮実装で4つ生成させる。後ほど爆発した時の時間によって発生する瓦礫の個数を変化させるスクリプトに修正予定。
        rubble_1 = Instantiate(rubble_1_prefab,end,Quaternion.identity);
        rubble_2 = Instantiate(rubble_2_prefab,end,Quaternion.identity);
        rubble_3 = Instantiate(rubble_3_prefab,end,Quaternion.identity);
        rubble_4 = Instantiate(rubble_4_prefab,end,Quaternion.identity);
    }
}
