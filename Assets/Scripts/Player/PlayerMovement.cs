using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 25f;
    public float climbSpeed = 8f;

    [Header("References")]
    public GridManager gridManager;

    [Header("Ladder Platform")]
    private GameObject currentPlatform;
    public Collider2D playerCollider;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    bool isClimbing = false;
    public bool isNearLadder = false;
    private Ladder currentLadder;

    private Rigidbody2D rb;

    //public AnimatorOverrideController toolgunOverrideController;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetBool("isWalking", horizontalMove != 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
            //animator.SetBool("isJumping", true);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isClimbing && currentLadder != null)
            {
                Vector2 abovePos = currentLadder.transform.position + Vector3.up * gridManager.tileSize;
                gridManager.PlaceLadder(abovePos);
            }
            else
            {
                gridManager.PlaceLadder(transform.position);
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            gridManager.PlacePillar(transform.position);
        }

        if (!isClimbing && isNearLadder && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
            StartClimbing();

        if (isClimbing && Mathf.Abs(horizontalMove) > 0.1f)
            StopClimbing();

        if (isClimbing)
            HandleClimbing();

        HandleMouseRotation();
    }

    public void OnLanding()
    {
        //animator.SetBool("isJumping", false);
    }

    void FixedUpdate()
    {
        if (!isClimbing)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;
        }
    }
    void HandleMouseRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.localScale = mousePosition.x > transform.position.x ? Vector3.one : new Vector3(-1, 1, 1);
    }

    void HandleClimbing()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(0f, vertical * climbSpeed);

        if (currentLadder != null)
        {
            Vector3 pos = transform.position;
            pos.x = currentLadder.transform.position.x;
            transform.position = pos;
        }

        if (!isNearLadder)
            StopClimbing();
    }

    public void EnterLadder(Ladder ladder)
    {
        isNearLadder = true;
        currentLadder = ladder;
    }

    public void ExitLadder(Ladder ladder)
    {
        if (ladder == currentLadder)
        {
            isNearLadder = false;
            currentLadder = null;
            StopClimbing();
            Debug.Log("Player detached");
        }
    }

    void StartClimbing()
    {
        isClimbing = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.linearDamping = 0f;
        jump = false;
        //animator.SetBool("isClimbing", true);
    }

    void StopClimbing()
    {
        isClimbing = false;
        rb.gravityScale = 3f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.linearDamping = 0f;
        //animator.SetBool("isClimbing", false);
    }
}

