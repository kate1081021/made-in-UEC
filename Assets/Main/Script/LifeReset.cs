using UnityEngine;

public class LifeReset : MonoBehaviour
{
    public void lifeReset()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(true); // ライフUIのリセット
        }
    }
}
