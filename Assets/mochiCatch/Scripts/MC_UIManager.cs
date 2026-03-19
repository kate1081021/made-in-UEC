using UnityEngine;

public class MC_UIManager : MonoBehaviour
{
    // オブジェクトを有効にする
    public void GetEnabled(bool value)
    {
        gameObject.SetActive(value);
    }
}
