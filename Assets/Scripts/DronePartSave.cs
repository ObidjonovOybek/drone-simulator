using UnityEngine;
using System.Collections;

public class DronePartSave : MonoBehaviour
{
    [Header("Hover")]
    public Color hoverColor = Color.yellow;

    [Header("Animation")]
    public float moveDuration = 0.5f;

    private Vector3 assembledPosition;
    private Quaternion assembledRotation;

    private Vector3 scatteredPosition;
    private Quaternion scatteredRotation;

    private Renderer[] renderers;
    private Color[] originalColors;

    private Coroutine moveCoroutine;

    private void Awake()
    {
        // Bu detalning yig'ilgan joyi deb olinadi
        assembledPosition = transform.position;
        assembledRotation = transform.rotation;

        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                originalColors[i] = renderers[i].material.color;
            }
        }
    }

    private void Start()
    {
        // Agar detal Start gacha tarqalgan joyga ko‘chirilgan bo‘lsa,
        // shu joy orqaga qaytish nuqtasi bo‘ladi
        SaveScatteredState();
    }

    public void SaveScatteredState()
    {
        scatteredPosition = transform.position;
        scatteredRotation = transform.rotation;
    }

    public void Highlight(bool state)
    {
        if (renderers == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null || renderers[i].material == null) continue;
            renderers[i].material.color = state ? hoverColor : originalColors[i];
        }
    }

    public void ResetPart()
    {
        MoveTo(assembledPosition, assembledRotation);
    }

    public void ReturnToScatteredPosition()
    {
        MoveTo(scatteredPosition, scatteredRotation);
    }

    private void MoveTo(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveRoutine(targetPosition, targetRotation));
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = time / moveDuration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        moveCoroutine = null;
    }
}