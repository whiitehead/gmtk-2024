using UnityEditor;
using UnityEngine;

public class ColliderUpdater : EditorWindow
{
    [MenuItem("Window/Collider Updater")]
    public static void ShowWindow()
    {
        GetWindow((typeof(ColliderUpdater)));
    }
    
    private void OnGUI()
    {
        if (GUILayout.Button("Generate Collider"))
        {
            UpdateCollider();
        }
    }

    private void UpdateCollider()
    {
        var selectedGameObject = Selection.activeGameObject;
            
        if (selectedGameObject is null)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }
            
        var collider = selectedGameObject.GetComponent<PolygonCollider2D>();

        if (collider != null)
        {
            DestroyImmediate(collider);
        }

        collider = selectedGameObject.AddComponent<PolygonCollider2D>();
        collider.useDelaunayMesh = true;
    }
    
}
