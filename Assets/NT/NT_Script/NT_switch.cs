using UnityEngine;
using TMPro;
using System.Diagnostics;

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
    public bool isCleared = false; // クリアフラグ

    public Stopwatch sw = new Stopwatch();

    public override void OnGameStart(){
    BGMPlay(false);
    MGManager.Load();
    count = 0;
    //audioSource = GetComponent<AudioSource>();
    sw.Start();
    }

 public override void OnGameEnd(){}

    void Update()
    {
        if (Action.IsPressed() && !isCleared)
        {
            count=count+Time.timeScale;
            if (countText != null){
            countText.text = "" + count;
            }
            sw.Restart();
        }
        if (count >= soundborder + i*sounddistance)
        {
            SEPlay("NT_DropWater");
            //audioSource.Play();
            i++;
        }
    }
}
}