using UnityEngine;
using UnityEngine.UI; // UIを操作するために追加

namespace plugmatch
{
    public class PM_Outlet : MonoBehaviour
    {
        [Header("このコンセント穴の形")]
        public PM_ShapeType outletShape;

        [Header("コンセント穴の画像(Image)")]
        public Image outletImage;
    }
}