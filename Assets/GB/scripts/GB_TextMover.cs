using UnityEngine;
using System.Collections.Generic;

namespace garbage
{
    public class GB_TextMover : MiniGameBase
    {
        float initialtime, currenttime;
        [SerializeField] SpriteRenderer sr;
        [SerializeField] Sprite notEcoFriendly;
        [SerializeField] GameObject manager;
        GB_GameManagingScript managingscript;
        public override void OnGameStart()
        {
            initialtime = Time.time;
            sr = GetComponent<SpriteRenderer>();
            managingscript = manager.GetComponent<GB_GameManagingScript>();
            if (managingscript.SuccessOrFailure == -1) { sr.sprite = notEcoFriendly; }
        }
        void Update()
        {
            currenttime = Time.time;
            int t = (int)((currenttime - initialtime) * 100);
            if (t < 100)
            {
                Color c = sr.color;
                c.a = (0.02f * t < 1) ? 0.02f * t : 1;
                sr.color = c;
            }
        }
    }
}