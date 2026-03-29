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

public class TitleManager : MiniGameBase
{
    public static bool isNormalMode = false;
    private int choice = 0;
    private string mode = "Title";
    private bool isSetting = false;
    [SerializeField] List<Slider> nowSetting;
    private int OPchoice = 1;
    [SerializeField] GameObject Credit;
    [SerializeField] GameObject Option;
    [SerializeField] Transform cursor;
    [SerializeField] Transform OPcursor;
    [SerializeField] Slider slider;

    public override void OnGameStart()
    {
        InputSystems = new MIU_InputSystem();
        InputSystems.Enable();
        Move = InputSystems.FindAction("Move");  // WASD
        Action = InputSystems.FindAction("Action");  // Space
        Move.started += moving;
        Action.started += choiced;
        Credit.SetActive(false);
        Option.SetActive(false);
        cursorUpdate();
    }
    
    void moving(InputAction.CallbackContext ctx)
    {
        Vector2 direction = ctx.ReadValue<Vector2>();
        convert_stick_to_dir(direction);
        switch (mode) {
            case "Title":
            if (direction == new Vector2(-1.0f,0))
            {
                choice = Math.Max(choice-1,0);
            }
            if (direction == new Vector2(1.0f,0))
            {
                choice = Math.Min(choice+1,3);
            }
            cursorUpdate();
            Debug.Log(choice);
            break;

            case "Option":
                if (!isSetting) {
                    if (direction == new Vector2(0,1.0f))
                    {
                        OPchoice = Math.Max(OPchoice-1,0);
                    }
                    if (direction == new Vector2(0,-1.0f))
                    {
                        OPchoice = Math.Min(OPchoice+1,1);
                    }
                }
                else
                {
                    float addValue = direction.x / 5;
                    Setting(nowSetting[OPchoice],addValue);
                }
            OPcursorUpdate();
            Debug.Log(OPchoice);
            break;

            case "Credit":
            break;
        }
    }

    void Setting(Slider target,float add)
    {
        target.value += add;
    }
    void choiced(InputAction.CallbackContext ctx)
    {
        if (mode == "Credit") { mode = "Title"; Credit.SetActive(false); }
        else if (mode == "Option")
        {
            if (isSetting) {
                Image img = OPcursor.GetComponent<Image>();
                img.color = new Color(1.0f,1.0f,1.0f,1.0f);
                isSetting = false; }
            else
            {
                switch(OPchoice) {
                    case 0:
                    Image img = OPcursor.GetComponent<Image>();
                    img.color = new Color(0.5f,0.5f,0.5f,1.0f);
                    isSetting = true;
                    break;

                    case 1:
                    mode = "Title";
                    Option.SetActive(false);
                    break;
                }
            }
        }
        else
        {
            switch (choice)
            {
                case 0:
                SceneManager.LoadScene("EndCredits");
                /// Credit.SetActive(true);
                break;

                case 1:
                SceneManager.LoadScene("Prologue");
                isNormalMode = true;
                break;

                case 2:
                SceneManager.LoadScene("Main");
                isNormalMode = false;
                break;

                case 3:
                mode = "Option";
                Option.SetActive(true);
                break;
            }
        }
    }

    void cursorUpdate()
    {
        Vector2[] positions = new Vector2[4];
        positions[0] = new Vector2(-705, 250);
        positions[1] = new Vector2(-380, -150);
        positions[2] = new Vector2(380, -150);
        positions[3] = new Vector2(775, 250);
        cursor.localPosition = positions[choice];
        if (choice == 1 || choice == 2) { cursor.eulerAngles = new Vector3(0,0,180); }
        else { cursor.eulerAngles = new Vector3(0,0,0); }
    }
    void OPcursorUpdate()
    {
        Vector2[] positions = new Vector2[2];
        positions[0] = new Vector2(-340, 0);
        positions[1] = new Vector2(-340, -360);
        OPcursor.localPosition = positions[OPchoice];
    }

    // 音量が変更されたとき
    public void OnVolumeUpdate()
    {
        MGManager.sound_volume = slider.value;  // 音量変更
        ApplyVolume(slider.value);  // 変更を適応
    }

    // Update is called once per frame
    void OnDisable()
    {
        Move.started -= moving;
        Action.started -= choiced;
        InputSystems.Disable();
    }
}
