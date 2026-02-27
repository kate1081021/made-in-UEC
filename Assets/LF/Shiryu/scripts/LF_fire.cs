using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LF_fire : MonoBehaviour
{
    [Header("諸設定")]
    public Transform FireGoal;
    public float throwDuration = 0.5f;

    public void StartFire(){
        StartCoroutine(FireMovie());
    }

    private IEnumerator FireMovie(){

        Vector3 startPos = transform.position;
        
        // 座標の移動
        float elapsed = 0;
        while (elapsed < throwDuration){
            elapsed += Time.deltaTime;
            float t = elapsed / throwDuration;

            transform.position = Vector3.Lerp(startPos, FireGoal.position, t);

            yield return null;
        }
    }
}
