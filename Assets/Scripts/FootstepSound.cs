using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip footstepClip;

    public float stepDelay = 0.5f;

    private float stepTimer;

    void Update()
    {
        float move = Input.GetAxis("Vertical") + Input.GetAxis("Horizontal");

        if (move != 0)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                audioSource.PlayOneShot(footstepClip);
                stepTimer = stepDelay;
            }
        }
    }
}