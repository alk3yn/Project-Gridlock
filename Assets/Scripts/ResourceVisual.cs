using UnityEngine;

public class ResourceVisual : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 targetPos;
    private float moveProgress;
    private float speed;

    public void StartMoving(Vector3 from, Vector3 to, float tickDuration)
    {
        startPos = from;
        targetPos = to;
        moveProgress = 0f;

        // Speed ensures it arrives exactly when the next tick fires
        speed = 1f / tickDuration;
    }

    void Update()
    {
        if (moveProgress < 1f)
        {
            moveProgress += Time.deltaTime * speed;
            // Smooth lerp visually, even though logic snapped instantly
            transform.position = Vector3.Lerp(startPos, targetPos, moveProgress);
        }
    }
}