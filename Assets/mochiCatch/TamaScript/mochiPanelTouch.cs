using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace catchMochi
{
    public class mochiPanelTouch : MiniGameBase
    {
        public mochiCatch mochiCatch;
        public GameManager gameManager;
        public override void OnGameStart()
        {
            InputSystems.Enable();
            mochiCatch = this.gameObject.GetComponentInParent<mochiCatch>();
            gameManager = Object.FindAnyObjectByType<GameManager>();
        }

        public override void OnGameEnd()
        {
            
        }
        /*
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("clicked");
            mochiCatch.clickMochiAction();
            // gameManager.onClicked();
        }
            public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("finished");
            mochiCatch.endMochiAction();
        }
        */

        void Update()
        {
            // スペースキーが押された瞬間またはエンターキーが押された
            if (Action.WasPerformedThisFrame())
            {
                Debug.Log("clicked");
                mochiCatch.clickMochiAction();
            }
            else if (Action.WasReleasedThisFrame())
            {
                Debug.Log("finished");
                mochiCatch.endMochiAction();
            }
        }
    }
}