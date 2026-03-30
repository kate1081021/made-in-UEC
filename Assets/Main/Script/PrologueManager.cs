using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PrologueManager : MiniGameBase
{
    private int index = 0;
    [SerializeField] Prologue_Model model;
    [SerializeField] Prologue_View view;
    public override void OnGameStart()
    {
        BGMPlay();
        InputSystems = new MIU_InputSystem();
        InputSystems.Enable();
        Trigger_left = InputSystems.FindAction("Trigger_left");  // L
        Action = InputSystems.FindAction("Action");  // Space
        Trigger_left.started += skip;
        Action.started += choiced;
        index = 0;

        view.Initialize();

        view.EnableObject(model.context[index].enables);
        view.ShowText(model.context[index].text);
        view.ShowBackground(model.back.Find(e => e.key == model.context[index].background).sprite);
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

        view.DisableObject(model.context[index].disables);
        view.ShowText(model.context[index].text);
        view.EnableObject(model.context[index].enables);


        if (model.context[index].background != "")
        {
            view.ShowBackground(model.back.Find(e => e.key == model.context[index].background).sprite);
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
