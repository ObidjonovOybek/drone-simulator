using UnityEngine;

public class SimplePropellerSpin : MonoBehaviour
{
    [SerializeField] private Transform propellerGroup;
    [SerializeField] private float rotationSpeed = 2000f;

    void Update()
    {
        if (propellerGroup == null) return;

        propellerGroup.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}