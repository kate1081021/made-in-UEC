using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LF_match : MonoBehaviour
{
    [Header("画像リスト")]
    public Sprite[] sprites;

    [Header("切り替え間隔")]
    public float interval = 0.1f;

    [Header("画像数")]
    private int max = 5;

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;
    private bool loop = true;

    public void StartFire(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(RotateImage());
    }

    IEnumerator RotateImage(){
        while(loop){
            // spriteRenderer.sprite = sprites[currentIndex];
            Vector3 pos = transform.position;
            pos.x -= 1.8f;
            transform.position = pos;

            currentIndex += 1;
            if(currentIndex == max){
                loop = false;
            } 

            yield return new WaitForSeconds(interval);
        }
    }
}
