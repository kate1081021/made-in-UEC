using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine.UIElements;

public class PrologueManager : MonoBehaviour
{
    private int choice = 0;
    private string mode = "Title";
    private MIU_InputSystem InputSystems;
    private InputAction Trigger_left;
    private InputAction Action;
    [SerializeField] GameObject textbox;
    [SerializeField] TMP_Text text;
    List<string> context = new List<string>();
    private int index = 0;
    
    
    void Start()
    {
        InputSystems = new MIU_InputSystem();
        InputSystems.Enable();
        Trigger_left = InputSystems.FindAction("Trigger_left");  // L
        Action = InputSystems.FindAction("Action");  // Space
        Trigger_left.started += skip;
        Action.started += choiced;
        context.Add("Actionで進み、Trigger_leftでスキップします");
        context.Add("プロローグのテストです");
        context.Add("この文章を進めるとMainが始まります");
        index = 0;
        text.text = context[index];
    }
    void skip(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("Main");
    }

    void choiced(InputAction.CallbackContext ctx)
    {
        index++;
        if (index > context.Count - 1)
        {
            SceneManager.LoadScene("Main");
        }
        text.text = context[index];
    }

    // Update is called once per frame
    void OnDisable()
    {
        Trigger_left.started -= skip;
        Action.started -= choiced;
        InputSystems.Disable();
    }
}
