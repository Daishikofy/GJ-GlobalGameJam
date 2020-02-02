#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Events;

public class EnnemyController : MonoBehaviour, IReaparable, IDamageable
{
    [SerializeField]
    float speed;
    [SerializeField]
    int power;
    [SerializeField]
    int lifePoints;
    [SerializeField]
    int sadnessPoints;
    [SerializeField]
    float stopDistance;
    [SerializeField]
    float patrolCadence;
    [SerializeField]
    CircleCollider2D searchField;

    Rigidbody2D myRigidbody;
    Rect roomArea;
    Vector2 patrolPoint;
    Transform playerPosition;

    Vector2 inicialPosition;

    float timeSincePatrol;
    bool isChasing = false;

    [Space]
    [Space]
    [SerializeField]
    AudioClip attackingAttack;
    [SerializeField]
    AudioClip takesDamages;
    [SerializeField]
    AudioClip whenDead;
    [SerializeField]
    AudioClip takesHealing;

    [HideInInspector]
    public UnityEvent isDead;
    [HideInInspector]
    public UnityEvent isFree;

    public int LifePoints
    {
        get { return lifePoints; }
        set { lifePoints = value; }
    }
    public int SadnessPoints
    {
        get { return sadnessPoints; }
        set { sadnessPoints = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        inicialPosition = this.transform.position;
        Vector2Int posMin = GetComponentInParent<Room>().position;
        Vector2Int dimensions;
        dimensions = DungeonFactory.Instance.roomMaxDimensions;
        Debug.Log(dimensions);
        roomArea = new Rect(posMin, dimensions);

        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isChasing)
        {
            if (Vector2.Distance(transform.position, playerPosition.position) < stopDistance)
                patrolPoint = transform.position;
            else
                patrolPoint = playerPosition.position;
            if (!roomArea.Contains(this.transform.position))
            {
                isChasing = false;
                this.transform.position = inicialPosition;
            }
        }
        else
        {
            timeSincePatrol += Time.deltaTime;
            if (timeSincePatrol >= patrolCadence)
            {
                patrolPoint = randomPatrolPoint();
                timeSincePatrol = 0;
            }
        }
    }
    private void FixedUpdate()
    {
        float step = speed * Time.deltaTime;
        myRigidbody.MovePosition(Vector2.MoveTowards(myRigidbody.transform.position, patrolPoint, step));
    }
    public void onDamage(int damage)
    {
        LifePoints -= damage;
        if (LifePoints < 0)
        {
            LifePoints = 0;
            isKo();
        }
        else
        {
            //TODO: Add an effect to show life
        }
    }

    public void onRepair(int value)
    {
        SadnessPoints -= value;
        if (SadnessPoints < 0)
        {
            SadnessPoints = 0;
            isHealed();
        }
        else
        {
            //TODO: Add an effect to show life
        }
    }

    private void isKo()
    {
        //ANIMATOR
        //SOUND
        Destroy(this.gameObject);
    }

    private void isHealed()
    {
        //ANIMATOR
        //SOUND
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = true;
            playerPosition = collision.transform;
            patrolPoint = playerPosition.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
            patrolPoint = randomPatrolPoint();
        }
    }

    private Vector2 randomPatrolPoint()
    {
        float x = Random.Range(roomArea.xMin, roomArea.xMax);
        float y = Random.Range(roomArea.yMin, roomArea.yMax);

        return new Vector2(x, y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        { }
        else
            patrolPoint = randomPatrolPoint();
            timeSincePatrol = 0;
    }
}
