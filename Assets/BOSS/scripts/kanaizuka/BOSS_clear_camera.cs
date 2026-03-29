using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BOSS_clear_camera : MonoBehaviour
{
    [SerializeField] private float duration = 3f;
    [SerializeField] private float start_y = 5f;
    [SerializeField] private float end_y = 0f;

    public void clear_camera(){
        StartCoroutine(moving_camera());
    }

    IEnumerator moving_camera(){
        float elapsed = 0f;
        Vector3 start_pos = new Vector3(0f,start_y,-10f);
        Vector3 end_pos = new Vector3(0f,end_y,-10f);
        gameObject.transform.position = start_pos;

        while(elapsed < duration){
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(start_pos,end_pos,t);
            yield return null;
        }

        transform.position = end_pos;
    }
}
