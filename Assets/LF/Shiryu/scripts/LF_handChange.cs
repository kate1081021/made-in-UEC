using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LF_handChange : MonoBehaviour
{
    public Sprite newSprite;
    private SpriteRenderer image;

    public void change(){
        image = GetComponent<SpriteRenderer>();
        image.sprite = newSprite;
    }
}
