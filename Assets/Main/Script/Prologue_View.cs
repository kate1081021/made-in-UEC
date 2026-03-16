using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Prologue_View : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image backTarget;
    [SerializeField] TMP_Text textTarget;
    public void ShowBackground(Sprite background)
    {
        backTarget.sprite = background;
    }
    public void ShowText(string text)
    {
        textTarget.text = text;
    }
}
