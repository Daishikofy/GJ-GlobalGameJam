﻿#pragma warning disable 0649
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [HideInInspector]
    public PlayerController player;

    [HideInInspector]
    public Vector2Int playerStartRoom;
    [HideInInspector]
    public GameObject levelEndRoom;
    int currentLevel = 1;
    [HideInInspector]
    public IntEvent updateLevel;

    public int CurrentLevel {
        get { return currentLevel; }
        set
        {
            currentLevel = value;
            updateLevel.Invoke(currentLevel);
        }
    }

    #region Singleton
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public static GameManager Instance { get { return instance; } }
    #endregion Singleton
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        //TESTE: Remove the following line
        changeLevel();
    }

    public void changeLevel()
    {
        StartCoroutine(newRandomLevel());
    }

    private IEnumerator newRandomLevel()
    {
        CurrentLevel += 1;
        Time.timeScale = 0;
        //Animation panelUp = StartAnimation();
        //yield return WaitForWaitForAnimation(panelUp);

        int roomsNumber = (int)Mathf.Log(Mathf.Pow((float)currentLevel,2f),2f)+5;
        DungeonFactory.Instance.generateDungeon(roomsNumber);
        Vector2 middleRoom = ((Vector2)playerStartRoom + (Vector2)DungeonFactory.Instance.roomMaxDimensions)/2;
        Debug.Log("Middle toom: " + middleRoom);
        player.transform.position = new Vector3(middleRoom.x, middleRoom.y, 0);
        CameraController.Instance.setPosition(new Vector3(middleRoom.x, middleRoom.y, -10));
        //Animation panelDown = StartAnimation();
        //yield return WaitForWaitForAnimation(panelUp);
        yield return null;
        Time.timeScale = 1;
    }

    private IEnumerator WaitForAnimation(Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);
    }

}
