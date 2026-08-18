using UnityEngine;

public class DronePartInfo : MonoBehaviour
{
    public string partName;

    [TextArea(3, 8)]
    public string description;

    public Sprite partImage;
}