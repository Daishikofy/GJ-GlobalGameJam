#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    float patrolCadence;
    [SerializeField]
    CircleCollider2D searchField;

    Rigidbody2D myRigidbody;
    RectInt roomArea;
    Vector2 patrolPoint;

    float timeSincePatrol;
    bool isChasing = false;

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
        Vector2Int posMin = GetComponentInParent<Room>().position;
        Vector2Int dimensions = DungeonFactory.Instance.roomMaxDimensions;
        Debug.Log(dimensions);
        roomArea = new RectInt(posMin, dimensions);

        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {          
        if (isChasing) return;
        timeSincePatrol += Time.deltaTime;
        if (timeSincePatrol >= patrolCadence)
        {
            patrolPoint = randomPatrolPoint();
            timeSincePatrol = 0;
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
            patrolPoint = collision.transform.position;
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
