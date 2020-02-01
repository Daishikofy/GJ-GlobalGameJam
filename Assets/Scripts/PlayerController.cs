#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int attackPower;
    [SerializeField]
    private int healPower;
    [SerializeField]
    private int healLauchLevel;
    private int healMeter;
    [SerializeField]
    private float healRadius;
    [SerializeField]
    private BoxCollider2D boxCollider;
    private PlayerInput playerInput;
    private Rigidbody2D myRigidbody;
    private Vector2 playerMovement;
    private Vector2 playerDirection;
    bool isMoving = false;
    Animator animator;

    public int HealMeter
    {
        get { return healMeter; }
        set { healMeter = value; } 
    }

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
        myRigidbody = GetComponent<Rigidbody2D>();

        playerDirection = Vector2.down;
        animator.SetFloat("X", playerDirection.x);
        animator.SetFloat("Y", playerDirection.y);
        HealMeter = 3;
    }

    private void Update()
    {
        if (playerMovement == Vector2.zero)
            animator.SetBool("isMoving", false);
        else
            animator.SetBool("isMoving", true);
    }

    private void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + playerMovement * Time.deltaTime);
    }

    private void horizontal(float value)
    {
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
        animator.SetFloat("X", playerDirection.x);
        animator.SetFloat("Y", playerDirection.y);
    }

    private void vertical(float value)
    {
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
        animator.SetFloat("X", playerDirection.x);
        animator.SetFloat("Y", playerDirection.y);
    }

    private void startHealing()
    {
        if (HealMeter < healLauchLevel) return;

        HealMeter -= healLauchLevel;
        //ANIMATION : Start healing animation, in the middle, activates collider and hurt ennemies
        //SOUND : Healing attack sound
        
    }

    private void healing()
    {
        Collider2D[] ennemies = Physics2D.OverlapCircleAll(this.transform.position, healRadius, 8);
        Debug.Log("There are " + ennemies.Length + " ennemies in the circle");
        foreach (var ennemy in ennemies)
        {
            ennemy.GetComponent<IDamageable>().onDamage(attackPower);
        }
    }

    private void startAttacking()
    {
        //ANIMATION : Start attack animation
        //SOUND : Attacking attack sound
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ennemy")) return;
        if (collision.IsTouching(boxCollider))//The fist hit the ennemy
        {
            collision.GetComponent<IDamageable>().onDamage(attackPower);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, healRadius);
    }

}
