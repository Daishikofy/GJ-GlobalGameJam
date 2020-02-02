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
    float inicialSpeed;
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

    int currentKarmaLevel;
    
    PlayerInput playerInput;
    Rigidbody2D myRigidbody;
    Vector2 playerMovement;
    Vector2 playerDirection;
    bool isMoving = false;
    bool isAttacking = false;
    Animator animator;
    [Space]
    [Space]
    [SerializeField]
    GameObject healingAnimation;
    [SerializeField]
    GameObject powerUpAnimation;
    [SerializeField]
    GameObject canHealAnimation;
    [SerializeField]
    GameObject canHealPicture;

    [Space]
    [Space]
    [SerializeField]
    AudioClip[] walking;
    [SerializeField]
    AudioClip healingAttack;
    [SerializeField]
    AudioClip attackingAttack;
    [SerializeField]
    AudioClip takesDamages;
    [SerializeField]
    AudioClip whenDead;
    [SerializeField]
    AudioClip powerUp;
    [SerializeField]
    AudioClip powerDown;

    [HideInInspector]
    public IntEvent updateKarma;
    [HideInInspector]
    public IntEvent updateLife;
    [HideInInspector]
    public IntEvent updateHealMeter;
    [HideInInspector]
    public UnityEvent updateStatus;

    [HideInInspector]
    public int savedGhosts;
    [HideInInspector]
    public int killedGhosts;

    

    //Getters ans setters here
    #region Getters&Setters
    public int Karma
    {
        get { return karma; }
        set {
            karma = value;
            updateKarma.Invoke(karma);
            if (karma % 20 != currentKarmaLevel)
                changeKarmaLevel();
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
            if (canHealPicture.activeSelf && (healMeter < healLauchLevel))
                canHealPicture.SetActive(false);
            else if (!canHealPicture.activeSelf && (healMeter >= healLauchLevel))
            {
                Instantiate(canHealAnimation, new Vector3(0, 0.5f, 0), Quaternion.identity, this.transform);
                canHealPicture.SetActive(true);
            }
        }        
    }

    public int Life {
        get { return currentLife; }
        set {
            currentLife = value;
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
        animator = GetComponent<Animator>();
        playerInput = new PlayerInput();
        playerInput.Enable();
        playerInput.Player.Horizontal.performed += context => horizontal(context.ReadValue<float>());
        playerInput.Player.Vertical.performed += context => vertical(context.ReadValue<float>());
        playerInput.Player.Attack1.performed += context => startHealing();
        playerInput.Player.Attack2.performed += context => startAttacking();
        playerInput.Player.Interaction.performed += context => interacting();
        myRigidbody = GetComponent<Rigidbody2D>();

        playerDirection = Vector2.down;
        // animator.SetFloat("X", playerDirection.x);
        // animator.SetFloat("Y", playerDirection.y);
        attackCollider.isTrigger = true;
        Life = maxLife;
        Karma = 0;
        HealMeter = 0;
        canHealPicture.SetActive(false);
        inicialSpeed = speed;
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
        isMoving = (value != 0);
        animator.SetBool("IsWalking", isMoving);
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
        isMoving = (value != 0);
        animator.SetBool("IsWalking", isMoving);
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

    private void interacting()
    {
        Vector3 startPoint = transform.position + Vector3.up * 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, playerDirection, 0.5f);
        Debug.Log("fraction: " + hit.fraction);
        Debug.DrawRay(startPoint, playerDirection, Color.red, 1);
      
        IInteractible interactable = hit.collider.gameObject.GetComponent<IInteractible>();
        //Debug.Log(hit.collider.gameObject.name);

        if (interactable == null) return;
        interactable.onInteraction(this);
    }

    private void startHealing()
    {
        if (HealMeter < healLauchLevel) return;
        isAttacking = true;
        HealMeter -= healLauchLevel;
        Instantiate(healingAnimation, this.transform);
        //SOUND : Healing attack sound
        //SoundManager.Instance.playSingle(healingAttack);
        if (isMoving)
        {
            isMoving = false;
            playerMovement = Vector2.zero;
        }
    }

    private void startAttacking()
    {
        isAttacking = true;
        animator.SetBool("IsWalking", false);
        animator.SetTrigger("IsAttacking");
        //SOUND : Attacking attack sound
        //SoundManager.Instance.playSingle(attackingAttack);
        if (isMoving)
        {
            isMoving = false;
            playerMovement = Vector2.zero;
        }
    }

    private void killedEnnemy()
    {
        killedGhosts++;
        Karma -= 1;
    }
    private void savedEnnemy()
    {
        savedGhosts++;
        Life += 1;
        Karma += 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ennemy")) return;
        if (collision.IsTouching(attackCollider))//The fist hit the ennemy
        {
            Debug.Log("Attacking the ennemy");
            collision.GetComponent<IDamageable>().onDamage(attackPower);
            var controller = collision.GetComponent<EnnemyController>();
            //controller.isDead.RemoveAllListeners();
            controller.isDead.RemoveListener(killedEnnemy);
            controller.isDead.AddListener(killedEnnemy);
        }
    }
    public void onDamage(int damage)
    {
        Life = Life - damage;
        //SOUND : Takes damages
        //SoundManager.Instance.playSingle(takesDamages);
        animator.SetTrigger("TakesDamage");
    }

    private void isKo()
    {
        //ANIMATION : Play death animation
        //SOUND : Death sound
        //SoundManager.Instance.playSingle(whenDead);
        SoundManager.Instance.pauseMusic();
        updateStatus.Invoke();
    }

    private void changeKarmaLevel()
    {
        int newKarmaLevel = Mathf.Abs(karma) % 20;
        if (newKarmaLevel > currentKarmaLevel)
        {
            Instantiate(powerUpAnimation, new Vector3(0, 0.5f, 0), Quaternion.identity, this.transform);
            if (Karma > 0)
            {// bon karma
                healPower++;
                speed += 0.5f;
            }
            else if (Karma < 0) // mauvais
            {
                attackPower++;
            }
            //SOUND: power up
            //SoundManager.Instance.playSingle(powerUp);
        }
        else if (newKarmaLevel < currentKarmaLevel)
        {
            if (Karma > 0)
            {// bon karma
                healPower--;
                speed -= 0.5f;
            }
            else if (Karma < 0) // mauvais
            {
                attackPower--;
            }
            //ANIMATION: Power down
            //SOUND: power down
            //SoundManager.Instance.playSingle(powerDown);
        }
        else { attackPower = 1; healPower = 1; speed = inicialSpeed; }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, healRadius);
    }
    public void animationTriggeredHealing()
    {
        Collider2D[] ennemies = Physics2D.OverlapCircleAll(this.transform.position, healRadius);
        Debug.Log("There are " + ennemies.Length + " objects in the circle");
        foreach (var thing in ennemies)
        {
            if (thing.CompareTag("Ennemy") && !thing.isTrigger)
            {
                Debug.Log("Ennemy");
                thing.GetComponent<IReaparable>().onRepair(healPower);
                var controller = thing.GetComponent<EnnemyController>();
                controller.isFree.RemoveListener(savedEnnemy);
                controller.isFree.AddListener(savedEnnemy);
            }
        }
        isAttacking = false;
    }
    private void animationTriggeredAttacking()
    {
        attackCollider.enabled = true;
        isAttacking = false;
    }
    private void animationTriggeredEndAttacking()
    {
        attackCollider.enabled = false;
        isAttacking = false;
    }
}
