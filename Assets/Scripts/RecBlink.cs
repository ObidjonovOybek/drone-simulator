using UnityEngine;

public class RecBlink : MonoBehaviour
{
    public GameObject recObject;
    public float blinkInterval = 0.5f;

    private float timer;

    void Update()
    {
        if (recObject == null) return;

        timer += Time.deltaTime;

        if (timer >= blinkInterval)
        {
            recObject.SetActive(!recObject.activeSelf);
            timer = 0f;
        }
    }
}