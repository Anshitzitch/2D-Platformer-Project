using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables appear in the Inspector, so you can tweak them without editing code.
    public float moveSpeed = 4f;       // How fast the player moves left/right

    //Jump realated variables for the Jump Feature (later)
    public float jumpForce = 5.5f;      // How strong the jump is (vertical speed)
    public Transform groundCheck;      // Empty child object placed at the player's feet
    public float groundCheckRadius = 0.2f; // Size of the circle used to detect ground
    public LayerMask groundLayer;      // Which layer counts as "ground" (set in Inspector)
    private Vector3 startPosition;  // where the player starts
    public float fallLimit = -6f;   // y-value that counts as "game over"
    public float topLimit = 6f;   // Y level above the screen

    // Private variables are used internally by the script.
    private Rigidbody2D rb;            // Reference to the Rigidbody2D component
    private bool isGrounded;           // True if player is standing on ground
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private int extraJumps;
    public int extraJumpsValue = 1; // Allows one double jump

    void Start()
    {
        // Grab the Rigidbody2D attached to the Player object once at the start.
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        extraJumps = extraJumpsValue;
    }

    void Update()
    {
        // --- Horizontal movement ---
        // Get input from keyboard (A/D or Left/Right arrows).
        float moveInput = Input.GetAxis("Horizontal");
        // Apply horizontal speed while keeping the current vertical velocity.
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- Flip the player based on movement direction ---
        if (moveInput != 0)
        {
            spriteRenderer.flipX = moveInput < 0;
        }

        // Jump realated code for the Jump Feature (later)
        // --- Ground check ---
        // Create an invisible circle at the GroundCheck position.
        // If this circle overlaps any collider on the "Ground" layer, player is grounded.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if (Input.GetButtonDown("Jump") && extraJumps > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            extraJumps--;
        }
        else if (Input.GetButtonDown("Jump") && extraJumps == 0 && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Respawn if player falls below the screen
        if (transform.position.y < fallLimit)
        {
            transform.position = startPosition;
            rb.linearVelocity = Vector2.zero; // stop any leftover motion
        }

        // Respawn if player goes above the screen
        if (transform.position.y > topLimit)
        {
            transform.position = startPosition;
            rb.linearVelocity = Vector2.zero;
        }

        // --- Animation handling ---
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("isJumping", !isGrounded);
    }
}
