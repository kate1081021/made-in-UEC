using UnityEngine;
using UnityEngine.UI;

namespace SK
{
    public class SK_NoteMover : MonoBehaviour 
    {
        public float baseSpeed = 800f;
        public SK_WWG.ButtonType noteType; // Left or Right

        [Header("Visual Feedback")]
        public Color pressedColor = new Color(1f, 0.9f, 0.4f, 1f); // 押された時の色(薄い黄色など)

        private SK_WWG gameManager;
        private Image myImage;
        private bool isPressed = false;

        public void Setup(SK_WWG manager, SK_WWG.ButtonType type, Sprite sprite)
        {
            gameManager = manager;
            noteType = type;
            isPressed = false;

            myImage = GetComponent<Image>();
            if (myImage != null && sprite != null)
            {
                myImage.sprite = sprite;
                myImage.color = Color.white; // 初期化
            }
        }

        void Update()
        {
            if (gameManager == null) return;

            float currentSpeed = baseSpeed * gameManager.GetSpeedMultiplier();
            transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

            if (transform.localPosition.x < -1500)
            {
                Destroy(gameObject);
            }
        }

        // 入力成功時に呼ばれる
        public void OnInputReceived()
        {
            if (isPressed) return;

            if (myImage != null)
            {
                myImage.color = pressedColor;
            }
            isPressed = true;
        }
    }
}