using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LF_SceneChange : MonoBehaviour
{
    [Header("ゲーム用オブジェクト")]
    public GameObject loupe;
    public GameObject match;

    [Header("演出用オブジェクト")]
    public GameObject backgroundImage;
    public GameObject bomb;
    public GameObject explpsion;

    [Header("諸設定")]
    public Transform ThrowTarget;
    public float throwDuration = 0.3f;

    public void StartClear(){
        StartCoroutine(ClearMovie());
    }

    private IEnumerator ClearMovie(){
        loupe.SetActive(false);
        match.SetActive(false);

        bomb.SetActive(true);
        Vector3 startPos = bomb.transform.position;

        yield return new WaitForSeconds(0.2f);

        float elapsed = 0;
        while (elapsed < throwDuration){
            elapsed += Time.deltaTime;
            float t = elapsed / throwDuration;

            bomb.transform.position = Vector3.Lerp(startPos, ThrowTarget.position, t);

            yield return null;
        }

        explpsion.transform.position = bomb.transform.position;
        explpsion.SetActive(true);
        bomb.SetActive(false);
    }
}
