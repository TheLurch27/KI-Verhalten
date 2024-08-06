using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    public Transform gun;
    public Transform gunDefaultPosition;
    public Transform gunAimPosition;
    public float aimSpeed = 5f;
    public float raycastRange = 100f;

    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction aimAction;

    private void Awake()
    {
        playerInput = new PlayerInput();
        fireAction = playerInput.Shooting.Fire;
        aimAction = playerInput.Shooting.Aim;

        fireAction.performed += _ => Fire();
    }

    private void OnEnable()
    {
        fireAction.Enable();
        aimAction.Enable();
    }

    private void OnDisable()
    {
        fireAction.Disable();
        aimAction.Disable();
    }

    private void Update()
    {
        Aim();
    }

    private void Fire()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, raycastRange))
        {
            Debug.Log("Hit: " + hit.transform.name);
            ZombieHealth zombieHealth = hit.transform.GetComponent<ZombieHealth>();
            if (zombieHealth != null)
            {
                int damage = hit.collider.CompareTag("Head") ? 100 : 25;
                Debug.Log("Applying damage: " + damage);
                zombieHealth.TakeDamage(damage);
            }
        }
    }


    private void Aim()
    {
        if (aimAction.ReadValue<float>() > 0)
        {
            gun.position = Vector3.Lerp(gun.position, gunAimPosition.position, aimSpeed * Time.deltaTime);
        }
        else
        {
            gun.position = Vector3.Lerp(gun.position, gunDefaultPosition.position, aimSpeed * Time.deltaTime);
        }
    }
}
