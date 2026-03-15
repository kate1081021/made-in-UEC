using System;
using TMPro;
using UnityEngine;

public class InputFieldScript : MonoBehaviour
{
    public GameObject TestPlayInput;
    private TMP_InputField input;
    public GameObject Stage;
    private TMP_InputField input2;
    public GameObject SceneName;
    int stage;
    void Update()
    {
        if (!MGManager.isDebugMode) // 公開時は即消去
        {
            Destroy(TestPlayInput);
        }
    }
    public void endInput()
    {
        input = Stage.GetComponent<TMP_InputField>();
        input2 = SceneName.GetComponent<TMP_InputField>();
        int result;
        int.TryParse(input.text,out result);
        
        // stage数のにゅりょく
        if (result == 0)
        {
            Debug.LogError("入力値を整数に変換できませんでした。入力できるのは1~100までの整数です。");
        }
        else
        {
            stage = int.Parse(input.text);
            if ( stage < 1 || stage > 99 )
            {
                Debug.LogError("範囲外の値が入力されました。入力できるのは1~99までの整数です。");
            }
            else
            {
                stage = int.Parse(input.text);
                MGManager.TestPlay(stage);
            }
        }

        // シーン名
        string scene = input2.text;
        MGManager.stuckScene(scene);  // シーン名をストック

    }
}
