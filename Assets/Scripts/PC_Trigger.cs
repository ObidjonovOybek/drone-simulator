using UnityEngine;

public class PCInteraction : MonoBehaviour
{
    public GameObject interactionUI;
    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Kompyuter ochildi");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionUI.SetActive(true);
            playerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionUI.SetActive(false);
            playerNear = false;
        }
    }
}