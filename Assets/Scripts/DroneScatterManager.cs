using UnityEngine;

public class DroneScatterManager : MonoBehaviour
{
    public DronePartSave[] parts;
    public Transform tableCenter1;
    public Transform tableCenter2;

    public int columnsPerTable = 4;
    public float spacingX = 0.25f;
    public float spacingZ = 0.25f;

    public float tableY1 = 1.0f;
    public float tableY2 = 1.0f;

    void Start()
    {
        Debug.Log("DroneScatterManager ishladi");
        ScatterNow();
    }

    public void ScatterNow()
    {
        if (parts == null || parts.Length == 0)
        {
            Debug.LogError("Parts bo'sh");
            return;
        }

        if (tableCenter1 == null || tableCenter2 == null)
        {
            Debug.LogError("TableCenter ulanmagan");
            return;
        }

        int half = Mathf.CeilToInt(parts.Length / 2f);

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == null) continue;

            Transform center;
            float tableY;
            int localIndex;

            if (i < half)
            {
                center = tableCenter1;
                tableY = tableY1;
                localIndex = i;
            }
            else
            {
                center = tableCenter2;
                tableY = tableY2;
                localIndex = i - half;
            }

            int row = localIndex / columnsPerTable;
            int col = localIndex % columnsPerTable;

            Vector3 newPos = center.position + new Vector3(col * spacingX, 0f, row * spacingZ);
            newPos.y = tableY;

            parts[i].transform.position = newPos;
            parts[i].transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            Debug.Log(parts[i].name + " ko‘chirildi");
        }
    }
}