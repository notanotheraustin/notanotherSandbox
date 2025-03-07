using UnityEngine;

public class FPSController : MonoBehaviour
{
    public CharacterController controller;
    public Camera playerCamera;
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float crouchSpeed = 1.5f;
    public float jumpHeight = 1.0f;
    public float gravity = -9.81f;
    public KeyCode noclipKey = KeyCode.N;
    public float mouseSensitivity = 100f;
    
    private Vector3 velocity;
    private bool isGrounded;
    private bool isNoclip = false;
    private float originalControllerHeight;
    private Vector3 originalCenter;
    private float xRotation = 0f;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalControllerHeight = controller.height;
        originalCenter = controller.center;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        MouseLook();
        
        if (Input.GetKeyDown(noclipKey))
        {
            ToggleNoclip();
        }
        
        if (isNoclip)
        {
            NoclipMovement();
        }
        else
        {
            NormalMovement();
        }
    }
    
    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    void NormalMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            speed = crouchSpeed;
            controller.height = originalControllerHeight / 2;
            controller.center = new Vector3(0, originalCenter.y / 2, 0);
        }
        else
        {
            controller.height = originalControllerHeight;
            controller.center = originalCenter;
        }
        
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    void NoclipMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0;
        if (Input.GetKey(KeyCode.Space)) moveY = 1;
        if (Input.GetKey(KeyCode.LeftControl)) moveY = -1;
        
        Vector3 move = transform.right * moveX + transform.up * moveY + transform.forward * moveZ;
        transform.position += move * runSpeed * Time.deltaTime;
    }
    
    void ToggleNoclip()
    {
        isNoclip = !isNoclip;
        controller.enabled = !isNoclip;
        velocity = Vector3.zero;
    }
}
