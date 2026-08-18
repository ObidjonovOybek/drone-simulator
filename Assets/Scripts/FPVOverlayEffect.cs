using UnityEngine;
using UnityEngine.UI;

public class FPVOverlayEffect : MonoBehaviour
{
    [Header("UI References")]
    public Image fpvOverlay;
    public Image recIcon;
    public Image noiseImage;

    [Header("REC Blink")]
    public float recBlinkSpeed = 2f;

    [Header("Noise Flicker")]
    public float noiseMinAlpha = 0.08f;
    public float noiseMaxAlpha = 0.18f;
    public float noiseFlickerSpeed = 10f;

    [Header("Overlay Shake")]
    public RectTransform overlayRect;
    public float shakeAmount = 1.5f;
    public float shakeSpeed = 18f;

    private Vector2 originalOverlayPos;

    void Awake()
    {
        if (overlayRect != null)
            originalOverlayPos = overlayRect.anchoredPosition;
    }

    void Update()
    {
        // REC blink
        if (recIcon != null)
        {
            Color c = recIcon.color;
            c.a = (Mathf.Sin(Time.time * recBlinkSpeed * Mathf.PI) > 0f) ? 1f : 0.25f;
            recIcon.color = c;
        }

        // Noise flicker
        if (noiseImage != null)
        {
            Color c = noiseImage.color;
            float t = (Mathf.Sin(Time.time * noiseFlickerSpeed) + 1f) * 0.5f;
            c.a = Mathf.Lerp(noiseMinAlpha, noiseMaxAlpha, t);
            noiseImage.color = c;
        }

        // Slight shake
        if (overlayRect != null)
        {
            float x = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float y = Mathf.Cos(Time.time * shakeSpeed * 0.85f) * shakeAmount;
            overlayRect.anchoredPosition = originalOverlayPos + new Vector2(x, y);
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}