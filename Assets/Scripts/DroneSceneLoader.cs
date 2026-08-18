using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DroneSceneLoader : MonoBehaviour
{
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    [SerializeField] private string sceneName;

    private void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene biriktirilmagan: " + gameObject.name);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
    }
#endif
}