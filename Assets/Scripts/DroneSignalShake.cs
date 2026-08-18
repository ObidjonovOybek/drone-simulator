using UnityEngine;

public class DroneSignalShake : MonoBehaviour
{
    public Transform player;
    public Transform drone;

    [Header("Signal Distance")]
    public float goodSignalDistance = 30f;
    public float weakSignalDistance = 60f;
    public float lostSignalDistance = 100f;

    [Header("Shake")]
    public float maxPositionShake = 0.08f;
    public float maxRotationShake = 1.2f;

    [Range(0f, 1f)] public float signalStrength = 1f;
    public float currentDistance;

    private Vector3 baseLocalPos;
    private Quaternion baseLocalRot;

    void Start()
    {
        baseLocalPos = transform.localPosition;
        baseLocalRot = transform.localRotation;
    }

    void LateUpdate()
    {
        if (player == null || drone == null) return;

        currentDistance = Vector3.Distance(player.position, drone.position);

        signalStrength = 1f - Mathf.InverseLerp(goodSignalDistance, lostSignalDistance, currentDistance);
        signalStrength = Mathf.Clamp01(signalStrength);

        float glitch = 1f - signalStrength;

        Vector3 posOffset = Random.insideUnitSphere * (maxPositionShake * glitch);

        Vector3 rotOffset = new Vector3(
            Random.Range(-maxRotationShake, maxRotationShake),
            Random.Range(-maxRotationShake, maxRotationShake),
            Random.Range(-maxRotationShake, maxRotationShake)
        ) * glitch;

        transform.localPosition = baseLocalPos + posOffset;
        transform.localRotation = baseLocalRot * Quaternion.Euler(rotOffset);
    }

    public float GetDistance()
    {
        return currentDistance;
    }

    public float GetSignal()
    {
        return signalStrength;
    }
}