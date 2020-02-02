#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    int currentPos;
    public int position;

    Camera camera;
    bool isMoving;

    [SerializeField]
    int transitionSpeed;

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
        if (isMoving) return;
        Vector3 playerRelativePos = camera.WorldToViewportPoint(GameManager.Instance.player.transform.position);
        if (playerRelativePos.x > 1)
            StartCoroutine (moveHorizontal(1));
        else if (playerRelativePos.x < 0)
            StartCoroutine (moveHorizontal(-1));
        else if (playerRelativePos.y > 1.1)
            StartCoroutine (moveVertical(1));
        else if (playerRelativePos.y < -0.1)
            StartCoroutine (moveVertical(-1));
    }

    public void setPosition(Vector3 position)
    {
        transform.position = position;       
    }

    public IEnumerator moveHorizontal(int value)
    {
        isMoving = true;
        var destination = transform.position + Vector3.right * value * DungeonFactory.Instance.roomMaxDimensions.x;
        float sqrRemainingDistance = (transform.position - destination).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, transitionSpeed * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - destination).sqrMagnitude;

            yield return null;
        }
        isMoving = false;
    }
    public IEnumerator moveVertical(int value)
    {
        isMoving = true;
        var destination = transform.position + Vector3.up * value * DungeonFactory.Instance.roomMaxDimensions.y;
        float sqrRemainingDistance = (transform.position - destination).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, transitionSpeed * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - destination).sqrMagnitude;

            yield return null;
        }
        isMoving = false;
    }
}
