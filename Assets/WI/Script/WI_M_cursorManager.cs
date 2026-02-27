using UnityEngine;

namespace WI
{

    public class WI_M_cursorManager : MiniGameBase
    {
        [SerializeField] private float speed = 0.1f;
        [SerializeField] private Vector2 cursorPosition;

        private float cursorSpeed = 0.1f;
        private bool slow = false;
        private Vector2 inputDirection;
        private Vector3 viewPosition;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnGameStart()
        {
            inputDirection = new Vector2(0f, 0f);
            cursorPosition = transform.position;
            viewPosition = Camera.main.WorldToViewportPoint(cursorPosition);
        }

        public override void OnGameEnd() { }

        // Update is called once per frame
        void FixedUpdate()
        {
            inputDirection = Move.ReadValue<Vector2>();

            if (slow)
            {
                cursorSpeed = 0.05f;
            }
            else
            {
                cursorSpeed = this.speed * MGManager.timeScale;
            }
            // Debug.Log(cursorSpeed);

            cursorPosition = new Vector2(transform.position.x + inputDirection.x * cursorSpeed,
                                         transform.position.y + inputDirection.y * cursorSpeed);

            viewPosition = Camera.main.WorldToViewportPoint(cursorPosition);
            
            viewPosition.x = Mathf.Clamp(viewPosition.x, 0.02f, 1.02f);
            viewPosition.y = Mathf.Clamp(viewPosition.y, -0.035f, 0.965f);
            
            cursorPosition = Camera.main.ViewportToWorldPoint(viewPosition);
            transform.position = new Vector2(cursorPosition.x, cursorPosition.y);
            
        }

        // L/R“ü—ÍŽž0.5f‚ÉŒ¸‘¬
        protected override void OnTriggerPerformed(float value)
        {
            slow = true;
        }
        protected override void OnTriggerCanceled(float value)
        {
            slow = false;
        }

        public void setSpeed(float s)
        {
            this.speed = s;
        }
    }
}

