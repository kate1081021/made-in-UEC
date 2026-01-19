using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace catchMochi
{
    public class mochiPanelTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public mochiCatch mochiCatch;
        public GameManager gameManager;
        void Start()
        {
            mochiCatch = this.gameObject.GetComponentInParent<mochiCatch>();
            gameManager = Object.FindAnyObjectByType<GameManager>();
        }
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

        void Update()
        {
            // スペースキーが押された瞬間またはエンターキーが押された
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("clicked");
                mochiCatch.clickMochiAction();
            }
            else if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return))
            {
                Debug.Log("finished");
                mochiCatch.endMochiAction();
            }
        }
    }
}