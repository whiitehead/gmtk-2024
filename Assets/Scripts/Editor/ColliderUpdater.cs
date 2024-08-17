using System.Collections.Generic;
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
            var selectedGameObject = Selection.activeGameObject;
            
            if (selectedGameObject is null)
            {
                Debug.LogWarning("No GameObject selected.");
                return;
            }
            
            var collider = selectedGameObject.GetComponent<PolygonCollider2D>();

            if (collider is null)
            {
                collider = selectedGameObject.AddComponent<PolygonCollider2D>();
                collider.useDelaunayMesh = true;
            }

            var sprite = selectedGameObject.GetComponent<SpriteRenderer>()?.sprite;

            if (sprite is null || sprite.GetPhysicsShapeCount() <= 0)
            {
                Debug.LogWarning("No valid sprite on object.");
                return;
            }

            var physicsShape = new List<Vector2>(sprite.GetPhysicsShapePointCount(0));
            
            sprite.GetPhysicsShape(0, physicsShape);
            collider.SetPath(0, physicsShape);
        }
    }
}
