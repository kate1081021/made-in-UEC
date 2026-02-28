using TMPro;
using UnityEngine;

public class InputFieldScript : MonoBehaviour
{
    public TMP_InputField input;
    public GameObject TestPlayInput;
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
        input = GetComponent<TMP_InputField>();
        int result;
        int.TryParse(input.text,out result);
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
    }
}
