using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Runtime.CompilerServices;
using System.ComponentModel;

public class GameOverManager : MonoBehaviour
{
    private int choice = 0;
    private MIU_InputSystem InputSystems;
    private InputAction Move;
    private InputAction Action;
    [SerializeField] Transform cursor;

    void Start()
    {
        InputSystems = new MIU_InputSystem();
        InputSystems.Enable();
        Move = InputSystems.FindAction("Move");  // WASD
        Action = InputSystems.FindAction("Action");  // Space
        Move.performed += moving;
        Action.started += choiced;
        cursorUpdate();
    }
    private Vector2 convert_stick_to_dir(Vector2 val)
    {
        float mag = val.magnitude;
        Vector2 ans = new Vector2(0,0);
        if (mag < 0.5) { return ans; }
        float theta = Mathf.Atan2(val.y,val.x) * Mathf.Rad2Deg;
        if (-45 < theta && theta <= 45)
        {
            ans = new Vector2(1,0);
        } else if (45 < theta && theta <= 135)
        {
            ans = new Vector2(0,1);
        } else if (-135 < theta && theta <= -45)
        {
            ans = new Vector2(0,-1);
        } else
        { ans = new Vector2(-1,0); } //左に-180と180の境目があってめんどくさい
        return ans;
    }
    void moving(InputAction.CallbackContext ctx)
    {
        Vector2 direction = ctx.ReadValue<Vector2>();
        convert_stick_to_dir(direction);
        if (direction == Vector2.zero) { return; }
            if (direction == new Vector2(-1.0f,0))
            {
                choice = Math.Max(choice-1,0);
            }
            if (direction == new Vector2(1.0f,0))
            {
                choice = Math.Min(choice+1,1);
            }
            cursorUpdate();
    }

    void choiced(InputAction.CallbackContext ctx)
    {
            switch (choice)
            {
                case 0:
                SceneManager.LoadScene("Main");
                break;

                case 1:
                SceneManager.LoadScene("Title");
                break;
            }
    }

    void cursorUpdate()
    {
        Vector2[] positions = new Vector2[2];
        positions[0] = new Vector2(-380, -150);
        positions[1] = new Vector2(380, -150);
        cursor.localPosition = positions[choice];
        cursor.eulerAngles = new Vector3(0,0,180);
    }
    // Update is called once per frame
    void OnDisable()
    {
        Move.performed -= moving;
        Action.started -= choiced;
        InputSystems.Disable();
    }
}
