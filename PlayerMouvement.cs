using UnityEngine;

public class PlayerMouvement : MonoBehaviour
{
    public Rigidbody rb;
    
    public float walkSpeed = 20f;
    public float jumpForce = 150f;

    private Vector3 _playerVelocity;
    
    private float GRAVITY = (float)(9.81 / 8);
    private float playerHeight;
    
    private bool isCrouching;
    private bool _jumpRequested;

    public Vector3 GetPlayerVelocity()
    {
        return this._playerVelocity;
    }
    
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        playerHeight = this.transform.localScale.y;
    }

    private void Crouch(bool isCrouching)
    {
        this.isCrouching = isCrouching;

        if (this.isCrouching)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x, playerHeight / 2, this.transform.localScale.z);
        }
        else
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x, playerHeight, this.transform.localScale.z);
        }
    }
    
    private bool b_isGrounded()
    {
        return Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 0.2f);
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpRequested = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch(true);
        } else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Crouch(false);
        }

    }
    void FixedUpdate()
    {
        // Get input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;
        Vector3 targetVelocity = walkSpeed * Time.fixedDeltaTime * 60f * moveDirection;

        _playerVelocity = rb.linearVelocity;

        Vector3 velocityChange = targetVelocity - new Vector3(_playerVelocity.x, 0f, _playerVelocity.z);
        velocityChange = Vector3.ClampMagnitude(velocityChange, walkSpeed);
        
        rb.AddForce(velocityChange + (Vector3.down * GRAVITY), ForceMode.VelocityChange); // Add more gravity;

        if (_jumpRequested)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;
            
            rb.AddForce(Vector3.up * this.jumpForce, ForceMode.Impulse);
            _jumpRequested = false;
        }
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, _playerVelocity.y, rb.linearVelocity.z);
    }
}
