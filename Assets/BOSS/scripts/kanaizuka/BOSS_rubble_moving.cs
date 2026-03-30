using System.Collections;
using UnityEngine;

public class BOSS_rubble_moving : MonoBehaviour
{
    public void Initialize(Vector3 start,Vector3 end,float duration){
        StartCoroutine(Rubble_moving(start,end,duration));
    }

    IEnumerator Rubble_moving(Vector3 start,Vector3 end,float duration){
        float elapsed = 0f;

        float controll_pos_x = (start.x + end.x) / 2f;
        float controll_pos_y = start.y + 4.0f;

        Vector3 controll = new Vector3 (controll_pos_x,controll_pos_y,0f);

        while(elapsed < duration){
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            Vector3 m1 = Vector3.Lerp(start,controll,t);
            Vector3 m2 = Vector3.Lerp(controll,end,t);
            
            gameObject.transform.position = Vector3.Lerp(m1,m2,t);

            yield return null;
        }
        Destroy(gameObject);
    }
}
