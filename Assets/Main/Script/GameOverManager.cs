using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

public class GameOverManager : MiniGameBase
{
    private int choice = 0;
    private bool isPlayingGameOver1 = false;
    [SerializeField] Transform cursor;
    [SerializeField] Animator anim;

    public override void OnGameStart()
    {
        isPlayingGameOver1 = false;
        InputSystems = new MIU_InputSystem();
        InputSystems.Enable();
        Move = InputSystems.FindAction("Move");  // WASD
        Action = InputSystems.FindAction("Action");  // Space
        Move.performed += moving;
        Action.started += choiced;
        cursorUpdate();
        StartCoroutine(StartMusic());
    }
    IEnumerator StartMusic()
    {
        SEPlay("GameOver1");
        anim.SetTrigger("GameOver");
        yield return new WaitForSeconds(3.0f);
        BGMPlay();
        isPlayingGameOver1 = true;
    }
    void moving(InputAction.CallbackContext ctx)
    {
        if (!isPlayingGameOver1) { return; }
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
        if (!isPlayingGameOver1) { return; }
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
