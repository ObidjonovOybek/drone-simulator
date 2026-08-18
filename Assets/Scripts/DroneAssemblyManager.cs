using UnityEngine;

public class DroneAssemblyManager : MonoBehaviour
{
    public DronePartSave[] parts;

    public void AssembleDrone()
    {
        Debug.Log("Assemble bosildi");

        foreach (DronePartSave part in parts)
        {
            if (part != null)
            {
                part.ResetPart();
            }
        }
    }

    public void DisassembleDrone()
    {
        Debug.Log("Disassemble bosildi");

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null)
            {
                Vector3 offset = new Vector3(
                    (i % 5) * 1.5f,
                    0.3f,
                    (i / 5) * 1.5f
                );

                parts[i].transform.position += offset;
            }
        }
    }
}