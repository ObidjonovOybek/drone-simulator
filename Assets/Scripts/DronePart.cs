using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DronePart : MonoBehaviour
{
    [Header("Placement Points")]
    public Transform tablePoint;
    public Transform snapPoint;

    [Header("Assembly Order")]
    public int assemblyStep = 1;

    [Header("Snap Settings")]
    public float snapDistance = 0.2f;

    [Header("State")]
    public bool isPlaced = false;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        MoveToTable();
    }

    private void Update()
    {
        if (isPlaced) return;
        if (snapPoint == null) return;
        if (AssemblyManager.Instance == null) return;
        if (!AssemblyManager.Instance.CanPlace(assemblyStep)) return;

        float dist = Vector3.Distance(transform.position, snapPoint.position);

        if (dist <= snapDistance)
        {
            SnapToTarget();
        }
    }

    public void MoveToTable()
    {
        if (tablePoint == null) return;

        transform.position = tablePoint.position;
        transform.rotation = tablePoint.rotation;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }
    }

    public void SnapToTarget()
    {
        transform.position = snapPoint.position;
        transform.rotation = snapPoint.rotation;
        isPlaced = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        AssemblyManager.Instance.CompleteStep();
    }
}