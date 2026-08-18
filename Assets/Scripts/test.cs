using UnityEngine;

public class AxisViewer : MonoBehaviour
{
    void Update()
    {
        for (int i = 1; i <= 10; i++)
        {
            float value = Input.GetAxis("Axis" + i);
            if (Mathf.Abs(value) > 0.2f)
            {
                Debug.Log("Axis " + i + " : " + value);
            }
        }
    }
}