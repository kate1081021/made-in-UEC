using UnityEngine;
using UnityEngine.EventSystems;

namespace WI
{
    public class WI_I_hoverDestroy : MiniGameBase, IPointerEnterHandler, IPointerExitHandler
    {
        private bool isHovering = false;

        public override void OnGameStart()
        {
            this.gameObject.SetActive(true);
        }

        public override void OnGameEnd() { }

        void Update() { }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
        }

        public void setInputClose()
        {
            Destroy(this.gameObject);
        }
    }
}