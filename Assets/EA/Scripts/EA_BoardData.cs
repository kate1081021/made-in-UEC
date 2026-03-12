using UnityEngine;

[CreateAssetMenu(fileName = "EA_BoardData", menuName = "EA_BoardData")]
public class EA_BoardData : ScriptableObject
{
    // 正解となる盤面のデータ
    public bool[] data;

}
