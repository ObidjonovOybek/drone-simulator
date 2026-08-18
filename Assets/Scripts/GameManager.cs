using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player References")]
    public SimplePlayer player;
    public CharacterController playerCharacterController;
    public Camera playerCamera;

    [Header("Drone References")]
    public DroneSwitch droneSwitch;
    public DroneCrash droneCrash;
    public DroneCameraToggle droneCameraToggle;

    [Header("Default Spawn")]
    public Transform defaultSpawnPoint;

    private Vector3 savedCheckpointPosition;
    private Quaternion savedCheckpointRotation;

    private bool hasCheckpoint = false;
    private bool isDroneCrashState = false;
    private bool isPlayerDeathState = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (player == null)
            player = FindObjectOfType<SimplePlayer>();

        if (playerCharacterController == null && player != null)
            playerCharacterController = player.GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R bosildi");
            RestartByState();
        }
    }

    public void SaveCheckpoint()
    {
        if (player == null)
        {
            Debug.LogWarning("SaveCheckpoint: player null");
            return;
        }

        savedCheckpointPosition = player.transform.position;
        savedCheckpointRotation = player.transform.rotation;
        hasCheckpoint = true;

        Debug.Log("Checkpoint saqlandi: " + savedCheckpointPosition);
    }

    public void SaveCheckpoint(Transform point)
    {
        if (point == null)
        {
            Debug.LogWarning("SaveCheckpoint: point null");
            return;
        }

        savedCheckpointPosition = point.position;
        savedCheckpointRotation = point.rotation;
        hasCheckpoint = true;

        Debug.Log("Checkpoint saqlandi: " + savedCheckpointPosition);
    }

    public void DroneCrashed()
    {
        isDroneCrashState = true;
        isPlayerDeathState = false;
        Debug.Log("Drone crashed - R bosilsa checkpointga qaytadi");
    }

    public void PlayerDied()
    {
        isPlayerDeathState = true;
        isDroneCrashState = false;
        Debug.Log("Player died - R bosilsa default spawn ga qaytadi");
    }

    void RestartByState()
    {
        if (isDroneCrashState)
        {
            RespawnAtCheckpoint();
        }
        else if (isPlayerDeathState)
        {
            RespawnAtDefault();
        }
        else
        {
            Debug.Log("State yo'q");

            if (hasCheckpoint)
                RespawnAtCheckpoint();
            else
                RespawnAtDefault();
        }

        isDroneCrashState = false;
        isPlayerDeathState = false;
    }

    void RespawnAtCheckpoint()
    {
        if (!hasCheckpoint)
        {
            Debug.Log("Checkpoint yo'q, default spawn ga qaytyapti");
            RespawnAtDefault();
            return;
        }

        MovePlayer(savedCheckpointPosition, savedCheckpointRotation);
        ResetAllModesAfterRespawn();

        Debug.Log("Checkpointga qaytdi");
    }

    void RespawnAtDefault()
    {
        if (defaultSpawnPoint == null)
        {
            Debug.LogWarning("Default spawn yo'q, scene reload bo'lyapti");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        MovePlayer(defaultSpawnPoint.position, defaultSpawnPoint.rotation);
        ResetAllModesAfterRespawn();

        Debug.Log("Default spawn ga qaytdi");
    }

    void ResetAllModesAfterRespawn()
    {
        // Avval camera state ni third person holatga qaytar
        if (droneCameraToggle != null)
        {
            droneCameraToggle.SetThirdPerson();
        }

        // Keyin drone crash state ni reset qil
        if (droneCrash != null)
        {
            droneCrash.ResetCrashState();
        }

        // Keyin drone mode ni to'liq o'chir
        if (droneSwitch != null)
        {
            droneSwitch.ForceResetDroneMode();
        }

        // Oxirida playerni normal holatga qaytar
        if (player != null)
        {
            player.ResetAfterRespawn();
            player.SetDroneMode(false);
        }

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }
    }

    void MovePlayer(Vector3 position, Quaternion rotation)
    {
        if (player == null)
        {
            Debug.LogError("Player null");
            return;
        }

        if (playerCharacterController == null)
            playerCharacterController = player.GetComponent<CharacterController>();

        if (playerCharacterController != null)
            playerCharacterController.enabled = false;

        player.transform.SetPositionAndRotation(position, rotation);

        if (playerCharacterController != null)
            playerCharacterController.enabled = true;

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        Debug.Log("Player moved: " + position);
    }
}