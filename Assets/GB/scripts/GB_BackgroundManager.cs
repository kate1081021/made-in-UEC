using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace garbage
{
    public class GB_BackgroundManager : MiniGameBase
    {
        [SerializeField] SpriteRenderer sr;
        [SerializeField] Sprite cloudy;
        public override void OnGameStart()
        {
            sr = GetComponent<SpriteRenderer>();
        }
        public void cloudify()
        {
            sr.sprite = cloudy;
        }
    }
}