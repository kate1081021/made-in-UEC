using UnityEngine;
using UnityEngine.EventSystems;

namespace garbage
{
    public class GB_GarbageMover : MiniGameBase
    {
        private int lane;            
    private string garbageType;  

    public void Init(int lane, string type)
    {
        this.lane = lane;
        this.garbageType = type;
    }

    public int GetLane()
    {
        return lane;
    }

    public string GetType()
    {
        return garbageType;
    }
    private void SavePosition()
    {
        var manager = FindObjectOfType<GB_GameManagingScript>();
        manager.trashPositions[garbageType] = lane;
    }
    public override void OnGameStart()
    {
            
    }

    void Update()
    {
        garbageMovement(Vector2.down);

        if (transform.position.y < -3.3f)
        {
            SavePosition();
            Destroy(this.gameObject);
        }
    }
    private void CheckCorrect()
    {
        var manager = FindObjectOfType<GB_GameManagingScript>();

        int correctPos = manager.positions[garbageType];

        if (lane == correctPos)
        {
            Debug.Log("OK");
        }
        else
        {
            Debug.Log("MISS");
        }
    }
        private void garbageMovement(Vector3 moveDirection)
        {
            var pos = transform.position;

            var moveSpeed = 1;

            pos += moveDirection * moveSpeed * Time.deltaTime;

            transform.position = pos;
        }
    }
}