using UnityEngine;

public class EA_CrossButtonView : MonoBehaviour
{
    // スプライト
    public Sprite[] assets;  // normal: 0, right: 1, down: 2

    // コンポーネント
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 十字キーが押された
    public void Pressed(int idx)
    {
        spriteRenderer.sprite = assets[idx];
    }
}
