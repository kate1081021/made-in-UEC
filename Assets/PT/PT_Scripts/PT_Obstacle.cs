using UnityEngine;
using UnityEngine.Rendering;

namespace PTgame
{
    public class PT_Obstacle : MonoBehaviour
    {
        [SerializeField] public float power; //プラスで右に、マイナスで左に影響を与える
    }
}