using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviourPunCallbacks
{
    [Header("Reference GameObjects")]
    [SerializeField] private Transform camController;

    [Header("Character Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float runSpeed = 15f;
    [SerializeField] private float LookDownConstraint = -60f;
    [SerializeField] private float LookUpConstraint = 60f;
    [SerializeField] private bool invertMouse;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletImpact;
    [SerializeField] private float bulletImpactDuration = 5f;
    [SerializeField] private float MaximumCharge = 20f;
    [SerializeField] private float rechargeRate = 6f;
    [SerializeField] private float depletedRechargeRate = 10f;

    private Gun gun;
    private UIController uiController;
    private CharacterController charController;
    private Health health;
    private float verticalRotationStore;
    private float currentCharge;
    private bool batteryDepleted = false;
    private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        gun = GetComponent<Gun>();
        uiController = GameObject.Find("Canvas").GetComponent<UIController>();
        charController = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        UpdateHealth();
        mainCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        currentCharge = MaximumCharge;
        uiController = UIController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Movement();
        MouseLook();

        currentCharge = Mathf.Clamp((currentCharge += rechargeRate * Time.deltaTime), 0, MaximumCharge);
        UpdateBatteryUI();

        if (!batteryDepleted)
        {
            if (Input.GetButtonDown("Fire1"))
                Shoot();
        } else
        {
            currentCharge = Mathf.Clamp((currentCharge += depletedRechargeRate * Time.deltaTime), 0, MaximumCharge);
            if (currentCharge == MaximumCharge)
            {
                UpdateBatteryUI();
                ;
                batteryDepleted = false;
            }
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
        if (!photonView.IsMine)
        {
            return;
        }

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
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                PhotonNetwork.Instantiate(bulletImpact.name, hit.point, Quaternion.identity);
                hit.collider.gameObject.GetPhotonView().RPC(nameof(TakeDamage), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            GameObject impact = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(impact, bulletImpactDuration);
        }

        currentCharge = Mathf.Clamp((currentCharge -= gun.AmmoCost), 0, MaximumCharge);
        UpdateBatteryUI();

        if (currentCharge == 0)
            batteryDepleted = true;
        else
            UpdateBatteryUI();
    }

    private void UpdateBatteryUI()
    {
        uiController.BatteryCharge.fillAmount = (currentCharge / MaximumCharge);
    }

    [PunRPC]
    public void TakeDamage(int myPhotonID)
    {
        if (photonView.IsMine)
        {
            int damage = 10;
            health.RemoveHealth(damage);
            if (health.GetCurrentHealth() < 1)
            {
                GameManager.instance.UpdateStatSend(myPhotonID, 0, 1);
                GameManager.instance.UpdateStatSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
                SpawnManager.instance.Death();
            }
            UpdateHealth();
        }
    }

    public void Heal(int Heal)
    {
        health.AddHealth(Heal);
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        uiController.HealthBar.fillAmount = health.GetHealthPerentage();
    }
}
