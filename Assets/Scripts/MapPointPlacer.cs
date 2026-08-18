using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapPointPlacer : MonoBehaviour, IPointerClickHandler
{
    [Header("Map UI")]
    public RectTransform mapRect;

    [Header("World Bounds")]
    public Transform worldMin;
    public Transform worldMax;

    [Header("Point Prefab")]
    public GameObject pointPrefab;
    public Transform pointParent;

    [Header("Route Line")]
    public LineRenderer lineRenderer;

    [Header("State")]
    public bool pointMode = false;

    public List<Vector2> mapPoints = new List<Vector2>();
    public List<Vector3> worldPoints = new List<Vector3>();
    private List<GameObject> spawnedPoints = new List<GameObject>();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!pointMode) return;
        if (mapPoints.Count >= 2) return;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
            return;

        float normalizedX = Mathf.InverseLerp(-mapRect.rect.width / 2f, mapRect.rect.width / 2f, localPoint.x);
        float normalizedY = Mathf.InverseLerp(-mapRect.rect.height / 2f, mapRect.rect.height / 2f, localPoint.y);

        float worldX = Mathf.Lerp(worldMin.position.x, worldMax.position.x, normalizedX);
        float worldZ = Mathf.Lerp(worldMin.position.z, worldMax.position.z, normalizedY);

        Vector3 worldPos = new Vector3(worldX, 8f, worldZ);

        mapPoints.Add(localPoint);
        worldPoints.Add(worldPos);

        if (pointPrefab != null && pointParent != null)
        {
            GameObject p = Instantiate(pointPrefab, pointParent);
            RectTransform rt = p.GetComponent<RectTransform>();
            rt.anchoredPosition = localPoint;
            spawnedPoints.Add(p);
        }

        UpdateLine();
    }

    public void EnablePointMode()
    {
        pointMode = true;
    }

    public void DisablePointMode()
    {
        pointMode = false;
    }

    public void ClearPoints()
    {
        mapPoints.Clear();
        worldPoints.Clear();

        foreach (var p in spawnedPoints)
        {
            if (p != null) Destroy(p);
        }
        spawnedPoints.Clear();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    void UpdateLine()
    {
        if (lineRenderer == null) return;

        if (worldPoints.Count < 2)
        {
            lineRenderer.positionCount = worldPoints.Count;
            for (int i = 0; i < worldPoints.Count; i++)
                lineRenderer.SetPosition(i, worldPoints[i]);
            return;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, worldPoints[0]);
        lineRenderer.SetPosition(1, worldPoints[1]);
    }
}