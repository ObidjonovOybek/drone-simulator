using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorLook : MonoBehaviour
{
    public GameObject pressEText;
    public float distance = 3f;
    public int sceneIndex = 1;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, distance) && hit.collider.CompareTag("Door"))
        {
            if (pressEText) pressEText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            if (pressEText) pressEText.SetActive(false);
        }
    }
}