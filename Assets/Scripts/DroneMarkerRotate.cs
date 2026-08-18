using UnityEngine;

public class DroneMarkerMapFollow : MonoBehaviour
{
    [Header("Drone")]
    public Transform droneTransform;

    [Header("World Bounds")]
    public Transform worldMin;
    public Transform worldMax;

    [Header("Small Map")]
    public RectTransform smallMapRect;
    public RectTransform smallMarker;

    [Header("Big Map")]
    public RectTransform bigMapRect;
    public RectTransform bigMarker;

    [Header("Settings")]
    public bool swapXZ = false;
    public bool invertX = false;
    public bool rotateMarker = true;

    [Header("Marker Limits")]
    public float edgePadding = 8f;

    void Update()
    {
        if (droneTransform == null || worldMin == null || worldMax == null)
            return;

        UpdateMarkerPosition(smallMapRect, smallMarker);
        UpdateMarkerPosition(bigMapRect, bigMarker);

        if (rotateMarker)
        {
            float yaw = droneTransform.eulerAngles.y;

            if (smallMarker != null)
                smallMarker.localRotation = Quaternion.Euler(0f, 0f, -yaw);

            if (bigMarker != null)
                bigMarker.localRotation = Quaternion.Euler(0f, 0f, -yaw);
        }
    }

    void UpdateMarkerPosition(RectTransform mapRect, RectTransform markerRect)
    {
        if (mapRect == null || markerRect == null) return;

        float normalizedX;
        float normalizedY;

        if (!swapXZ)
        {
            normalizedX = Mathf.InverseLerp(worldMin.position.x, worldMax.position.x, droneTransform.position.x);
            normalizedY = Mathf.InverseLerp(worldMin.position.z, worldMax.position.z, droneTransform.position.z);
        }
        else
        {
            normalizedX = Mathf.InverseLerp(worldMin.position.z, worldMax.position.z, droneTransform.position.z);
            normalizedY = Mathf.InverseLerp(worldMin.position.x, worldMax.position.x, droneTransform.position.x);
        }

        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedY = Mathf.Clamp01(normalizedY);

        if (invertX)
            normalizedX = 1f - normalizedX;

        float halfWidth = (mapRect.rect.width * 0.5f) - edgePadding;
        float halfHeight = (mapRect.rect.height * 0.5f) - edgePadding;

        float posX = Mathf.Lerp(-halfWidth, halfWidth, normalizedX);
        float posY = Mathf.Lerp(-halfHeight, halfHeight, normalizedY);

        markerRect.anchorMin = new Vector2(0.5f, 0.5f);
        markerRect.anchorMax = new Vector2(0.5f, 0.5f);
        markerRect.pivot = new Vector2(0.5f, 0.5f);
        markerRect.anchoredPosition = new Vector2(posX, posY);
    }
}