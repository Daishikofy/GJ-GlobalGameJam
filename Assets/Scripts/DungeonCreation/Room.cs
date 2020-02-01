#pragma warning disable 0649
using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
    [HideInInspector]
    public bool isEntry;
    [HideInInspector]
    public bool isExit;
    [HideInInspector]
    public Vector2Int position = Vector2Int.zero;
    [SerializeField]
    RoomType type;

    public RoomType Type
    {
        get { return type; }
    }
}
