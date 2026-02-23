using UnityEngine;
using TMPro;
using System.Diagnostics;

namespace NT
{
public class NT_switch : MiniGameBase
{
    public TMP_Text countText;
    public float count = 0;
<<<<<<< HEAD
    AudioSource audioSource;
    [SerializeField] private float sounddistance;
    [SerializeField] private float soundborder;
    int i = 0;

=======
    public Stopwatch sw = new Stopwatch();
>>>>>>> 10fb74610cf612ed4f4aaa19b2e99561160933db

    public override void OnGameStart(){
    MGManager.Load();
    count = 0;
<<<<<<< HEAD
    audioSource = GetComponent<AudioSource>();
=======
    sw.Start();
>>>>>>> 10fb74610cf612ed4f4aaa19b2e99561160933db
    }

 public override void OnGameEnd(){}

    void Update()
    {
        if (Action.IsPressed())
        {
            count=count+Time.timeScale;
            if (countText != null){
            countText.text = "" + count;
<<<<<<< HEAD
            }
        }
        if (audioSource != null && count >= soundborder + i*sounddistance)
        {
            audioSource.Play();
            i++;
=======
            sw.Restart();
>>>>>>> 10fb74610cf612ed4f4aaa19b2e99561160933db
        }
    }
}
}