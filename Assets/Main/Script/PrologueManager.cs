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
    private MIU_InputSystem InputSystems;
    private InputAction Trigger_left;
    private InputAction Action;
    private int index = 0;
    [SerializeField] Prologue_Model model;
    [SerializeField] Prologue_View view;
    void Start()
    {
        InputSystems = new MIU_InputSystem();
        InputSystems.Enable();
        Trigger_left = InputSystems.FindAction("Trigger_left");  // L
        Action = InputSystems.FindAction("Action");  // Space
        Trigger_left.started += skip;
        Action.started += choiced;
        index = 0;

        view.ShowText(model.context[index].text);
        view.ShowBackground(model.back.Find(e => e.key == model.context[index].background).images);
    }
    void skip(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("Main");
    }

    void choiced(InputAction.CallbackContext ctx)
    {
        index++;
        if (index > model.context.Count - 1)
        {
            SceneManager.LoadScene("Main");
        }

        view.ShowText(model.context[index].text);

        if (model.context[index].background != "")
        {
            view.ShowBackground(model.back.Find(e => e.key == model.context[index].background).images);
        }
    }

    // Update is called once per frame
    void OnDisable()
    {
        Trigger_left.started -= skip;
        Action.started -= choiced;
        InputSystems.Disable();
    }
}
