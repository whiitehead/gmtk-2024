
using UnityEngine;

public class GameUtils 
{



    public static Quaternion GetAngleToMouse(Transform target)
    {
        Vector3 screenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = screenPosition - target.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        return Quaternion.Euler(0, 0, angle);
    }

}
