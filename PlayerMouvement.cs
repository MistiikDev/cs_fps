using UnityEngine;

public class PlayerMouvement : Player
{
    public Rigidbody rb;
    
    public float walkSpeed = 20f;
    public float jumpForce = 150f;

    private Vector3 _playerVelocity;
    
    private float GRAVITY = (float)(9.81 / 8);
    private float playerHeight;
    
    public bool b_isCrouching;
    private bool _jumpRequested;
    private bool canJump;
    
    private Player player;
    
    public Vector3 GetPlayerVelocity()
    {
        return this._playerVelocity;
    }
    
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        playerHeight = this.transform.localScale.y;
    }

    public void Init(Player _player)
    {
        player = _player;
    }
    
    private void Crouch(bool isCrouching)
    {
        this.b_isCrouching = isCrouching;

        if (this.b_isCrouching)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x, playerHeight / 2, this.transform.localScale.z);
        }
        else
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x, playerHeight, this.transform.localScale.z);
        }
    }
    
    public bool b_isGrounded()
    {
        return Physics.Raycast(this.pCamera.player_camera.transform.position, Vector3.down, out RaycastHit hit, this.transform.localScale.y + 0.1f);
    }

    public bool b_isWalking()
    {
        return rb.linearVelocity.magnitude > 0.1f;
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
        
        if (this.b_isGrounded())
        {
            this.canJump = true;
        }
    }
    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;
        Vector3 targetVelocity = walkSpeed * Time.fixedDeltaTime * 60f * moveDirection;

        _playerVelocity = rb.linearVelocity;

        Vector3 velocityChange = targetVelocity - new Vector3(_playerVelocity.x, 0f, _playerVelocity.z);
        velocityChange = Vector3.ClampMagnitude(velocityChange, walkSpeed);
        
        rb.AddForce(velocityChange + (Vector3.down * GRAVITY), ForceMode.VelocityChange); // Add more gravity;

        if (_jumpRequested && canJump)
        {
            this.canJump = false;
            
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;
            
            rb.AddForce(Vector3.up * this.jumpForce, ForceMode.Impulse);
            _jumpRequested = false;
        }
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, _playerVelocity.y, rb.linearVelocity.z);
    }
}
