using UnityEngine;

public class AssemblyManager : MonoBehaviour
{
    public static AssemblyManager Instance;

    public int currentStep = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool CanPlace(int step)
    {
        return step == currentStep;
    }

    public void CompleteStep()
    {
        currentStep++;
        Debug.Log("Keyingi bosqich: " + currentStep);
    }
}