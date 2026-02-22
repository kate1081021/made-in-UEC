using UnityEngine;
using UnityEngine.UI;

namespace NT
{
    public class NT_graph : MiniGameBase
    {
        private float count;
        [SerializeField] private Image image;
        [SerializeField] private NT_switch switchComponent;


        public override void OnGameStart()
        {
            MGManager.Load();
        }
        void Update()
        {
            count = switchComponent.count;
            image.fillAmount = (float)count / 350;
            image.fillAmount = Mathf.Clamp01(image.fillAmount);
        }
        public override void OnGameEnd()
        {
            
        }
    }
}
