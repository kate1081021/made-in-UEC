using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class test : MiniGameBase
{
    // Update is called once per frame
    void Update()
    {

        Vector2 speed = moveValue * 2.0f;
        Debug.Log(speed);
    }
    protected override void OnMoveStarted(Vector2 value)
    {
        Debug.Log("こんにちは！");
    }
    public override void OnGameEnd()
    {
        
    }
    public override void OnGameStart()
    {
        
    }
}
