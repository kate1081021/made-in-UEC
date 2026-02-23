using UnityEngine;
using System.Collections.Generic;

namespace BK
{
    [CreateAssetMenu(fileName = "BK_LocationData", menuName = "BK_LocationData")]
    public class BK_LocationData : ScriptableObject
    {
        public List<int> data;
    }
}
