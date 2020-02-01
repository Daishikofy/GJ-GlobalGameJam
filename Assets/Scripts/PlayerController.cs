#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField]
    int maxLife;
    int currentLife;
    [Space]
    [SerializeField]
    float speed;
    [Space]
    [SerializeField]
    int attackPower;
    [SerializeField]
    BoxCollider2D attackCollider;
    [Space]
    [SerializeField]
    int healPower;
    [SerializeField]
    int healLauchLevel;
    [SerializeField]
    int healLunchNumber;
    int healMeter;
    [SerializeField]
    float healRadius;
    int karma;
    
    PlayerInput playerInput;
    Rigidbody2D myRigidbody;
    Vector2 playerMovement;
    Vector2 playerDirection;
    bool isMoving = false;
    Animator animator;

    IntEvent updateKarma;
    IntEvent updateLife;
    IntEvent updateHealMeter;
    UnityEvent updateStatus;

    //Getters ans setters here
    #region Getters&Setters
    public int Karma
    {
        get { return karma; }
        set {
            karma = value;
            updateKarma.Invoke(karma);
            }
    }

    public int HealMeter
    {
        get { return healMeter; }
        set
        {
            healMeter = value;
            if (healMeter < 0) healMeter = 0;
            updateHealMeter.Invoke(healMeter);
        }
         
    }

    public int Life {
        get { return currentLife; }
        set {
            Life = value;
            if (currentLife > maxLife) currentLife = maxLife;
            if (currentLife < 0)
            {
                isKo();
                currentLife = 0;
            }
            updateLife.Invoke(currentLife);
        }
    }
    #endregion gettersSetters
    // Use this for initialization
    void Start()
    {
        //animator = GetComponent<Animator>();
        playerInput = new PlayerInput();
        playerInput.Enable();
        playerInput.Player.Horizontal.performed += context => horizontal(context.ReadValue<float>());
        playerInput.Player.Vertical.performed += context => vertical(context.ReadValue<float>());
        playerInput.Player.Attack1.performed += context => startHealing();
        playerInput.Player.Attack2.performed += context => startAttacking();
        myRigidbody = GetComponent<Rigidbody2D>();

        playerDirection = Vector2.down;
        // animator.SetFloat("X", playerDirection.x);
        // animator.SetFloat("Y", playerDirection.y);
        attackCollider.isTrigger = true;
    }

    private void Update()
    {
       /* if (playerMovement == Vector2.zero)
            animator.SetBool("isMoving", false);
        else
            animator.SetBool("isMoving", true);*/
    }

    private void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + playerMovement * Time.deltaTime);
    }

    private void horizontal(float value)
    {
        //Debug.Log("Horizontal: " + value);
        if (value == 0 && playerMovement.y != 0)
            return;
        else
        {
            this.playerMovement.x = (int)value;
            this.playerMovement.y = 0;
        }
        if (playerMovement.x != 0 || playerMovement.y != 0)
            playerDirection = playerMovement;
        playerMovement *= speed;
       // animator.SetFloat("X", playerDirection.x);
        //animator.SetFloat("Y", playerDirection.y);
    }

    private void vertical(float value)
    {
        //Debug.Log("Vertical: " + value);
        if (value == 0 && playerMovement.x != 0)
            return;
        else
        {
            this.playerMovement.x = 0;
            this.playerMovement.y = (int)value;
        }
        if (playerMovement.x != 0 || playerMovement.y != 0)
            playerDirection = playerMovement;
        playerMovement *= speed;
        //animator.SetFloat("X", playerDirection.x);
        //animator.SetFloat("Y", playerDirection.y);
    }

    private void startHealing()
    {
        if (HealMeter < healLauchLevel) return;

        HealMeter -= healLauchLevel;
        //ANIMATION : Start healing animation, in the middle, activates collider and hurt ennemies
        //SOUND : Healing attack sound
        
    }

    private void startAttacking()
    {
        //ANIMATION : Start attack animation
        //SOUND : Attacking attack sound
    }

    private void healing()
    {
        Collider2D[] ennemies = Physics2D.OverlapCircleAll(this.transform.position, healRadius, 8);
        Debug.Log("There are " + ennemies.Length + " ennemies in the circle");
        foreach (var ennemy in ennemies)
        {
            ennemy.GetComponent<IReaparable>().onRepair(healPower);
            var controller = ennemy.GetComponent<EnnemyController>();
            controller.isFree.RemoveAllListeners();
            controller.isFree.AddListener(savedEnnemy);
        }
    }

    private void killedEnnemy()
    {
        Karma -= 1;
    }
    private void savedEnnemy()
    {
        Life += 1;
        Karma += 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ennemy")) return;
        if (collision.IsTouching(attackCollider))//The fist hit the ennemy
        {
            collision.GetComponent<IDamageable>().onDamage(attackPower);
            var controller = collision.GetComponent<EnnemyController>();
            controller.isDead.RemoveAllListeners();
            controller.isDead.AddListener(killedEnnemy);
        }
    }
    public void onDamage(int damage)
    {
        Life -= damage;
        //SOUND : Takes damages
        //ANIMATION : Takes damages
    }

    private void isKo()
    {
        //ANIMATION : Play death animation
        //SOUND : Death sound
        //MUSIC : Death music transition
        updateStatus.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, healRadius);
    }
}
