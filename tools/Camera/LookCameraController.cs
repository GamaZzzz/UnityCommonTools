using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LookCameraController : MonoBehaviour
{

    public static LookCameraController instance;

    public Transform cameraPivot;
    public float distance = 5f;
    public float distanceMax = 30f;
    public float distanceMin = 5f;
    public float mouseSpeed = 8f;
    public float mouseScroll = 15f;
    public float rotateYMax = 89.5f;
    public float rotateYMin = 0f;
    public float mouseSmoothingFactor = 0.08f;
    public float camDistanceSpeed = 0.7f;
    public float camBottomDistance = 1f;
    public float firstPersonThreshold = 0.8f;
    public float characterFadeThreshold = 1.8f;

    private Vector3 desiredPosition;
    private float desiredDistance;
    private float lastDistance;
    private float mouseX = 0f;
    private float mouseXSmooth = 0f;
    private float mouseXVel;
    private float mouseY = 0f;
    private float mouseYSmooth = 0f;
    private float mouseYVel;
    private float mouseYMin = -89.5f;
    private float mouseYMax = 89.5f;
    private float distanceVel;
    private bool camBottom;
    private bool constraint;

    private static float halfFieldOfView;
    private static float planeAspect;
    private static float halfPlaneHeight;
    private static float halfPlaneWidth;



    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        distance = Mathf.Clamp(distance, 0.05f, distanceMax);
        desiredDistance = distance;

        halfFieldOfView = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
        planeAspect = Camera.main.aspect;
        halfPlaneHeight = Camera.main.nearClipPlane * Mathf.Tan(halfFieldOfView);
        halfPlaneWidth = halfPlaneHeight * planeAspect;

        mouseX = 0f;
        mouseY = 15f;
    }

    Vector3 finger0;
    Vector3 finger1;

    void LateUpdate()
    {
        if (cameraPivot == null)
        {
            Debug.Log("Error: No cameraPivot found! Please read the manual for further instructions.");
            return;
        }

        GetInput();

        GetDesiredPosition();

        PositionUpdate();
    }

    float lastFingerDistance = 0f;

    void GetInput()
    {
        bool constrainMouseY = transform.position.y - cameraPivot.transform.position.y <= 0;
        float scrollDelta = 0f;
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            mouseX += Input.GetAxis("Mouse X") * mouseSpeed;

            if (constrainMouseY)
            {
                if (Input.GetAxis("Mouse Y") < 0)
                {
                    mouseY -= Input.GetAxis("Mouse Y") * mouseSpeed;
                }
            }
            else
            {
                mouseY -= Input.GetAxis("Mouse Y") * mouseSpeed;
            }
        }

        //scrollDelta = Input.GetAxis("Mouse ScrollWheel");

#elif UNITY_ANDROID
        if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Moved && !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
        {
            mouseX += Input.touches[0].deltaPosition.x * mouseSpeed * mouseSmoothingFactor;

            if (constrainMouseY)
            {
                if (Input.touches[0].deltaPosition.y < 0)
                {
                    mouseY -= Input.touches[0].deltaPosition.y * mouseSpeed * mouseSmoothingFactor;
                }
            }
            else
            {
                mouseY -= Input.touches[0].deltaPosition.y * mouseSpeed * mouseSmoothingFactor;
            }
        }
        //else if(Input.touchCount > 1)
        //{
        //    if(Input.touches[0].phase == TouchPhase.Began 
        //        || Input.touches[1].phase == TouchPhase.Began)
        //    {
        //        finger0 = Input.touches[0].position;
        //        finger1 = Input.touches[1].position;
        //    }

        //    if (Input.GetTouch(0).phase == TouchPhase.Moved
        //        || Input.GetTouch(1).phase == TouchPhase.Moved)
        //    {
        //        var deltaDis0 = Input.GetTouch(0).deltaPosition.magnitude;
        //        var deltaDis1 = Input.GetTouch(1).deltaPosition.magnitude;
        //        scrollDelta = (deltaDis0 + deltaDis1) * Time.deltaTime;

        //        lastFingerDistance = Vector3.Distance(finger0, finger1);

        //        float curFingerDistance = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
        //        float deltaDistance = curFingerDistance - lastFingerDistance;

        //        scrollDelta *= -deltaDistance / Mathf.Abs(deltaDistance);

        //        finger0 = Input.GetTouch(0).position;
        //        finger1 = Input.GetTouch(1).position;
        //    }
        //}
#endif
        mouseY = ClampAngle(mouseY, rotateYMin, rotateYMax);
        mouseXSmooth = Mathf.SmoothDamp(mouseXSmooth, mouseX, ref mouseXVel, mouseSmoothingFactor);
        mouseYSmooth = Mathf.SmoothDamp(mouseYSmooth, mouseY, ref mouseYVel, mouseSmoothingFactor);

        if (constrainMouseY)
            mouseYMin = mouseY;
        else
            mouseYMin = rotateYMin;

        mouseYSmooth = ClampAngle(mouseYSmooth, mouseYMin, mouseYMax);

        desiredDistance = desiredDistance + scrollDelta * mouseScroll;

        if (desiredDistance > distanceMax)
            desiredDistance = distanceMax;

        if (desiredDistance < distanceMin)
            desiredDistance = distanceMin;
    }


    void GetDesiredPosition()
    {
        distance = desiredDistance;

        distance = Mathf.SmoothDamp(lastDistance, distance, ref distanceVel, camDistanceSpeed);

        if (distance < distanceMin)
            distance = distanceMin;

        lastDistance = distance;

        desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, distance); // if the camera view was blocked, then this is the new "forced" position
    }


    void PositionUpdate()
    {
        transform.position = desiredPosition;

        if (distance > 0.05)
            transform.LookAt(cameraPivot);
    }

    Vector3 GetCameraPosition(float xAxis, float yAxis, float distance)
    {
        Vector3 offset = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(xAxis, yAxis, 0);
        return cameraPivot.position + rotation * offset;
    }

    float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360 || angle > 360)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
        }

        return Mathf.Clamp(angle, min, max);
    }


    public struct ClipPlaneVertexes
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }
}
