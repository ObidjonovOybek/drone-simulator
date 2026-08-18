using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DroneHUD : MonoBehaviour
{
    [Header("Drone")]
    public Transform droneTransform;
    public Rigidbody droneRb;

    [Header("HUD Text")]
    public TMP_Text altNumberText;
    public TMP_Text speedNumberText;

    [Header("REC Blink")]
    public Image recImage;
    public float blinkSpeed = 2f;

    void Update()
    {
        UpdateAltitude();
        UpdateSpeed();
        UpdateREC();
    }

    void UpdateAltitude()
    {
        if (droneTransform != null && altNumberText != null)
        {
            float altitude = Mathf.Max(0f, droneTransform.position.y);
            altNumberText.text = altitude.ToString("F1");
        }
    }

    void UpdateSpeed()
    {
        if (droneRb != null && speedNumberText != null)
        {
            float speed = droneRb.linearVelocity.magnitude;
            speedNumberText.text = speed.ToString("F1");
        }
    }

    void UpdateREC()
    {
        if (recImage != null)
        {
            Color c = recImage.color;
            c.a = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            recImage.color = c;
        }
    }
}