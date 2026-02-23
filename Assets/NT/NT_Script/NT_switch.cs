using UnityEngine;
using TMPro;

namespace NT
{
public class NT_switch : MiniGameBase
{
    public TMP_Text countText;
    public float count = 0;
    AudioSource audioSource;
    [SerializeField] private float sounddistance;
    [SerializeField] private float soundborder;
    int i = 0;


    public override void OnGameStart(){
    MGManager.Load();
    count = 0;
    audioSource = GetComponent<AudioSource>();
    }

 public override void OnGameEnd(){}

    void Update()
    {
        if (Action.IsPressed())
        {
            count=count+Time.timeScale;
            if (countText != null){
            countText.text = "" + count;
            }
        }
        if (audioSource != null && count >= soundborder + i*sounddistance)
        {
            audioSource.Play();
            i++;
        }
    }
}
}