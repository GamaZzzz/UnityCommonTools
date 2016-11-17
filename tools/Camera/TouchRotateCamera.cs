using UnityEngine;

public class TouchRotateCamera : MonoBehaviour
{
    [SerializeField]
    private Transform aroundTarget;
    [SerializeField]
    private float rotateRatio = 65f;

    public float minAngle { get; private set; }
    public float RspeedX { get; private set; }
    public float camHeight { get; private set; }
    public float maxAngle { get; private set; }
    public Quaternion direction { get; private set; }

    void LateUpdate()
    {
        if (!aroundTarget)
        {
            return;
        }

        float v = 0f;
        float h = 0f;
#if UNITY_EDITOR
        v = Input.GetAxis("Horizontal");
        h = Input.GetAxis("Vertical");
#else
         if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchDeltaPosition = touch.deltaPosition;
            v = -touchDeltaPosition.x;
        }
#endif
        float angleH = - v * 2 * Mathf.PI / Screen.width;
        float angleV = - h * 2 * Mathf.PI / Screen.width;

        transform.RotateAround(aroundTarget.position, Vector3.up, angleH * rotateRatio);
        
        transform.RotateAround(aroundTarget.position, aroundTarget.TransformDirection(Vector3.right), angleV * rotateRatio);

        transform.LookAt(aroundTarget);
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}

