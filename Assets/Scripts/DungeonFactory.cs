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

    private void Start()
    {
        generateDungeon(/*roomNumberE*/);
    }
    public void generateDungeon(/*int roomsNumber*/)
    {
        selectRooms(/*roomsNumber*/);
        generateRooms();
        instanciateRooms();
    }

    private void selectRooms(/*int roomsNumber*/)
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
        foreach (var room in roomsSpaces)
        {
            RoomType type = selectRoomType(room);
            //GameObject room = selectRoom(type) **Select room
            //Add room to an array of rooms
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
                //room.GetComponent<Room>().isEntry = true;
                //Gamemanager set entry room position to roomsSpaces[i]
            }
            else if (i == rooms.Length - 1)
            {
                //room.GetComponent<Room>().isExit = true;
            }

            Tilemap[] tilemaps = room.GetComponentsInChildren<Tilemap>();
            //use paintRoom to copy the tileMap and then delete
        }
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
        if (roomsSpaces == null) return;
        foreach (var room in roomsSpaces)
        {
            Gizmos.color = Color.red;
            var position = new Vector3(room.x, room.y, 0);
            Gizmos.DrawLine(position, (position + (Vector3.right * roomMaxDimensions.x)));
            Gizmos.DrawLine(position, (position + (Vector3.up * roomMaxDimensions.y)));
            Gizmos.DrawLine((position + (Vector3.up * roomMaxDimensions.y)), ((position + (Vector3.up * roomMaxDimensions.y)) + (Vector3.right * roomMaxDimensions.x)));
            Gizmos.DrawLine((position + (Vector3.right * roomMaxDimensions.x)), ((position + (Vector3.up * roomMaxDimensions.y)) + (Vector3.right * roomMaxDimensions.x)));
        }
    }
} 
