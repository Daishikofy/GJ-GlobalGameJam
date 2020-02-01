using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private PlayerInput playerInput;
    private Rigidbody2D rigidbody2D;
    private Vector2 playerMovement;
    [HideInInspector]
    public Vector2 playerDirection;
    bool isMoving = false;
    Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        playerInput = new PlayerInput();
        playerInput.Enable();
        playerInput.Player.Horizontal.performed += context => horizontal(context.ReadValue<float>());
        playerInput.Player.Vertical.performed += context => vertical(context.ReadValue<float>());
        rigidbody2D = GetComponent<Rigidbody2D>();

        playerDirection = Vector2.down;
        animator.SetFloat("X", playerDirection.x);
        animator.SetFloat("Y", playerDirection.y);
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
        rigidbody2D.MovePosition(rigidbody2D.position + playerMovement * Time.deltaTime);
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
}
