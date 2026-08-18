using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulatorDoor : MonoBehaviour
{
    public int sceneIndex = 1;
    public GameObject pressEText;
    bool playerInside = false;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (pressEText) pressEText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (pressEText) pressEText.SetActive(false);
        }
    }
}