using UnityEngine;
using DG.Tweening;

public class TouchRotateCamera : MonoBehaviour
{
    [SerializeField]
    private Transform lookAtTarget;
    [SerializeField]
    private Transform cameraRoot;
    [SerializeField]
    private float smoothSpeed = 65f;
    [SerializeField]
    private float maxDistance = 4f;
    [SerializeField]
    private float minDistance = 1f;
    [SerializeField]
    private float distance = 2.76f;
    [SerializeField]
    private float defaultAngle = 63f;

    private Vector3 lastPosition  = Vector3.zero;
    private float rotateV = 0f;
    private float rotateH = 0f;
    private float lastDistance = 0f;

    void Start()
    {
        cameraRoot.localRotation = Quaternion.Euler(defaultAngle, 0f, 0f);
    }

    void Update()
    {
        if (!lookAtTarget)
        {
            return;
        }

        float v = 0f;
        float h = 0f;

        rotateV = cameraRoot.transform.localEulerAngles.x;
        rotateH = cameraRoot.transform.localEulerAngles.z;

        float deltaDistance = 0f;
#if UNITY_EDITOR

        deltaDistance = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButton(0))
        {
            v = Input.GetAxis("Mouse X");
            h = -Input.GetAxis("Mouse Y");
#else
         if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchDeltaPosition = touch.deltaPosition;
            v = touchDeltaPosition.x;
            h = -touchDeltaPosition.y;
#endif
            //Rotate
            float angleH = v * 360 / Screen.width;
            float angleV = h * 360 / Screen.width;

            angleH *= smoothSpeed;

            //Target Rotate
            if (Mathf.Abs(v) > Mathf.Abs(h))
            {
                lookAtTarget.transform.localRotation *= Quaternion.Euler(0, 0, angleH * Time.deltaTime * smoothSpeed);
            }
            //Camera Rotate
            rotateV = Mathf.Clamp(angleV * smoothSpeed + rotateV, 275, 355);
        }
        else if(Input.touchCount > 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved 
                || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                var pos1 = Input.GetTouch(0).deltaPosition.x;
                var pos2 = Input.GetTouch(1).deltaPosition.x;
                deltaDistance = pos1 + pos2;
            }
        }

        if(Mathf.Abs(v) < Mathf.Abs(h))
        {
            cameraRoot.localEulerAngles = Vector3.Lerp(cameraRoot.localEulerAngles, new Vector3(rotateV, 0f, 0f), Time.deltaTime * smoothSpeed * 0.5f);
        }

        //Zoom View
        distance = Mathf.Clamp((distance + deltaDistance), minDistance, maxDistance);

        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, -distance), Time.deltaTime * smoothSpeed);
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

