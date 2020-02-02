#pragma warning disable 0649
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
    int currentLevel = 0;
    [HideInInspector]
    public IntEvent updateLevel;

    [SerializeField]
    AudioClip attackMusic;
    [SerializeField]
    AudioClip normalMusic;
    [SerializeField]
    AudioClip transitionEffects;
    int ennemiesChassing;

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
        SoundManager.Instance.pauseMusic();
        StartCoroutine(newRandomLevel());
    }

    private IEnumerator newRandomLevel()
    {
        CurrentLevel += 1;
        Time.timeScale = 0;
        //Animation panelUp = StartAnimation();
        //yield return WaitForWaitForAnimation(panelUp);
        Destroy(DungeonFactory.Instance.roomHolder);

        int roomsNumber = (int)Mathf.Log(Mathf.Pow((float)currentLevel,2f),2f)+5;
        DungeonFactory.Instance.generateDungeon(roomsNumber);

        float x = (float)playerStartRoom.x + (float)DungeonFactory.Instance.roomMaxDimensions.x/ 2;
        float y = (float)playerStartRoom.y + (float)DungeonFactory.Instance.roomMaxDimensions.y/ 2;
        Vector2 middleRoom = new Vector2(x,y);
        player.transform.position = new Vector3(middleRoom.x, middleRoom.y, 0);
        CameraController.Instance.setPosition(new Vector3(middleRoom.x, middleRoom.y, -10));

        //Animation panelDown = StartAnimation();
        //SoundManager.Instance.playSingle(transitionEffects);
        //yield return WaitForWaitForAnimation(panelUp);
        yield return null;
        SoundManager.Instance.playMusic(normalMusic);
        Time.timeScale = 1;
    }

    private IEnumerator WaitForAnimation(Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);
    }

    public void changeToAttackMusic(bool value)
    {
        if (value)
        {
            if (ennemiesChassing == 0)
            {
                SoundManager.Instance.playMusic(attackMusic);
            }
            ennemiesChassing++;
        }
        else
        {
            ennemiesChassing--;
            if (ennemiesChassing == 0)
            {
                SoundManager.Instance.playMusic(normalMusic);
            }
        }
    }

}
