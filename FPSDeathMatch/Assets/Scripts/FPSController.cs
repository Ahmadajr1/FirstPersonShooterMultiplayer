using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [SerializeField] private Transform camController;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float runSpeed = 15f;
    [SerializeField] private float LookDownConstraint = -60f;
    [SerializeField] private float LookUpConstraint = 60f;
    [SerializeField] private bool invertMouse;

    [SerializeField] private GameObject bulletImpact;
    [SerializeField] private float bulletImpactDuration = 5f;
    [SerializeField] private float firingRate = 1f;
    [SerializeField] private Gun gun;

    private CharacterController charController;
    private float verticalRotationStore;
    private float firingTimer = 0f;

    private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        MouseLook();
        if (firingTimer <= firingRate)
            firingTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && firingTimer >= firingRate)
        {
            Shoot();
            firingTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButton(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void LateUpdate()
    {
        mainCam.transform.rotation = camController.rotation;
        mainCam.transform.position = camController.position;
    }

    private void Movement()
    {
        Vector3 moveVertical = transform.forward * Input.GetAxisRaw("Vertical");
        Vector3 moveHorizontal = transform.right * Input.GetAxisRaw("Horizontal");
        float currentSpeed = (Input.GetKey(KeyCode.LeftShift)) ? runSpeed : moveSpeed;
        Vector3 movement = (moveVertical + moveHorizontal).normalized;
        movement *= currentSpeed;
        charController.Move(movement * Time.deltaTime);
    }

    private void MouseLook()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        float yRotation = transform.rotation.eulerAngles.y + mouseInput.x;
        transform.rotation = Quaternion.Euler(transform.rotation.x, yRotation, transform.rotation.z);
        float invert = (!invertMouse) ? -1f : 1f;
        verticalRotationStore += mouseInput.y * invert;
        verticalRotationStore = Mathf.Clamp(verticalRotationStore, LookDownConstraint, LookUpConstraint);
        camController.rotation = Quaternion.Euler(verticalRotationStore, camController.eulerAngles.y, camController.eulerAngles.z);
    }

    private void Shoot()
    {
        Ray rayShot = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
        gun.Flash();
        if (Physics.Raycast(rayShot, out RaycastHit hit))
        {
            GameObject impact = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(impact, bulletImpactDuration);
        }
    }
}
