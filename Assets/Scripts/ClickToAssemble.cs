using UnityEngine;

public class ClickToAssemble : MonoBehaviour
{
    private DronePartSave partSave;
    private DronePartInfo partInfo;

    void Start()
    {
        partSave = GetComponentInParent<DronePartSave>();
        partInfo = GetComponentInParent<DronePartInfo>();
    }

    void OnMouseEnter()
    {
        if (partSave != null)
        {
            partSave.Highlight(true);
        }

        if (partInfo != null && PartInfoUI.Instance != null)
        {
            PartInfoUI.Instance.ShowPartInfo(partInfo, this);
        }
    }

    void OnMouseExit()
    {
        if (partSave != null)
        {
            partSave.Highlight(false);
        }

        if (PartInfoUI.Instance != null)
        {
            PartInfoUI.Instance.ClosePanel();
        }
    }

    void OnMouseOver()
    {
        if (partSave == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            AssemblePart();
        }

        if (Input.GetMouseButtonDown(1))
        {
            ReturnPart();
        }
    }

    public void AssemblePart()
    {
        if (partSave != null)
        {
            partSave.ResetPart();
            Debug.Log(gameObject.name + " yig‘ildi");
        }
    }

    public void ReturnPart()
    {
        if (partSave != null)
        {
            partSave.ReturnToScatteredPosition();
            Debug.Log(gameObject.name + " orqaga qaytdi");
        }
    }
}   