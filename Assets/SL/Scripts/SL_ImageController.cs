using UnityEngine;
using UnityEngine.UI;

namespace SL
{
    public class SL_ImageController : MiniGameBase
    {
        [Header("画像")]
        public SpriteRenderer sleepDogImage;
        public Sprite wakeDogImage;
        public SpriteRenderer backGround_Space;
        public Sprite backGround_Real;
        public override void OnGameStart()
        {
            Debug.Log("ImageManager起動");
        }
        public void changeDogImage()
        {
            if(sleepDogImage != null && wakeDogImage != null)
            {
                sleepDogImage.sprite = wakeDogImage;
                Debug.Log("起きたね");
            }
            else
            {
                Debug.LogError("犬のどっちかの画像が登録されてないかも");
            }
        }

        public void changeBackGround()
        {
            if(backGround_Space != null && backGround_Real != null)
            {
                backGround_Space.sprite = backGround_Real;
                Debug.Log("背景変化");
            }
            else
            {
                Debug.LogError("どっちかの背景が登録されてないかも");
            }
        }
    }
}
