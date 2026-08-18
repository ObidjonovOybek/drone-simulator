using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -6f);
    public float smooth = 10f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPos = target.TransformPoint(offset); // dronning orqasida
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smooth);
        transform.LookAt(target.position + Vector3.up * 0.5f);
    }
}   