using UnityEngine;

public class MapToggle : MonoBehaviour
{
    [Header("Map Objects")]
    public GameObject blurBackground;
    public GameObject bigMap;
    public GameObject smallMap;

    [Header("Main UI")]
    public GameObject mapButtonText;
    public GameObject keyboardUI;
    public GameObject joystickUI;
    public GameObject cameraModeIcon;

    [Header("Mission UI")]
    public GameObject startButton;
    public GameObject pointButton;
    public GameObject clearButton;

    private bool mapOpen = false;

    void Start()
    {
        blurBackground.SetActive(false);
        bigMap.SetActive(false);
        smallMap.SetActive(true);

        mapButtonText.SetActive(true);
        keyboardUI.SetActive(true);
        joystickUI.SetActive(true);
        cameraModeIcon.SetActive(true);

        startButton.SetActive(false);
        pointButton.SetActive(false);
        clearButton.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapOpen = !mapOpen;

            blurBackground.SetActive(mapOpen);
            bigMap.SetActive(mapOpen);
            smallMap.SetActive(!mapOpen);

            mapButtonText.SetActive(!mapOpen);
            keyboardUI.SetActive(!mapOpen);
            joystickUI.SetActive(!mapOpen);
            cameraModeIcon.SetActive(!mapOpen);

            startButton.SetActive(mapOpen);
            pointButton.SetActive(mapOpen);
            clearButton.SetActive(mapOpen);
        }
    }
}