using UnityEngine;
using UnityEngine.InputSystem;

public class UT_playermove : MonoBehaviour
{
    [SerializeField] private float movespeed;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector2 pos = moveInput.normalized * movespeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + pos);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
