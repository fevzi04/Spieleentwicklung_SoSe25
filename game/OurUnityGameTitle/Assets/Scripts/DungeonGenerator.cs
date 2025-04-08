using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrunkWalkDungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int maxSteps = 20;
    public int walkerCount = 3;
    public Vector2Int startPos = Vector2Int.zero;
    public float roomSpacing = 30f;

    [Header("Room Prefabs")]
    public GameObject startingRoomPrefab;
    public GameObject bossRoomPrefab;
    public List<GameObject> standardRoomPrefabs;
    public List<GameObject> treasureRoomPrefabs;
    public List<GameObject> shopRoomPrefabs;

    [Header("Special Room Counts")]
    public int shopRoomCount;
    public int treasureRoomsCount;

    private Dictionary<Vector2Int, RoomType> roomLayout = new Dictionary<Vector2Int, RoomType>();
    private Dictionary<Vector2Int, RoomManager> dungeonRooms = new Dictionary<Vector2Int, RoomManager>();
    private HashSet<(Vector2Int from, Vector2Int to)> roomConnections = new HashSet<(Vector2Int, Vector2Int)>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        roomLayout.Clear();
        roomConnections.Clear();

        List<Vector2Int> walkerPositions = new List<Vector2Int>();
        for (int i = 0; i < walkerCount; i++)
            walkerPositions.Add(startPos);

        int stepsPlaced = 0;
        Dictionary<Vector2Int, int> distanceFromStart = new Dictionary<Vector2Int, int>();
        distanceFromStart[startPos] = 0;

        roomLayout[startPos] = RoomType.StartingRoom;

        while (stepsPlaced < maxSteps)
        {
            for (int i = 0; i < walkerCount && stepsPlaced < maxSteps; i++)
            {
                Vector2Int dir = GetRandomDirection();
                Vector2Int nextPos = walkerPositions[i] + dir;

                if (!roomLayout.ContainsKey(nextPos))
                {
                    roomLayout[nextPos] = RoomType.StandardRoom;
                    distanceFromStart[nextPos] = distanceFromStart[walkerPositions[i]] + 1;
                    stepsPlaced++;
                }

                roomConnections.Add((walkerPositions[i], nextPos));
                roomConnections.Add((nextPos, walkerPositions[i]));

                walkerPositions[i] = nextPos;
            }
        }

        Vector2Int bossPos = distanceFromStart.OrderByDescending(kvp => kvp.Value).First().Key;
        roomLayout[bossPos] = RoomType.BossRoom;

        AssignSpecialRooms(bossPos);
        InstantiateRooms();
        UpdateAllRoomDoors();
    }

    void AssignSpecialRooms(Vector2Int bossPos)
    {
        var candidateRooms = roomLayout
            .Where(pair => pair.Value == RoomType.StandardRoom && pair.Key != bossPos)
            .Select(pair => pair.Key)
            .OrderBy(x => Random.value)
            .ToList();

        int minDistance = 3; 
        List<Vector2Int> placedTreasureRooms = new List<Vector2Int>();
        int index = 0;

        while (placedTreasureRooms.Count < treasureRoomsCount && index < candidateRooms.Count)
        {
            Vector2Int candidate = candidateRooms[index++];
            bool tooClose = placedTreasureRooms.Any(existing =>
                Mathf.Abs(existing.x - candidate.x) + Mathf.Abs(existing.y - candidate.y) < minDistance);

            if (!tooClose)
            {
                roomLayout[candidate] = RoomType.TreasureRoom;
                placedTreasureRooms.Add(candidate);
            }
        }

        for (int i = 0; i < shopRoomCount && index < candidateRooms.Count; i++, index++)
        {
            roomLayout[candidateRooms[index]] = RoomType.ShopRoom;
        }
    }


    void InstantiateRooms()
    {
        dungeonRooms.Clear();

        foreach (var kvp in roomLayout)
        {
            Vector2Int pos = kvp.Key;
            RoomType type = kvp.Value;
            GameObject prefab = GetPrefabForRoomType(type);
            Vector3 worldPos = new Vector3(pos.x * roomSpacing, pos.y * roomSpacing, 0);

            GameObject roomObj = Instantiate(prefab, worldPos, Quaternion.identity);
            RoomManager manager = roomObj.GetComponent<RoomManager>();
            manager.roomType = type;
            dungeonRooms[pos] = manager;
        }
    }

    GameObject GetPrefabForRoomType(RoomType type)
    {
        switch (type)
        {
            case RoomType.StartingRoom:
                return startingRoomPrefab;
            case RoomType.BossRoom:
                return bossRoomPrefab;
            case RoomType.TreasureRoom:
                return treasureRoomPrefabs[Random.Range(0, treasureRoomPrefabs.Count)];
            case RoomType.ShopRoom:
                return shopRoomPrefabs[Random.Range(0, shopRoomPrefabs.Count)];
            case RoomType.StandardRoom:
            default:
                return standardRoomPrefabs[Random.Range(0, standardRoomPrefabs.Count)];
        }
    }

    void UpdateAllRoomDoors()
    {
        foreach (var entry in dungeonRooms)
        {
            Vector2Int pos = entry.Key;
            RoomManager room = entry.Value;

            room.hasTopDoor = false;
            room.hasBottomDoor = false;
            room.hasLeftDoor = false;
            room.hasRightDoor = false;

            if (roomConnections.Contains((pos, pos + Vector2Int.up))) room.hasTopDoor = true;
            if (roomConnections.Contains((pos, pos + Vector2Int.down))) room.hasBottomDoor = true;
            if (roomConnections.Contains((pos, pos + Vector2Int.left))) room.hasLeftDoor = true;
            if (roomConnections.Contains((pos, pos + Vector2Int.right))) room.hasRightDoor = true;

            room.UpdateDoors();
        }
    }

    Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };
        return directions[Random.Range(0, directions.Length)];
    }
}