using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PaintDungeon : MonoBehaviour
{
    Tilemap tilemap;
    [SerializeField]
    Vector2Int position;
    [SerializeField]
    Tilemap[] rooms;
    [SerializeField]
    int roomX, roomY;
    [SerializeField]
    BoundsInt area;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = gameObject.AddComponent<Tilemap>();
        gameObject.AddComponent<TilemapRenderer>();
        copyRoom(position,rooms[0]);
        position.y += roomY;
        copyRoom(position, rooms[1]);
    }

    private void copyRoom(Vector2Int position, Tilemap room)
    {
        TileBase[] tiles = GetRoomTiles(room);

        Vector3Int tilePosition = new Vector3Int(0,0,0);
        Vector2Int increment = new Vector2Int(position.x, position.y);
        foreach (var tile in tiles)
        {        
            if (increment.x >= (roomX + position.x))
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
        Vector3Int tilePosition = new Vector3Int(0,0,0);    
        int count = roomX * roomY;
        TileBase[] tiles = new TileBase[count];

        int index = 0;
        for (int y = 0; y < roomY; y++)
        {
            for (int x = 0; x < roomX; x++)
            {
                tilePosition.x = x;
                tilePosition.y = y;
                tiles[index] = room.GetTile(tilePosition);
                index++;
            }
        }
        return tiles;    
    }
}
