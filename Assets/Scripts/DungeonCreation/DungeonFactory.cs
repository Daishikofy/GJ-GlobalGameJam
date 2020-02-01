
#pragma warning disable 0649
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonFactory : MonoBehaviour
{
    [SerializeField]
    Vector2Int roomMaxDimensions;
    [SerializeField]
    int roomsNumber;
    List<Vector2Int> availableSpaces;
    Vector2Int[] roomsSpaces;
    GameObject[] rooms;

    [Space]
    [SerializeField]
    Tilemap upWallMap;
    [SerializeField]
    Tilemap downWallMap;
    [SerializeField]
    Tilemap floorMap;

    public void generateDungeon(int roomsNumber)
    {
        selectRooms(roomsNumber);
        generateRooms();
        instanciateRooms();
    }

    private void selectRooms(int roomsNumber)
    {
        roomsSpaces = new Vector2Int[roomsNumber];
        availableSpaces = new List<Vector2Int>();
        availableSpaces.Add(Vector2Int.zero);

        for (int currentRoom = 0; currentRoom < roomsNumber; currentRoom++)
        {
            int random = Random.Range(0, availableSpaces.Count);
            Vector2Int roomSpace = availableSpaces[random];
            availableSpaces.RemoveAt(random);
            roomsSpaces[currentRoom] = roomSpace;

            Vector2Int[] newSpaces = FindNeighbours(roomSpace);

            foreach (var newSpace in newSpaces)
            {
                if (!roomsSpaces.Contains<Vector2Int>(newSpace) && !availableSpaces.Contains(newSpace))
                    availableSpaces.Add(newSpace);
            }           
        }
    }

    private Vector2Int[] FindNeighbours(Vector2Int position)
    {
        var up = position + (Vector2Int.up * roomMaxDimensions.y);
        var down = position + (Vector2Int.down * roomMaxDimensions.y);
        var left = position + (Vector2Int.left * roomMaxDimensions.x);
        var right = position + (Vector2Int.right * roomMaxDimensions.x);

        return new Vector2Int[] { up, down, left, right };
    }

    private void generateRooms()
    {
        rooms = new GameObject[roomsNumber];
        int i = 0;
        foreach (var roomSpace in roomsSpaces)
        {
            RoomType type = selectRoomType(roomSpace);
            GameObject room = selectRoom(type);
            rooms[i] = room;
            i++;

            if (room.GetComponent<Room>().Type != type)
                Debug.LogError("LIRE CETTE ERREUR : The room " + room.name + " in folder resources/prefabRooms/" + type + " is not of the expected type.");
        }
    }

    private void instanciateRooms()
    {
        Vector3 conversion = new Vector3();
        for (int i = 0; i < rooms.Length; i++)
        {
            conversion.x = roomsSpaces[i].x;
            conversion.y = roomsSpaces[i].y;
            //TODO : Check this transform
            GameObject room = Instantiate(rooms[i], conversion, Quaternion.identity, this.transform);
            if (i == 0)
            {
                room.GetComponent<Room>().isEntry = true;
                //TODO : Gamemanager set entry room position to roomsSpaces[i]
            }
            else if (i == rooms.Length - 1)
            {
                room.GetComponent<Room>().isExit = true;
            }

            Tilemap[] tilemaps = room.GetComponentsInChildren<Tilemap>();
            foreach (var tilemap in tilemaps)
            {
                Tilemap currentTilemap = selectCurrentMap(tilemap.name, room.name);
                copyRoom(roomsSpaces[i], tilemap, currentTilemap);
                Destroy(tilemap.gameObject);
            }
        }

    }

    private void copyRoom(Vector2Int position, Tilemap roomToCopy, Tilemap tilemap)
    {
        TileBase[] tiles = GetRoomTiles(roomToCopy);

        Vector3Int tilePosition = new Vector3Int(0, 0, 0);
        Vector2Int increment = new Vector2Int(position.x, position.y);
        foreach (var tile in tiles)
        {
            if (increment.x >= (roomMaxDimensions.x + position.x))
            {
                increment.y++;
                increment.x = position.x;
            }
            tilePosition.x = increment.x;
            tilePosition.y = increment.y;

            tilemap.SetTile(tilePosition, tile);
            increment.x++;
        }
        tilemap.RefreshAllTiles();
    }

    private TileBase[] GetRoomTiles(Tilemap room)
    {
        Vector3Int tilePosition = new Vector3Int(0, 0, 0);
        int count = roomMaxDimensions.x * roomMaxDimensions.y;
        TileBase[] tiles = new TileBase[count];

        int index = 0;
        for (int y = 0; y < roomMaxDimensions.y; y++)
        {
            for (int x = 0; x < roomMaxDimensions.x; x++)
            {
                tilePosition.x = x;
                tilePosition.y = y;
                tiles[index] = room.GetTile(tilePosition);
                index++;
            }
        }
        return tiles;
    }

    private GameObject selectRoom(RoomType type)
    {
        string path = "RoomsPrefab/" + type.ToString();
        GameObject[] rooms = Resources.LoadAll<GameObject>(path);
        int rand = Random.Range(0, rooms.Length);
        return rooms[rand];
    }

    private Tilemap selectCurrentMap (string name, string roomName)
    {
        if (name == "Up_Wall")
            return upWallMap;
        else if (name == "Down_Wall")
            return downWallMap;
        else if (name == "Floor")
            return floorMap;
        else
            Debug.LogError("LIRE CETTE ERREUR : In room >" + roomName + "<, the map " + name + " has an invalid name.");
        return null;
    }

    private RoomType selectRoomType(Vector2Int position)
    {
        int doors = 0;
        bool up = roomsSpaces.Contains<Vector2Int>((position + (Vector2Int.up * roomMaxDimensions.y)));
        bool bottom = roomsSpaces.Contains<Vector2Int>(position + (Vector2Int.down * roomMaxDimensions.y));
        bool left = roomsSpaces.Contains<Vector2Int>(position + (Vector2Int.left * roomMaxDimensions.x));
        bool right = roomsSpaces.Contains<Vector2Int>(position + (Vector2Int.right * roomMaxDimensions.x));

        if (up) doors += 1;
        if (bottom) doors += 1;
        if (left) doors += 1;
        if (right) doors += 1;

        if (doors == 4)
            return RoomType.ALL;
        else if (doors == 3)
        {
            if (!up)
                return RoomType.LRB;
            else if (!left)
                return RoomType.RUB;
            else if (!right)
                return RoomType.LUB;
            else if (!bottom)
                return RoomType.LRU;
        }
        else if (doors == 2)
        {
            if (up)
            {
                if (left)
                    return RoomType.LU;
                if (right)
                    return RoomType.RU;
                if (bottom)
                    return RoomType.UB;
            }
            else if (left)
            {
                if (right)
                    return RoomType.LR;
                if (bottom)
                    return RoomType.LB;
            }
            else
                return RoomType.RB;
        }
        
        if (up) return RoomType.U;
        if (bottom) return RoomType.B;
        if (left) return RoomType.L;
        return RoomType.R;
    }

    private void OnDrawGizmos()
    {
        

        Gizmos.color = Color.red;
        var position = new Vector3(0, 0, 0);

        Vector3 dLeft = position;
        Vector3 dRight = (position + (Vector3.right * roomMaxDimensions.x));
        Vector3 uLeft = (position + (Vector3.up * roomMaxDimensions.y));
        Vector3 uRight = ((position + (Vector3.up * roomMaxDimensions.y)) + (Vector3.right * roomMaxDimensions.x));

        Gizmos.DrawLine(dLeft, dRight);
        Gizmos.DrawLine(dLeft, uLeft);
        Gizmos.DrawLine(uLeft, uRight);
        Gizmos.DrawLine(dRight, uRight);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(dLeft, 0.15f);
        Gizmos.DrawSphere(dRight, 0.15f);
        Gizmos.DrawSphere(uLeft, 0.15f);
        Gizmos.DrawSphere(uRight, 0.15f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere((dLeft + dRight)/2, 0.1f);
        Gizmos.DrawSphere((dLeft + uLeft) / 2, 0.1f);
        Gizmos.DrawSphere((dRight + uRight) / 2, 0.1f);
        Gizmos.DrawSphere((uRight + uLeft) / 2, 0.1f);

        foreach (var room in roomsSpaces)
        {
            Gizmos.color = Color.blue;
            position = new Vector3(room.x, room.y, 0);
            Gizmos.DrawLine(position, (position + (Vector3.right * roomMaxDimensions.x)));
            Gizmos.DrawLine(position, (position + (Vector3.up * roomMaxDimensions.y)));
            Gizmos.DrawLine((position + (Vector3.up * roomMaxDimensions.y)), ((position + (Vector3.up * roomMaxDimensions.y)) + (Vector3.right * roomMaxDimensions.x)));
            Gizmos.DrawLine((position + (Vector3.right * roomMaxDimensions.x)), ((position + (Vector3.up * roomMaxDimensions.y)) + (Vector3.right * roomMaxDimensions.x)));
        }
        
    }
} 
