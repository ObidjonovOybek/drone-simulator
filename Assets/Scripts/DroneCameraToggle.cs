using UnityEngine;

public class DroneCameraToggle : MonoBehaviour
{
    public Camera mainCamera;
    public Transform firstPersonFollowPoint;
    public CMFollow followScript;

    public GameObject crosshair; // Canvas > Crosshair
    public GameObject fpvHUD;    // Canvas > FPV_HUD

    [Header("First Person Settings")]
    public Vector3 firstPersonPosition = Vector3.zero;
    public Vector3 firstPersonRotation = Vector3.zero;

    public KeyCode toggleKey = KeyCode.V;

    private bool isFirstPerson = false;

    void Awake()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera ulanmagan!");
            enabled = false;
            return;
        }

        if (crosshair != null)
            crosshair.SetActive(false);

        if (fpvHUD != null)
            fpvHUD.SetActive(false);
    }

    void Start()
    {
        SetThirdPerson();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (isFirstPerson)
                SetThirdPerson();
            else
                SetFirstPerson();
        }
    }

    public void SetFirstPerson()
    {
        if (firstPersonFollowPoint == null)
        {
            Debug.LogError("FirstPersonFollowPoint ulanmagan!");
            return;
        }

        isFirstPerson = true;

        if (followScript != null)
            followScript.enabled = false;

        mainCamera.transform.SetParent(firstPersonFollowPoint, false);
        mainCamera.transform.localPosition = firstPersonPosition;
        mainCamera.transform.localRotation = Quaternion.Euler(firstPersonRotation);

        if (crosshair != null)
            crosshair.SetActive(true);

        if (fpvHUD != null)
            fpvHUD.SetActive(true);

        Debug.Log("FPV ON");
    }

    public void SetThirdPerson()
    {
        isFirstPerson = false;

        // Kamerani FPV parentdan chiqarib yuboramiz
        mainCamera.transform.SetParent(null, true);

        if (followScript != null)
        {
            followScript.enabled = false;
            followScript.enabled = true;
            followScript.SnapToTarget();
        }

        if (crosshair != null)
            crosshair.SetActive(false);

        if (fpvHUD != null)
            fpvHUD.SetActive(false);

        Debug.Log("Third Person ON");
    }

    public void ForceHideHUD()
    {
        SetThirdPerson();
    }

    public bool IsFirstPerson()
    {
        return isFirstPerson;
    }
}