using UnityEngine;
using UnityEditor;

namespace SoftKitty.MasterNavigationMap
{
    [CustomEditor(typeof(MapCreatorTag))]
    public class MapCreatorTag_Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            MapCreatorTag myTarget = (MapCreatorTag)target;

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            myTarget._water = GUILayout.Toggle(myTarget._water," Water");
            GUILayout.EndHorizontal();

            if (!myTarget._water)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Tag: ",GUILayout.Width(60));
                myTarget._tag = GUILayout.TextField(myTarget._tag,GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
            else
            {
                myTarget._tag = "";
            }

            if (GUI.changed && !Application.isPlaying) UnityEditor.EditorUtility.SetDirty(myTarget);
        }
    }
}
