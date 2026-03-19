using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Prologue_View : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image backTarget;
    [SerializeField] Transform objectParent;
    [SerializeField] TMP_Text textTarget;
    public void ShowBackground(Sprite background)
    {
        backTarget.sprite = background;
    }
    public void ShowText(string text)
    {
        textTarget.text = text;
    }
    public void EnableObject(List<string> obj)
    {
        foreach (var target in obj)
        {
            objectParent.Find(target).gameObject.SetActive(true);
        }
    }

    public void DisableObject(List<string> obj)
    {
        foreach (var target in obj)
        {
            objectParent.Find(target).gameObject.SetActive(false);
        }
    }

    public void Initialize()
    {
        for (int i = 0; i < objectParent.childCount; i++)
        {
            Transform child = objectParent.GetChild(i);
            if (i == 0) { child.gameObject.SetActive(true); } // background
            else { child.gameObject.SetActive(false); } // それ以外のオブジェクト
        }
    }
}
