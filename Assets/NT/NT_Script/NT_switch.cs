using UnityEngine;
using TMPro;

namespace NT
{
public class NT_switch : MiniGameBase
{
    public TMP_Text countText;
    public float count = 0;


    public override void OnGameStart(){
        MGManager.TestPlay(100);
    MGManager.Load();
    count = 0;
    }

 public override void OnGameEnd(){}

    void Update()
    {
        if (Action.IsPressed())
        {
            count=count+Time.timeScale;
            countText.text = "" + count;
        }
    }
}
}