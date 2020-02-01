using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    int currentPos;
    public int position;

    Camera camera;

    #region Singleton
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public static CameraController Instance { get { return instance; } }
    #endregion Singleton

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        currentPos = position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerRelativePos = camera.WorldToViewportPoint(GameManager.Instance.player.transform.position);
        if (playerRelativePos.x > 1)
            moveHorizontal(1);
        else if (playerRelativePos.x < 0)
            moveHorizontal(-1);
        else if (playerRelativePos.y > 1)
            moveVertical(1);
        else if (playerRelativePos.y < 0)
            moveVertical(-1);
    }

    public void setPosition(Vector3 position)
    {
        transform.position = position;       
    }

    public void moveHorizontal(int value)
    {
        transform.position += Vector3.right * value * DungeonFactory.Instance.roomMaxDimensions.x;
    }
    public void moveVertical(int value)
    {
        transform.position += Vector3.up * value * DungeonFactory.Instance.roomMaxDimensions.y;
    }
}
