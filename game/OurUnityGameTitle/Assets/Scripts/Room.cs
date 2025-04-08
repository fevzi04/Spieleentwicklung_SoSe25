using UnityEngine;

public enum RoomType
{
    StartingRoom,
    StandardRoom,
    ShopRoom,
    TreasureRoom,
    BossRoom
}

public class RoomManager : MonoBehaviour
{
    [Header("Room Type")]
    public RoomType roomType;

    [Header("Door Settings")]
    public bool hasTopDoor;
    public bool hasBottomDoor;
    public bool hasLeftDoor;
    public bool hasRightDoor;

    [Header("Door References")]
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    public void UpdateDoors()
    {
        if (topDoor != null) topDoor.SetActive(hasTopDoor);
        if (bottomDoor != null) bottomDoor.SetActive(hasBottomDoor);
        if (leftDoor != null) leftDoor.SetActive(hasLeftDoor);
        if (rightDoor != null) rightDoor.SetActive(hasRightDoor);
    }
}