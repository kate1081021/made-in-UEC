using UnityEngine;

public class EA_AButtonView : MonoBehaviour
{
    // スプライト
    public Sprite normal;
    public Sprite pressed;

    // コンポーネント
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Aボタンが押された
    public void Pressed(bool flag)
    {
        if (flag)
        {
            spriteRenderer.sprite = pressed;
        }
        else
        {
            spriteRenderer.sprite = normal;
        }
    }
}
