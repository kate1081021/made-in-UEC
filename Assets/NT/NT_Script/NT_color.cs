using UnityEngine;

namespace NT {

public class NT_color : MiniGameBase
{
    private SpriteRenderer Sprite_renderer;
   [SerializeField] private NT_switch switch_script;
   float clear_level;

    public override void OnGameStart(){
        Sprite_renderer=GetComponent<SpriteRenderer>();
    }

    public override void OnGameEnd(){}
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame

    void Update()
    {if (switch_script==null) return;
     if (switch_script.count<200){
     clear_level=switch_script.count/2;
    }
    else{
     clear_level=(switch_script.count-199.75f)*(switch_script.count-199.75f)+100;
    }
    if (clear_level>255) clear_level=255;
    Sprite_renderer.color=new Color32(212,0,116,(byte)clear_level);}
}}
