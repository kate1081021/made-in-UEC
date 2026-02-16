using UnityEngine;
using TMPro;

namespace NT
{
public class NT_switch : MiniGameBase
{
    public TMP_Text countText;
    public int count = 0;


 public override void OnGameStart(){
    MGManager.Load();
 }

 public override void OnGameEnd(){}

    void Update()
    {
        if (Action.IsPressed())
        {
            count++;
            countText.text = "" + count;
        }
    }
}
}