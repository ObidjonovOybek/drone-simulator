using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MissionPlanner : MonoBehaviour, IPointerClickHandler
{
    [Header("Map UI")]
    public RectTransform mapRect;
    public RectTransform pointParent;
    public GameObject pointPrefab;

    [Header("World Bounds")]
    public Transform worldMin;
    public Transform worldMax;

    [Header("Route Settings")]
    public LineRenderer routeLine;
    public float flightHeight = 8f;
    public int laneCount = 6;

    [Header("State")]
    public bool isPlacementMode = false;

    public List<Vector2> mapPoints = new List<Vector2>();
    public List<Vector3> worldCornerPoints = new List<Vector3>();
    public List<Vector3> generatedRoute = new List<Vector3>();

    private List<GameObject> spawnedPointObjects = new List<GameObject>();

    public void EnablePlacementMode()
    {
        isPlacementMode = true;
    }

    public void ClearAll()
    {
        isPlacementMode = false;

        mapPoints.Clear();
        worldCornerPoints.Clear();
        generatedRoute.Clear();

        foreach (GameObject obj in spawnedPointObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedPointObjects.Clear();

        if (routeLine != null)
            routeLine.positionCount = 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPlacementMode) return;
        if (mapPoints.Count >= 4) return;

        Vector2 localPoint;
        bool ok = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        if (!ok) return;

        mapPoints.Add(localPoint);

        Vector3 worldPoint = MapToWorld(localPoint);
        worldCornerPoints.Add(worldPoint);

        if (pointPrefab != null && pointParent != null)
        {
            GameObject pointObj = Instantiate(pointPrefab, pointParent);
            RectTransform rt = pointObj.GetComponent<RectTransform>();
            rt.anchoredPosition = localPoint;
            spawnedPointObjects.Add(pointObj);
        }

        if (mapPoints.Count == 4)
        {
            isPlacementMode = false;
            GenerateParallelRoute();
            DrawRoute();
        }
    }

    Vector3 MapToWorld(Vector2 localPoint)
    {
        float normalizedX = Mathf.InverseLerp(-mapRect.rect.width * 0.5f, mapRect.rect.width * 0.5f, localPoint.x);
        float normalizedY = Mathf.InverseLerp(-mapRect.rect.height * 0.5f, mapRect.rect.height * 0.5f, localPoint.y);

        float worldX = Mathf.Lerp(worldMin.position.x, worldMax.position.x, normalizedX);
        float worldZ = Mathf.Lerp(worldMin.position.z, worldMax.position.z, normalizedY);

        return new Vector3(worldX, flightHeight, worldZ);
    }

    void GenerateParallelRoute()
    {
        generatedRoute.Clear();

        if (worldCornerPoints.Count < 4) return;

        float minX = worldCornerPoints[0].x;
        float maxX = worldCornerPoints[0].x;
        float minZ = worldCornerPoints[0].z;
        float maxZ = worldCornerPoints[0].z;

        for (int i = 1; i < worldCornerPoints.Count; i++)
        {
            Vector3 p = worldCornerPoints[i];
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.z < minZ) minZ = p.z;
            if (p.z > maxZ) maxZ = p.z;
        }

        if (laneCount < 2) laneCount = 2;

        float step = (maxX - minX) / (laneCount - 1);

        bool forward = true;

        for (int i = 0; i < laneCount; i++)
        {
            float x = minX + step * i;

            Vector3 a = new Vector3(x, flightHeight, minZ);
            Vector3 b = new Vector3(x, flightHeight, maxZ);

            if (forward)
            {
                generatedRoute.Add(a);
                generatedRoute.Add(b);
            }
            else
            {
                generatedRoute.Add(b);
                generatedRoute.Add(a);
            }

            forward = !forward;
        }
    }

    void DrawRoute()
    {
        if (routeLine == null) return;

        routeLine.positionCount = generatedRoute.Count;

        for (int i = 0; i < generatedRoute.Count; i++)
        {
            routeLine.SetPosition(i, generatedRoute[i]);
        }
    }
}