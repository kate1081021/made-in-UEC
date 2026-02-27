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
    public GameObject fire;
    [SerializeField] AudioClip bakuhatsu;
    AudioSource audioSource;

    [Header("諸設定")]
    public Transform ThrowTarget;
    public float throwDuration;
    [SerializeField] LF_handChange handChange;
    [SerializeField] LF_fire fireMove;

    public void StartClear(){
        throwDuration = 0.3f / Time.timeScale;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(ClearMovie());
    }

    private IEnumerator ClearMovie(){
        // ルーペを非表示にし火を表示
        loupe.SetActive(false);
        
        // 1. まず fire オブジェクトを有効化
        fire.SetActive(true);

        // 2. fireMove がついている本体の状態を詳しくチェック
        if (fireMove != null) {
            fireMove.gameObject.SetActive(true); // 念のため自分自身をオンにする

            // 【重要】ここで状態をログに出す
            Debug.Log($"fireMove自体の状態: {fireMove.gameObject.activeSelf}");
            Debug.Log($"シーン内での実質的な状態: {fireMove.gameObject.activeInHierarchy}");

            if (fireMove.gameObject.activeInHierarchy) {
                fireMove.StartFire();
            } else {
                Debug.LogError("警告: 親オブジェクトの誰かがオフなので、fireMoveはまだ眠ったままです！");
            }
        }

        yield return new WaitForSeconds(0.5f / Time.timeScale);
        
        // 爆弾と手を表示
        bomb.SetActive(true);
        hand.SetActive(true);
        Vector3 startPos = bomb.transform.position;

        // 待機
        yield return new WaitForSeconds(0.5f / Time.timeScale);

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
        audioSource.PlayOneShot(bakuhatsu);
        explpsion.SetActive(true);
        bomb.SetActive(false);
    }
}
