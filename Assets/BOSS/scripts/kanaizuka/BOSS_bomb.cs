using UnityEngine;

public class BOSS_bomb : MonoBehaviour
{
    [Header("諸数値")]
    [SerializeField] private float generate_duration = 7.0f; //爆弾の生成間隔
    [SerializeField] private float generate_x_pos = 10.0f; //爆弾の生成位置のx座標
    [SerializeField] private float generate_y_pos = 0.0f; //爆弾の生成位置のy座標

    [Header("prefab")]
    public GameObject bomb_prefab; //爆弾のオブジェクト
    public GameObject taeget_prefab; //ターゲットのオブジェクト

    float time = 0.0f; //時間計測用変数

    void Update(){
        time += Time.deltaTime;

        if(time > generate_duration){
            bomb_generate();

            float target_x = Random.Range(-5.0f,5.0f);
            float target_y = Random.Range(2.0f,5.0f);

            Vector3 target_pos = new Vector3(target_x,target_y,0.0f);

            taeget_generate(target_pos);
            time = 0.0f;
        }
    }

    private void bomb_generate(){
        int place = Random.Range(0,2);
        
        Vector3 spawn_pos;

        if(place == 0){
            spawn_pos = new Vector3(generate_x_pos,generate_y_pos,0.0f);
        }else{
            spawn_pos = new Vector3(-generate_x_pos,generate_y_pos,0.0f);
        }

        Instantiate(bomb_prefab, spawn_pos, Quaternion.identity);
    }

    private void taeget_generate(Vector3 target_pos){
        Instantiate(taeget_prefab, target_pos, Quaternion.identity);
    }
}
