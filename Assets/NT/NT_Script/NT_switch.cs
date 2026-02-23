using UnityEngine;
using TMPro;
using System.Diagnostics;

namespace NT
{
public class NT_switch : MiniGameBase
{
    public TMP_Text countText;
    public float count = 0;
    public Stopwatch sw = new Stopwatch();

    public override void OnGameStart(){
        MGManager.TestPlay(100);
    MGManager.Load();
    count = 0;
    sw.Start();
    }

 public override void OnGameEnd(){}

    void Update()
    {
        if (Action.IsPressed())
        {
            count=count+Time.timeScale;
            countText.text = "" + count;
            sw.Restart();
        }
    }
}
}