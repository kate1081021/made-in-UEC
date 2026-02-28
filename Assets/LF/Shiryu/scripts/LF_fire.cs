using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LF_fire : MonoBehaviour
{
    [Header("諸設定")]
    public Transform FireGoal;
    public Transform FireRestart;
    public GameObject bomb;
    public GameObject match;
    public float fireDuration = 0.5f;
    public float lastDuration = 0.2f;

    public void StartFire(){
        StartCoroutine(FireMovie());
    }

    private IEnumerator FireMovie(){

        Vector3 startPos = transform.position;

        // 座標の移動
        float elapsed = 0;
        while (elapsed < fireDuration){
            elapsed += Time.deltaTime;
            float t = elapsed / fireDuration;

            transform.position = Vector3.Lerp(startPos, FireGoal.position, t);

            yield return null;
        }

        Vector3 lastgoal = new Vector3(bomb.transform.position.x,startPos.y,transform.position.z);

        transform.position = FireRestart.position;

        elapsed = 0;
        startPos = transform.position;
        
        while (elapsed < lastDuration){
            elapsed += Time.deltaTime;
            float t = elapsed / lastDuration;

            transform.position = Vector3.Lerp(startPos, lastgoal, t);

            yield return null;
        }
    }
}
