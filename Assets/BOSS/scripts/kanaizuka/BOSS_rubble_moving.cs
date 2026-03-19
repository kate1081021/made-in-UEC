using UnityEngine;

public class BOSS_rubble_moving : MonoBehaviour
{
    [Header("諸数値")]
    [SerializeField] private float number;
    [SerializeField] private float dx = 2.0f;
    [SerializeField] private float dy = 5.0f;

    //スタート使ってよいのかちょっと不安だったのとめちゃ眠いからいったんオブジェクトの削除を時間で管理するカス
    private float time;
    private float pos_x;
    private float pos_y;
    Vector3 pos;

    void Update()
    {
        pos_x = gameObject.transform.position.x - dx * Time.deltaTime;
        pos_y = gameObject.transform.position.y - dy * Time.deltaTime;
        time += Time.deltaTime;
        pos = new Vector3(pos_x,pos_y,0.0f);
        transform.position = pos;

        if(time > 2.0f){
            Destroy(gameObject);
        }
    }
}
