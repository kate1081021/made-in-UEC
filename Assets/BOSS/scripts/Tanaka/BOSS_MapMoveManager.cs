using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BOSS_MapMoveManager : MiniGameBase
{
    public float BOSS_MapSpeed;
    Rigidbody2D BOSSMaprigidbody2D;
    Transform BOSSTransform;
    public override void OnGameStart()
    {
        BOSSMaprigidbody2D = this.GetComponent<Rigidbody2D>();
        BOSSTransform = this.GetComponent<Transform>();
    }

    void Update()
    {
        BOSSMaprigidbody2D.linearVelocityY = BOSS_MapSpeed;
        if (BOSSTransform.transform.position.y < -100)
        {
            BOSSTransform.transform.position = new Vector3(BOSSTransform.transform.position.x, 100, BOSSTransform.transform.position.z);
        }
    }
}
