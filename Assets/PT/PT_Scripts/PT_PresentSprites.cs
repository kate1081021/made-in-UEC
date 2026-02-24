using System.Collections.Generic;
using UnityEngine;

namespace PTgame
{
    public class PT_PresentSprites : MonoBehaviour
    {
        [SerializeField] private List<Sprite> presentSprites; // プレゼントテクスチャ制御 追加
        [SerializeField] private SpriteRenderer sr;

        public void Awake()
        {
            if (sr != null && presentSprites.Count > 0)
            {
                // ランダムな新しいスプライト適用
                sr.sprite = presentSprites[Random.Range(0, presentSprites.Count)];
            }
        }
    }
}