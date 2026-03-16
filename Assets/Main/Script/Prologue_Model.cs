using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Prologue_Model", menuName = "Scriptable Objects/Prologue_Model")]
public class Prologue_Model : ScriptableObject
{
    public List<DialogueContext> context;
    public List<BackgroundKey> back;
}

[Serializable]
public class DialogueContext
{
    public string background;
    public List<string> enables;
    public List<string> disables;
    [TextArea(2,5)]
    public string text;
}
[Serializable]
public class BackgroundKey
{
    public string key;
    public Sprite sprite;
}