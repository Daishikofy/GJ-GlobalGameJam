#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnnemyController : MonoBehaviour, IReaparable, IDamageable
{
    [SerializeField]
    float speed;
    [SerializeField]
    int power;
    [SerializeField]
    float coolDown;
    [SerializeField]
    int lifePoints;
    int maxPoints;
    [SerializeField]
    int sadnessPoints;
    int maxSadnesse;
    [SerializeField]
    float stopDistance;
    [SerializeField]
    float patrolCadence;
    [SerializeField]
    CircleCollider2D searchField;


    [Space]
    [SerializeField]
    GameObject drop;
    [SerializeField]
    Vector2 dropCadence;
    bool canDrop = true;

    Rigidbody2D myRigidbody;
    Rect roomArea;
    Vector2 patrolPoint;
    Transform playerPosition;

    Vector2 inicialPosition;

    float timeSincePatrol;
    bool isChasing = false;

    bool canAttack = true;

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
    private SpriteRenderer renderer;
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
        roomArea = new Rect(posMin, dimensions);

        myRigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        maxPoints = lifePoints;
    }

    // Update is called once per frame
    private void Update()
    {
        if (canDrop)
        {
            float randTime = Random.Range(dropCadence.x, dropCadence.y);
            Instantiate(drop, this.transform.position, Quaternion.identity);
            StartCoroutine(coolDownActionDrop(randTime));
        }
        if (isChasing)
        {
            if (Vector2.Distance(transform.position, playerPosition.position) < stopDistance)
                patrolPoint = transform.position;
            else
                patrolPoint = playerPosition.position;
            if (!roomArea.Contains(this.transform.position))
            {
                GameManager.Instance.changeToAttackMusic(false);
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
            Color color = Color.white;
            color.a = ((float)lifePoints / (float)maxSadnesse);
            renderer.color = color;
        }
    }

    public void onRepair(int value)
    {
        Debug.Log("Entrou");
        SadnessPoints = SadnessPoints - value;
        if (SadnessPoints < 0)
        {
            SadnessPoints = 0;
            isHealed();
            Debug.Log("Is repaired");
        }
        else
        {
            
            float scale = ((float)SadnessPoints / ((float)maxPoints*2))+0.5f;
                Vector3 size = new Vector3(scale, scale, 0);
                this.transform.localScale = size;

            /*Color color = Color.white;
            color.a = ((float)lifePoints / (float)maxPoints);
            renderer.color = color;*/
        }
    }

    private void isKo()
    {
        //ANIMATOR
        //SOUND
        isDead.Invoke();
        Destroy(this.gameObject);
    }

    private void isHealed()
    {
        //ANIMATOR
        //SOUND
        isFree.Invoke();
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = true;
            GameManager.Instance.changeToAttackMusic(true);
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
        {
            if (canAttack)
            {
                collision.gameObject.GetComponent<IDamageable>().onDamage(power);
                StartCoroutine(coolDownActionAttack(coolDown));
            }
        }
        else
            patrolPoint = randomPatrolPoint();
            timeSincePatrol = 0;
    }

    private IEnumerator coolDownActionAttack(float coolDownTime)
    {
        canAttack = false;
        yield return new WaitForSeconds(coolDownTime);

        canAttack = true;
    }
    private IEnumerator coolDownActionDrop(float coolDownTime)
    {
        canDrop = false;
        yield return new WaitForSeconds(coolDownTime);

        canDrop = true;
    }
}
