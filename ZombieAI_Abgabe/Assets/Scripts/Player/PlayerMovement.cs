using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float lookSpeed = 2f;

    private PlayerInput inputActions;  // Achte darauf, dass dieser Name mit dem generierten Skript übereinstimmt
    private Vector2 moveInput;
    private Vector2 lookInput;

    private Rigidbody rb;
    private Transform cameraTransform;

    private void Awake()
    {
        inputActions = new PlayerInput();  // Stelle sicher, dass dieser Name korrekt ist
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        inputActions.Movement.Enable();
    }

    private void OnDisable()
    {
        inputActions.Movement.Disable();
    }

    private void Start()
    {
        inputActions.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Movement.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Movement.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void Update()
    {
        // Bewegung
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.deltaTime;
        move = transform.TransformDirection(move);
        rb.MovePosition(rb.position + move);

        // Blickrichtung
        Vector2 look = lookInput * lookSpeed * Time.deltaTime;
        transform.Rotate(0, look.x, 0);
        cameraTransform.Rotate(-look.y, 0, 0);
    }
}
