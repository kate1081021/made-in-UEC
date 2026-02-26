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
    public GameObject hand; 

    [Header("諸設定")]
    public Transform ThrowTarget;
    public float throwDuration = 0.3f;
    [SerializeField] LF_handChange handChange;

    public void StartClear(){
        StartCoroutine(ClearMovie());
    }

    private IEnumerator ClearMovie(){
        // game用オブジェクトを非表示
        loupe.SetActive(false); 
        match.SetActive(false); 

        // 爆弾と手を表示
        bomb.SetActive(true);
        hand.SetActive(true);
        Vector3 startPos = bomb.transform.position;

        // 待機
        yield return new WaitForSeconds(0.3f);

        // 手の画像の切り替え
        handChange.change();

        // 座標の移動
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
