using UnityEngine;
using TMPro;

public class NT_switch : MonoBehaviour
{
    public TMP_Text countText;
    private int count = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            count++;
            countText.text = "" + count;
        }
    }
}
