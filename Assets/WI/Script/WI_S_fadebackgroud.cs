using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

namespace WI
{
    public class WI_S_fadebackgroud : MiniGameBase
    {
        private float fadeout = 7f;
        private SpriteRenderer spriterender;


        public override void OnGameStart()
        {
          spriterender = GetComponent<SpriteRenderer>();
        }

        public override void OnGameEnd()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
           Color tempColor = spriterender.color;

           if(tempColor.a > 0)
            {
                float step = 1.0f / fadeout * Time.deltaTime;
                tempColor.a -= step;

                spriterender.color = tempColor;
            }
        }
    }
}
