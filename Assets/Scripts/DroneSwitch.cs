using UnityEngine;

public class DroneSwitch : MonoBehaviour
{
    public SimplePlayer playerController;
    public Camera playerCamera;
    public Camera droneCamera;

    public GameObject droneObject;
    public Transform droneSpawnPoint;

    [Header("Drone Camera Setup")]
    public CameraMovement droneCameraMovement;
    public Transform droneFollowTarget;
    public DroneCameraToggle droneCameraToggle;

    [Header("Disable These In Drone")]
    public GameObject droneFirstPersonObject;

    private bool isDroneMode = false;

    void Start()
    {
        if (droneCamera != null)
            droneCamera.gameObject.SetActive(false);

        if (droneObject != null)
            droneObject.SetActive(false);

        if (droneFirstPersonObject != null)
            droneFirstPersonObject.SetActive(false);

        if (droneCameraToggle != null)
            droneCameraToggle.SetThirdPerson();
    }

    void Update()
    {
        if (!isDroneMode && Input.GetKeyDown(KeyCode.F))
            ActivateDrone();

        if (isDroneMode && Input.GetKeyDown(KeyCode.Q))
            DeactivateDrone();
    }

    void ActivateDrone()
    {
        isDroneMode = true;

        if (GameManager.instance != null)
            GameManager.instance.SaveCheckpoint();

        if (droneObject != null && droneSpawnPoint != null)
        {
            droneObject.transform.SetPositionAndRotation(
                droneSpawnPoint.position,
                droneSpawnPoint.rotation);

            Rigidbody rb = droneObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            droneObject.SetActive(true);
        }

        if (playerController != null)
            playerController.SetDroneMode(true);

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);

        if (droneCamera != null)
            droneCamera.gameObject.SetActive(true);

        if (droneCameraMovement != null && droneFollowTarget != null)
        {
            droneCameraMovement.SetFollowTarget(droneFollowTarget);
            droneCameraMovement.enabled = false;
            droneCameraMovement.enabled = true;
        }

        // Har safar dronega kirganda majburan third persondan boshlasin
        if (droneCameraToggle != null)
            droneCameraToggle.SetThirdPerson();

        if (droneFirstPersonObject != null)
            droneFirstPersonObject.SetActive(false);

        Debug.Log("Drone mode ON - Third person start");
    }

    void DeactivateDrone()
    {
        isDroneMode = false;

        if (droneCameraToggle != null)
            droneCameraToggle.SetThirdPerson();

        if (playerController != null)
            playerController.SetDroneMode(false);

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        if (droneCamera != null)
            droneCamera.gameObject.SetActive(false);

        if (droneFirstPersonObject != null)
            droneFirstPersonObject.SetActive(false);

        if (droneObject != null)
            droneObject.SetActive(false);

        Debug.Log("Drone mode OFF");
    }

    public void ForceResetDroneMode()
    {
        isDroneMode = false;

        if (droneCameraToggle != null)
            droneCameraToggle.SetThirdPerson();

        if (playerController != null)
            playerController.SetDroneMode(false);

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        if (droneCamera != null)
            droneCamera.gameObject.SetActive(false);

        if (droneFirstPersonObject != null)
            droneFirstPersonObject.SetActive(false);

        if (droneObject != null)
            droneObject.SetActive(false);

        Debug.Log("Drone reset bo'ldi");
    }
}