using UnityEngine;

public class CMFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Third Person Position")]
    public Vector3 offset = new Vector3(0f, 2f, -5f);

    [Header("Look Settings")]
    public float lookHeight = 0.5f;

    [Header("Smooth Settings")]
    public float followSmooth = 5f;
    public float rotateSmooth = 4f;

    private Vector3 currentVelocity;

    void OnEnable()
    {
        SnapToTarget();
    }

    public void SnapToTarget()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = desiredPosition;

        Vector3 lookTarget = target.position + Vector3.up * lookHeight;
        Vector3 dir = lookTarget - transform.position;

        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);

        currentVelocity = Vector3.zero;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(offset);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / Mathf.Max(0.01f, followSmooth)
        );

        Vector3 lookTarget = target.position + Vector3.up * lookHeight;
        Vector3 dir = lookTarget - transform.position;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                rotateSmooth * Time.deltaTime
            );
        }
    }
}