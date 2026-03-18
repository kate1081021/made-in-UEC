using UnityEngine;

public class BOSS_bomb : MonoBehaviour
{
    [Header("諸数値")]
    [SerializeField] private float generate_duration = 7.0f;
    [SerializeField] private float generate_x_pos = 10.0f;
    [SerializeField] private float generate_y_pos = 0.0f;

    [Header("prefab")]
    public GameObject bomb_prefab;

    float time = 0.0f;

    void Update(){
        time += Time.deltaTime;

        if(time > generate_duration){
            bomb_generate();
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
}
