using NT;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace garbage
{
    public class GB_GarbageMover : MiniGameBase
    {
        [SerializeField] GameObject manager;
        GB_GameManagingScript managingscript;
        [SerializeField] float initialtime, currenttime, initialmovingtime;
        bool isMoving = false;
        bool blinking = false;
        [SerializeField] SpriteRenderer sr;
        public override void OnGameStart()
        {
            manager = GameObject.Find("GB_GameManager");
            managingscript = manager.GetComponent<GB_GameManagingScript>();
            sr = GetComponent<SpriteRenderer>();
            initialtime = Time.time;
            if (MGManager.stage >= 30){ blinking = true; }
        }

        void Update()
        {
            currenttime = Time.time;
            if (!isMoving && currenttime - initialtime > 1f)
            {
                isMoving = true;
                initialmovingtime = currenttime;
            }
            if (isMoving)
            {
                garbageMovement(Vector2.down);
                if (blinking)
                {
                    int t = (int)((currenttime - initialmovingtime) * 100) % 160;
                    Color color = sr.color;
                    color.a = Mathf.Abs(-0.0125f * t + 1f);
                    sr.color = color;
                    //Debug.Log(color);
                }
            }
            if (transform.position.y < -3.3f)
            {
                if (managingscript.SuccessOrFailure == 0 && !managingscript.judge)
                {
                    managingscript.judge = true;
                }
                Destroy(this.gameObject);
            }
        }
        private void garbageMovement(Vector3 moveDirection)
        {
            var pos = transform.position;

            //var moveSpeed = 1.2f * Time.timeScale;

            //pos += moveDirection * moveSpeed * Time.deltaTime;
            pos.y = 3.3f + (currenttime - initialmovingtime) * (-3.3f - 3.3f) / (initialtime + 6f - initialmovingtime); 

            transform.position = pos;
        }
    }
}