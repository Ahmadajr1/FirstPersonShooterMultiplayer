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



    private CharacterController charController;
    private float verticalRotationStore;
    private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        mainCam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        MouseLook();
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
}
