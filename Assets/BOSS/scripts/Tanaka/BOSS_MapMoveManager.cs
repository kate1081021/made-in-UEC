using UnityEngine;

public class BOSS_MapMoveManager : MonoBehaviour
{
    public float BOSS_MapSpeed;
    Rigidbody2D BOSSMaprigidbody2D;
    void Start()
    {
        BOSSMaprigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        BOSSMaprigidbody2D.linearVelocityY = BOSS_MapSpeed;
    }
}
